using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for SimpleTransactionalCmd.
	/// </summary>
	public class SimpleTransactionalCmd : IQSimpleCommand
	{
		public SimpleTransactionalCmd(IQSimpleCommand cmd)
		{
            _Cmd = cmd;
            _Transaction = QEvent.GetThreadTransaction ();
		}

        private IQSimpleCommand _Cmd;
        private IQFTransaction _Transaction;

	    public void Execute()
	    {
            try 
            {
                _Cmd.Execute ();
                if (null != _Transaction)
                {
                    _Transaction.Commit ();
                }
            } 
            catch
            {
                if (null != _Transaction)
                {
                    _Transaction.Abort ();
                }
                throw;
            }
	    }
	}
}
