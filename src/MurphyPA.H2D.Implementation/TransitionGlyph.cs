using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for TransitionGlyph.
	/// </summary>
	public class TransitionGlyph : GroupGlyphBase, ITransitionGlyph
	{
		Point _From = new Point (10, 20);
		Point _To = new Point (100, 22);

		public TransitionGlyph ()
		{
			BuildContactPoints ();
		}

		public TransitionGlyph (string id, Point point) 
			: base (id)
		{
			int width = _To.X - _From.X;
			int height = _To.Y - _From.Y;
			_From = point;
			_To = new Point (point.X + width, point.Y + height);
			BuildContactPoints ();
		}

		public TransitionGlyph (string id, Rectangle bounds) 
			: base (id)
		{
			_From = bounds.Location;
			_To = new Point (bounds.X + bounds.Width, bounds.Y + bounds.Height);
			BuildContactPoints ();
		}

		public override void Accept (IGlyphVisitor visitor)
		{
			visitor.Visit (this);
		}

		protected void BuildContactPoints ()
		{
			int radius = 5;
			TransitionContactPointCircleGlyph fromEnd = new TransitionContactPointCircleGlyph (_From, radius - 1, this, TransitionContactEnd.From, null);
			AddContactPoint (fromEnd);
			AddContactPoint (new TransitionContactPointCircleGlyph (_To, radius, this, TransitionContactEnd.To, fromEnd));
		}

		protected override void contactPoint_OffsetChanged(IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			int index = _ContactPoints.IndexOf (glyph);
			switch (index)
			{
				case 0: 
				{
					_From.Offset (offsetEventArgs.Offset.X, offsetEventArgs.Offset.Y);
				} break;
				case 1: 
				{
					_To.Offset (offsetEventArgs.Offset.X, offsetEventArgs.Offset.Y);
				} break;
			}
		}

		#region IGlyph Members

		protected override Rectangle GetBounds ()
		{
			return new Rectangle (_From, new Size (_To.X - _From.X, _To.Y - _From.Y));
		}

		public override void MoveTo(Point point)
		{
			// TODO:  Add TransitionGlyph.MoveTo implementation
		}

		public override void Offset(Point point)
		{
			_From.Offset (point.X, point.Y);
			_To.Offset (point.X, point.Y);
		}

		public bool IsProperInnerTransition () 
		{
			bool sameState = true;
			IStateGlyph lastState = null;
			foreach (IGlyph contact in ContactPoints)
			{
				IStateGlyph state = contact.Parent as IStateGlyph;
				if (lastState == null)
				{
					lastState = state;
				}
				if (lastState != state)
				{
					sameState = false;
					break;
				}
				lastState = state;
			}
			if (sameState)
			{
				if (IsInnerTransition)
				{
					return true;
				}
			}
			return false;
		}

		protected ArrayList GetDisplayTextAsContextList (IGraphicsContext GC)
		{
			StringCollection textList = new StringCollection ();
			DisplayText (textList);
			ArrayList contextList = new ArrayList ();
			Color innerColor = GC.Color;
			bool nextIsGuard = false;
			bool nextIsAction = false;
			foreach (string item in textList)
			{
				Color color = Color.Black;
				FontStyle fontStyle = FontStyle.Regular;
				int thickness = 10;
				if (item.StartsWith ("[") || item.StartsWith ("]") || item.StartsWith ("/") || item.StartsWith ("-"))
				{
					nextIsGuard = item.StartsWith ("[");
					nextIsAction = item.StartsWith ("/");
					color = innerColor;
					thickness = 10;
					fontStyle = FontStyle.Bold;
				}
				else if (nextIsGuard)
				{
					nextIsGuard = false;
					fontStyle = FontStyle.Italic;
				} 
				else if (nextIsAction)
				{
					nextIsAction = false;
					fontStyle = FontStyle.Regular;
				}
				contextList.Add (new DrawStringContext (item, thickness, color, fontStyle));
			}
			return contextList;
		}

		protected bool PrimaryInformation (out bool isOk, out Color primaryColor, out DashStyle dashStyle)
		{
			isOk = true;
			bool usePrimary = false;
			dashStyle = DashStyle.Solid;
			primaryColor = Color.DarkOrchid;
			if (IsNotEmptyString (EventSignal) == false)
			{
				isOk = false;
				primaryColor = Color.Red;
			}
			else if ((IsNotEmptyString (GuardCondition) || IsNotEmptyString (TimeOutExpression)) && IsNotEmptyString (Name) == false)
			{
				isOk = false;
				primaryColor = Color.Crimson;
			} 
			else if (IsProperInnerTransition ())
			{
				//usePrimary = true;
				//primaryColor = Color.CornflowerBlue;
				dashStyle = DashStyle.Dash;
			}
			else if (IsNotEmptyString (EventSource))
			{
				primaryColor = Color.Violet;
			}
			return usePrimary;
		}

		protected void DrawContactPoints (IGraphicsContext GC, bool isOk, bool usePrimary, Color primary, out Color fromColor, out Color toColor)
		{
			fromColor = primary;
			toColor = primary;
			foreach (ITransitionContactPointGlyph contact in ContactPoints)
			{
				switch (contact.WhichEnd)
				{
					case TransitionContactEnd.From: GC.Thickness = 3; break;
					case TransitionContactEnd.To: GC.Thickness = 5; break;
					default: throw new NotSupportedException ("Unknown TransitionContactEnd: " + contact.WhichEnd.ToString ());
				}
				IStateGlyph state = contact.Parent as IStateGlyph;
				if (usePrimary == false && state != null)
				{
					if (isOk)
					{
						switch (contact.WhichEnd)
						{
							case TransitionContactEnd.From: fromColor = state.StateColor; break;
							case TransitionContactEnd.To: toColor = state.StateColor; break;
							default: throw new NotSupportedException ("Unknown TransitionContactEnd: " + contact.WhichEnd.ToString ());
						}
					}
					GC.Color = state.StateColor;
				}
				else
				{
					GC.Color = primary;
				}
				if (usePrimary)
				{
					fromColor = primary;
					toColor = primary;
				}
				contact.Draw (GC);
			}
		}

		public override void Draw(IGraphicsContext GC)
		{
			using (GC.PushGraphicsState ())
			{
				bool isOk;
				bool usePrimary;
				Color primary;
				DashStyle dashStyle;
				usePrimary = PrimaryInformation (out isOk, out primary, out dashStyle);
				GC.Color = primary;
				int thickness = 2;
				if (Selected)
				{
					thickness = 5;
				}
 
				Color fromColor;
				Color toColor;
				DrawContactPoints (GC, isOk, usePrimary, primary, out fromColor, out toColor);

				GC.Thickness = thickness;
				GC.DrawLine (_From, _To, fromColor, toColor, dashStyle);

				ArrayList contextList = GetDisplayTextAsContextList (GC);
				int startX = (_To.X + _From.X) / 2;
				int startY = (_To.Y + _From.Y) / 2;
				bool isMoreHorizontal = Math.Abs (_To.X - _From.X) > Math.Abs (_To.Y - _From.Y);
				bool positiveWidthAdjust;
				bool positiveHeightAdjust;
				if (isMoreHorizontal)
				{
					positiveWidthAdjust = false;
					positiveHeightAdjust = true;
				} 
				else 
				{
					positiveWidthAdjust = false;
					positiveHeightAdjust = false;
				}
				GC.DrawString (contextList, new Point (startX, startY), positiveWidthAdjust, positiveHeightAdjust, true);

			}
		}

		bool IsValidText (string text)
		{
			if (text == null || text.Trim () == "")
			{
				return false;
			}
			return true;
		}

		protected void CompleteEventTextArray (StringCollection builder, bool includeName, bool includeTimeout)
		{
			if (includeName)
			{
				if (IsValidText (Name))
				{
					builder.Add (string.Format ("{0}", Name));
					builder.Add ("-");
				}
			}
			if (includeTimeout)
			{
				if (IsNotEmptyString (TimeOutExpression))
				{
					if (TimeOutExpression.IndexOf (" ") == -1)
					{
						builder.Add (string.Format ("after {0} raise ", TimeOutExpression));
					} 
					else 
					{
						builder.Add (string.Format ("{0} raise ", TimeOutExpression));
					}
				}
			}
			if (IsNotEmptyString (EventSource))
			{
				builder.Add (string.Format ("{0}.", EventSource));
			}
			builder.Add (string.Format ("{0}", EventSignal));
			if (IsValidText (GuardCondition))
			{
				builder.Add ("[");
				builder.Add (string.Format ("{0}", GuardCondition));
				builder.Add ("]");
			}
		}

		protected string ListToString (StringCollection list)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			foreach (string item in list)
			{
				builder.Append (item);
			}
			return builder.ToString ().Trim ();
		}

		protected void DisplayText (StringCollection builder) 
		{
			CompleteEventTextArray (builder, true, true);
			if (IsValidText (Action))
			{
				builder.Add ("/");
				builder.Add (string.Format ("{0}", Action));
			}
		}

		public string CompleteEventText (bool includeName, bool includeTimeout)
		{
			StringCollection builder = new StringCollection ();
			CompleteEventTextArray (builder, includeName, includeTimeout);
			return ListToString (builder);
		}

		public string DisplayText () 
		{
			StringCollection builder = new StringCollection ();
			DisplayText (builder);
			return ListToString (builder);
		}

		#endregion

		string _EventSignal;
		[Category ("Transition")]
		public string EventSignal { get { return _EventSignal; } set { _EventSignal = value; } }

		string _EventSource;
		[Category ("Transition")]
		public string EventSource { get { return _EventSource; } set { _EventSource = value; } }

		[Category ("Transition")]
		public string Event 
		{
			get 
			{ 
				string qual = QualifiedEvent; 
				if (qual == null) return qual;
				return qual.Replace ('.', '_'); 
			} 
		}

		public string QualifiedEvent 
		{
			get 
			{ 
				if (IsNotEmptyString (EventSource))
				{
					return string.Format ("{0}.{1}", EventSource, EventSignal); 
				}
				return EventSignal;
			} 
		}

		string _EventType = "";
		[Category ("Transition")]
		public string EventType { get { return _EventType; } set { _EventType = value; } }

		string _GuardCondition = "";
		[Category ("Transition")]
		public string GuardCondition { get { return _GuardCondition; } set { _GuardCondition = value; } }

		string _Action = "";
		[Category ("Transition")]
		public string Action { get { return _Action; } set { _Action = value; } }

		bool _IsInnerTransition;
		[Category ("Transition")]
		public bool IsInnerTransition { get { return _IsInnerTransition; } set { _IsInnerTransition = value; } }

		TransitionType _TransitionType = TransitionType.Normal;
		[Category ("Transition")]
		public TransitionType TransitionType { get { return _TransitionType; } set { _TransitionType = value; } }

		int _EvaluationOrderPriority = 0;
		[Category ("Transition")]
		public int EvaluationOrderPriority { get { return _EvaluationOrderPriority; } set { _EvaluationOrderPriority = value; } }

		string _TimeOutExpression;
		[Category ("Transition")]
		public string TimeOutExpression { get { return _TimeOutExpression; } set { _TimeOutExpression = value; } }
	}
}
