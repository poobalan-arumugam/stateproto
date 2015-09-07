using System;
using MurphyPA.H2D.Interfaces;
using MurphyPA.H2D.TestApp;
using qf4net;

namespace MurphyPA.H2D.StateInteraction
{
	/// <summary>
	/// Summary description for StateProtoViewAnimator.
	/// </summary>
	public class StateProtoViewAnimator
	{
        #region TransitionDelay
        private int _TransitionDelay = 0;
        public int TransitionDelay 
        {
            get 
            {
                return _TransitionDelay; 
            } 
            set 
            {
                _TransitionDelay = value; 
            }
        }
        #endregion
        
	    ILQHsm _Hsm;
	    StateDiagramView _View;
        HsmStateChangeHander _Hsm_StateChange_RunInViewThread_Handler;
	    
	    public StateProtoViewAnimator(ILQHsm hsm, StateDiagramView view)
		{
		    _Hsm = hsm;
		    _View = view;
		    _Hsm_StateChange_RunInViewThread_Handler = new HsmStateChangeHander(_Hsm_StateChange_RunInViewThread);
		        
		    RegisterEvents ();
		}

        private void RegisterEvents ()
        {
            _Hsm.StateChange += new EventHandler(_Hsm_StateChange);
        }
	    
        protected void ClearCurrentStateView ()
        {
            foreach (IGlyph glyph in _View.StateControl.Model.Glyphs)
            {
                glyph.Selected = false;
            }
            _View.StateControl.RefreshView ();
        }
	    
        private string GetStateName(IStateGlyph stateGlyph)
        {
            string sname = "S_" + stateGlyph.FullyQualifiedStateName;
            sname = sname.Replace (".", "_");
            return sname;
        }
	    
        protected void RefreshCurrentStateView (string qhsmName, string currentTransitionName)
        {
            ClearCurrentStateView ();
            
            foreach (IGlyph glyph in _View.StateControl.Model.Glyphs)
            {
                if(currentTransitionName == null)
                {
                    IStateGlyph stateGlyph = glyph as IStateGlyph;
                    if (stateGlyph != null)
                    {
                        if (qhsmName == GetStateName(stateGlyph))
                        {
                            stateGlyph.Selected = true;
                        }                    
                    }
                } 
                else
                {
                    ITransitionGlyph trans = glyph as ITransitionGlyph;
                    if(trans != null)
                    {
                        string sx = trans.FullyQualifiedStateName;
                        foreach(IGlyph owned in trans.OwnedItems)
                        {
                            IStateGlyph sg = owned.Parent as IStateGlyph;
                            if(sg != null)
                            {
                                if(GetStateName(sg) == qhsmName)
                                {
                                    string transName = trans.CompleteEventText (true, true);
                                    if(trans.Action != "")
                                    {
                                        transName = transName + "/" + trans.Action;
                                    }
                                    if(transName == currentTransitionName)
                                    {
                                        trans.Selected = true;
                                        _View.StateControl.RefreshView ();
                                        if(_TransitionDelay > 0)
                                        {
                                            System.Threading.Thread.Sleep (TimeSpan.FromMilliseconds (_TransitionDelay));
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            _View.StateControl.RefreshView ();
        }	   	    
	    
	    public delegate void HsmStateChangeHander (ILQHsm hsm, LogStateEventArgs ea);

        private void _Hsm_StateChange(object sender, EventArgs e)
        {
            ILQHsm hsm = (ILQHsm)sender;
            LogStateEventArgs ea = (LogStateEventArgs)e;
            _Hsm_StateChange_RunInViewThread(hsm, ea);
        }
	    
        private void _Hsm_StateChange_RunInViewThread(ILQHsm hsm, LogStateEventArgs ea)
        {            
            if(_View.InvokeRequired)
            {                
                object[] args = new object[] {hsm, ea};
                _View.Invoke (_Hsm_StateChange_RunInViewThread_Handler, args);
            } 
            else
            {
                _Hsm_StateChange_Internal (hsm, ea);
            }
        }
	    
        private void _Hsm_StateChange_Internal(ILQHsm hsm, LogStateEventArgs ea)
        {
            string currentStateName = null;
            string currentTransitionName = null;
            switch (ea.LogType)
            {
                case StateLogType.EventTransition:
                {
                    currentStateName = ea.State.Method.Name;
                    currentTransitionName = ea.EventDescription;
                }
                    break;
                case StateLogType.Init: 
                {
                    currentStateName = ea.NextState.Method.Name;
                } break;
                case StateLogType.Restored:
                case StateLogType.Entry:
                case StateLogType.Final:
                {
                    currentStateName = ea.State.Method.Name;
                } break;
            }

            if(currentStateName != null)
            {
                RefreshCurrentStateView (currentStateName, currentTransitionName);
            }
        }
	    
	}
}
