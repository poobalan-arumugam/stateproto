using System;

namespace qf4net
{
    /// <summary>
    /// QEventManager.
    /// </summary>
    public abstract class QEventManagerBase : QEventManagerEventsBase, IQEventManager
    {
        public QEventManagerBase (IQTimer timer)
        {
            _Timer = timer;
            _Timer.TimeOut += new QTimeoutHandler(_Timer_TimeOut);
        }

        #region Message Queue
        object _QLock = new Object ();
        System.Threading.AutoResetEvent _WaitHandle = new System.Threading.AutoResetEvent (false);
        System.Collections.Stack _FrontStack = new System.Collections.Stack ();
        System.Collections.Queue _BottomQueue = new System.Collections.Queue ();

        public void Dispatch (IQHsm hsm, IQEvent ev)
        {
            AsyncDispatch (hsm, ev);
            Poll ();
        }

        protected virtual void InternalDispatch (IQHsm hsm, IQEvent ev)
        {
            bool ok = hsm.DispatchWithExceptionTrap (ev, false);
            if (ok)
            {
                ev.Commit ();
            } else
            {
                ev.Abort ();
            }
        }

        public void AsyncDispatch (IQHsm hsm, IQEvent ev)
        {
            HsmEventHolder holder = new HsmEventHolder (this, hsm, ev);
            AsyncDispatch (holder);
        }

        public void AsyncDispatchFront (IQHsm hsm, IQEvent ev)
        {
            lock (_QLock)
            {
#warning Using a stack will alway push the most recent event to the front - thus reversing instead of maintaining "insert" order
                HsmEventHolder holder = new HsmEventHolder (this, hsm, ev);
                _FrontStack.Push (holder);
            }
            _WaitHandle.Set ();
        }

        public void AsyncDispatch (IQSimpleCommand cmd)
        {
            lock (_QLock)
            {
                if (!(cmd is HsmEventHolder))
                {
                    if (!(cmd is SimpleTransactionalCmd))
                    {
                        cmd = new SimpleTransactionalCmd (cmd);
                    }
                }
                _BottomQueue.Enqueue (cmd);
            }      
            _WaitHandle.Set ();
        }

        public bool PollOne ()
        {
            front:
                int count;
            object queueEntry = null;

            lock (_QLock)
            {
                count = _FrontStack.Count;
                if (count > 0)
                {
                    queueEntry = _FrontStack.Pop ();
                } else
                {
                    count = _BottomQueue.Count;
                    if (count > 0)
                    {
                        queueEntry = _BottomQueue.Dequeue ();                        
                    }                    
                }
            }

            if (count > 0)
            {
                if (queueEntry == null)
                {
                    Logger.Error ("---- QueueEntry is null! ----");
                    goto front;
                }

                IQSimpleCommand command = queueEntry as IQSimpleCommand;
                if (command == null)
                {
                    Logger.Error ("---- Command is null! ----");
                    goto front;
                }

                Execute (command);
                return true;
            }
            return false;
        }

        internal void DispatchFromEventHolder (HsmEventHolder holder)
        {
            System.Diagnostics.Debug.Assert (holder != null);
            try 
            {
                System.Security.Principal.IPrincipal previousPrincipal = System.Threading.Thread.CurrentPrincipal;
                System.Threading.Thread.CurrentPrincipal = holder.Principal;

                DoPolledEvent (this, holder, PollContext.BeforeHandled);
                InternalDispatch (holder.Hsm, holder.Event);
                DoPolledEvent (this, holder, PollContext.AfterHandled);

                System.Threading.Thread.CurrentPrincipal = previousPrincipal;
            } 
            catch (Exception ex)
            {
                DoEventManagerDispatchException (this, ex, holder.Hsm, holder.Event);
            }            
        }

        protected void Execute (IQSimpleCommand cmd)
        {
            try 
            {
                cmd.Execute ();
            } 
            catch (Exception ex)
            {
                DoEventManagerDispatchCommandException (this, ex, cmd);                
            }
        }

        IQEventManagerRunner _Runner;
        public IQEventManagerRunner Runner 
        {
            get { return _Runner; } 
            set { _Runner = value; }
        }

		public bool Poll ()
		{
			bool ok = false;
			while (PollOne ())
			{
				ok = true;
			}
			return ok;
		}

		public bool WaitOne (TimeSpan duration)
		{
			return _WaitHandle.WaitOne (duration, true);
		}

		protected override bool OnEventManagerDispatchException (IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
		{
            /* -- we're not ready for this type of thing yet. EventManagerDispatchException is an exception experienced by the eventManager
             * -- while trying to dispatch an event to an hsm.
             
			DispatchExceptionFailureEventArgs args = new DispatchExceptionFailureEventArgs (eventManager, ex, hsm, ev);
			AsyncDispatchFront (hsm, new QEvent ("Failure", args));
            
            */
			return base.OnEventManagerDispatchException (eventManager, ex, hsm, ev);
		}

		#endregion

		#region TimeOuts

		IQTimer _Timer;

		public void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev)
		{
			_Timer.SetTimeOut (name, duration, hsm, ev);
		}

		public void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			_Timer.SetTimeOut (name, duration, hsm, ev, timeOutType);
		}

		public void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev)
		{
			_Timer.SetTimeOut (name, at, hsm, ev);
		}

		public void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType)
		{
			_Timer.SetTimeOut (name, at, hsm, ev, timeOutType);
		}

		public void ClearTimeOut (string name)
		{
			_Timer.ClearTimeOut (name);
		}

		private void _Timer_TimeOut(IQTimer timer, IQHsm hsm, IQEvent ev)
		{
			AsyncDispatchFront (hsm, ev);
		}
		#endregion
    }
}
