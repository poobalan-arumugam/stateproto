using System;

namespace qf4net
{
    /// <summary>
    /// HsmEventHolder.
    /// </summary>
    public class HsmEventHolder : IQSimpleCommand
    {
        public HsmEventHolder (QEventManagerBase eventManager, IQHsm hsm, IQEvent ev)
        {            
            _EventManager = eventManager;
            _Hsm = hsm;
            _Event = ev;
            _Principal = System.Threading.Thread.CurrentPrincipal;
        }

        QEventManagerBase _EventManager;

        IQHsm _Hsm;
        public IQHsm Hsm { get { return _Hsm; } }

        IQEvent _Event;
        public IQEvent Event { get { return _Event; } }

        System.Security.Principal.IPrincipal _Principal;
        public System.Security.Principal.IPrincipal Principal { get { return _Principal; } }

        public void Execute()
        {
#warning Going this route made calling to a command simpler in the QEventManagerBase
#warning but has the overhead of this class instance having to hold an _EventManager field
#warning and the overhead of the extra calls (cmd.Execute () calls back to EventManager.DispatchFromEventHolder ()).

            _EventManager.DispatchFromEventHolder (this);
        }
    }
}
