using System;
using System.Text;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for SignalActionParser.
	/// </summary>
	public class SignalActionParser
	{
	    static bool _UseSignalClass = false;
	    static SignalActionParser()
	    {
	        string useSignalClass = System.Configuration.ConfigurationSettings.AppSettings["UseSignalClass"];
	        if(useSignalClass != null)
	        {
	            useSignalClass = useSignalClass.ToLower ();
	            _UseSignalClass = useSignalClass != "false";
	        }
	    }
	    
		string _Action;
		int _Index;

		protected string Remaining ()
		{
			if (Eof ()) return "";
			return _Action.Substring (_Index, _Action.Length - _Index);
		}

		protected char LookAhead ()
		{
			return _Action [_Index];
		}

		protected char LookAhead (int lookAheadAmount)
		{
			return _Action [_Index + lookAheadAmount];
		}

		protected bool Eof ()
		{
			return _Index >= _Action.Length;
		}

		protected void Increment ()
		{
			_Index++;
		}

		protected void Match (char match)
		{
			if (Eof ())
			{
				throw new ArgumentException ("No Match", match.ToString ());
			}

			char ch = LookAhead ();
			if (match != ch)
			{
				throw new ArgumentException ("No Match", match.ToString ());
			}
			Increment ();
		}

		protected void Match (string match)
		{
			try 
			{
				foreach (char ch in match)
				{
					Match (ch);
				}
			} 
			catch (ArgumentException argex)
			{
				throw new ArgumentException (argex.Message, match, argex);
			}
		}

		protected void MatchAny (string matchPattern)
		{
			bool found;
			do 
			{
				found = false;
				foreach (char ch in matchPattern)
				{
					while (Eof () == false && LookAhead () == ch)
					{
						Match (ch);
						found = true;
					}
				}
			} while (found);
		}

		protected string MatchTill (string matchPattern)
		{
			StringBuilder builder = new StringBuilder ();
			while (Eof () == false)
			{
				char next = LookAhead ();
				foreach (char ch in matchPattern)
				{
					if (ch == next)
					{
						return builder.ToString ();
					}
				}
				builder.Append (next);
				Match (next);
			}
			return builder.ToString ();
		}

		protected string GetArgs ()
		{
			string args = "";
			if (!Eof ())
			{
				Match ('(');
				args = MatchTill (")");
				if (!Eof ())
				{
					Match (')');
				}
				args = args.Trim ();
				if (args != "")
				{
					args = ", " + args;
				}
			}
			return args;
		}

		protected string GetSignalForPort (string signalClass, string port, out string loneSignal)
		{
			string signal = MatchTill ("(");

			if (signal.IndexOf (".") == -1)
			{
				loneSignal = signal;
                if(_UseSignalClass)
                {
                    signal = signalClass + "." + signal;
                } 
                else
                {
                    signal = port + "Signals" + "." + signal;
                }
            } 
			else 
			{
				string[] signalParts = signal.Split ('.');
				if (signalParts.Length > 2)
				{
					throw new ArgumentException ("Signal should have only one . separator", signal);
				}

				loneSignal = signalParts [1];
				signal = signalParts [0];
				if (!signal.EndsWith ("Signals"))
				{
					signal = signal + "Signals";
				}
				signal = signal + "." + signalParts [1];
			}
			if (signal.StartsWith ("Qualified"))
			{
				throw new ArgumentException ("Signal should not be marked as Qualified: ", signal);
			}

			return signal;
		}

		protected virtual void DoPortSignalToken (string port, string signalClass, string signal, string args)
		{
		}

		public string Parse (string signalClass, string action)
		{
			/// format:
			///   either:
			///      ^Signal [(Arg)]
			///      ^Port.[SignalClass.]Signal [(Arg)]
			///      
			///      SignalClass is optional 
			///            - does not need the Signals at the end of the class name 
			///            - will be automatically appended if not found.
			///      Arg is optional
			///      If no arg then () is also optional
			try 
			{
				StringBuilder builder = new StringBuilder ();

				_Action = action.Trim ();
				Match ("^");
				MatchAny (" \t\n");

				string portOrSignal = MatchTill (".(");

				if (Eof ())
				{
					string signal = portOrSignal;
					// only a signal - so send to self and qualify the signal.
					DoPortSignalToken (null, signalClass, signal, null);
					builder.AppendFormat ("AsyncDispatch (new QEvent ({0}.{1}))", signalClass, signal);
				} 
				else 
				{
					switch (LookAhead ())
					{
						case '.': 
						{
							string port = portOrSignal;
							Match ('.');
							string loneSignal;
							string signal = GetSignalForPort (signalClass, port, out loneSignal);
							string args = GetArgs ();
							DoPortSignalToken (port, signalClass, loneSignal, args);
							builder.AppendFormat ("{0}.Send (new QEvent ({1}{2}))", port, signal, args);
						} break;
						case '(':
						{
							string signal = portOrSignal;
							string args = GetArgs ();
							DoPortSignalToken (null, signalClass, signal, args);
							// qualify the signal
							builder.AppendFormat ("AsyncDispatch (new QEvent ({0}.{1}{2}))", signalClass, signal, args);
						} break;
						default: throw new NotSupportedException (LookAhead () + " not supported"); // this indicates a logic problem 
					}
				}

				string remaining = Remaining ();
				remaining = remaining.Trim ();
				if (remaining != "")
				{
					throw new ArgumentException ("Action parsed for signal still has remaining part [" + remaining + "]", action);
				}
				action = builder.ToString ();
				return action;
			} 
			catch (Exception ex)
			{
				throw new ArgumentException ("Action parsed for signal failed with exception: " + ex.Message, action, ex);
			}
		}
	}
}
