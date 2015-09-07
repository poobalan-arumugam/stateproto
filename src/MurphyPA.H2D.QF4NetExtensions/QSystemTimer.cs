using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for QSystemTimer.
	/// </summary>
	public class QSystemTimer : IQTimer
	{
		public QSystemTimer (){}
		public QSystemTimer (int timingScale)
		{
			TimingScale = timingScale;
		}

		#region IQTimer Members

		private int _TimingScale = 1;
		protected int TimingScale 
		{
			get { return _TimingScale; }
			set 
			{
				System.Diagnostics.Debug.Assert (value > 0);
				if (value <= 0)
				{
					throw new ArgumentException ("Value must be greater than zero", "TimingScale");
				}
				_TimingScale = value;
			}
		}

		protected System.Collections.Hashtable _Timers = new System.Collections.Hashtable ();

		protected class TimeOutCmd : IDisposable
		{
			string _Name;
			public string Name { get { return _Name; } }

			System.Timers.Timer _Timer;
			QSystemTimer _QTimer;

			IQHsm _Hsm;
			public IQHsm Hsm { get { return _Hsm; } }

			IQEvent _Event;
			public IQEvent Event { get { return _Event; } }

			bool _Expired;
			public bool Expired { get { return _Expired; } }

			TimeOutType _TimeOutType;
			public TimeOutType TimeOutType { get { return _TimeOutType; } }

            System.Security.Principal.IPrincipal _Principal;

			DateTime _Time = DateTime.MinValue;

			public TimeOutCmd (QSystemTimer qTimer, string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
			{
				Init (qTimer, name, duration, hsm, ev, timeOutType);
			}

			public TimeOutCmd (QSystemTimer qTimer, string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
			{
				TimeSpan duration = new TimeSpan (0);
				_Time = at;
				DateTime now = DateTime.Now;
				if (at > now)
				{
					duration = at - now;
				}
				Init (qTimer, name, duration, hsm, ev, timeOutType);
			}

			protected void Init (QSystemTimer qTimer, string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
			{
				_QTimer = qTimer;
				_Name = name;
				_Hsm = hsm;
				_Event = ev;
				_TimeOutType = timeOutType;
				double ms = duration.TotalMilliseconds;
				double msInterval = ms > 0 ? ms : TimeSpan.MaxValue.TotalMilliseconds;
				_Timer = new System.Timers.Timer (msInterval);
				_Timer.Elapsed += new System.Timers.ElapsedEventHandler(_Timer_Elapsed);
				_Timer.Enabled = ms > 0;

                _Principal = System.Threading.Thread.CurrentPrincipal;
			}

			private void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
			{
				switch (_TimeOutType)
				{
					case TimeOutType.Single:
					{
						_Expired = true;
						_Timer.Enabled = false;
					} break;
				}
                System.Threading.Thread.CurrentPrincipal = _Principal;
				_QTimer.DoTimeOut (_Hsm, _Event);
			}

			#region IDisposable Members

			public void Dispose()
			{
				_Timer.Enabled = false;
				_Timer.Dispose ();
			}

			#endregion
		}


		protected virtual TimeOutCmd CreateTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			if (_TimingScale != 1)
			{
				duration = new TimeSpan (duration.Ticks * _TimingScale);
			}
			return new TimeOutCmd (this, name, duration, hsm, ev, timeOutType);
		}

		protected virtual TimeOutCmd CreateTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			return new TimeOutCmd (this, name, at, hsm, ev, timeOutType);
		}

		protected TimeOutCmd GetTimeOut (string name)
		{
			if (_Timers.Contains (name))
			{
				return (TimeOutCmd)_Timers [name];
			}
			return null;
		}

		public void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev)
		{
			SetTimeOut (name, duration, hsm, ev, TimeOutType.Single);
		}

		protected void SetTimeOut (TimeOutCmd timeOut)
		{
			string name = timeOut.Name;
			TimeOutCmd oldTimeOut = GetTimeOut (name);
			if (oldTimeOut != null)
			{
				// only allow replace of timeout if for same hsm.
				System.Diagnostics.Debug.Assert (oldTimeOut.Hsm == timeOut.Hsm);
				oldTimeOut.Dispose ();
				_Timers.Remove (name);
			}
				
			_Timers.Add (name, timeOut);			
		}

		public void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			TimeOutCmd timeOut = CreateTimeOut (name, duration, hsm, ev, timeOutType);
			SetTimeOut (timeOut);
		}

		public void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev)
		{
			SetTimeOut (name, at, hsm, ev, TimeOutType.Single);
		}

		public void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			TimeOutCmd timeOut = CreateTimeOut (name, at, hsm, ev, timeOutType);
			SetTimeOut (timeOut);
		}

		public void ClearTimeOut (string name)
		{
			TimeOutCmd timeOut = GetTimeOut (name);
			if (timeOut != null)
			{
				timeOut.Dispose ();
				_Timers.Remove (timeOut);
			}
		}

		protected virtual bool OnTimeOut (IQHsm hsm, IQEvent ev)
		{
			return true;
		}

		public event QTimeoutHandler TimeOut;

		internal void DoTimeOut (IQHsm hsm, IQEvent ev)
		{
			if (OnTimeOut (hsm, ev))
			{
				if (TimeOut != null)
				{
					TimeOut (this, hsm, ev);
				}
			}
		}

		#endregion
	}
}
