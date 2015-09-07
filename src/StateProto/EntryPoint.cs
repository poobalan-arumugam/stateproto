using System;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for EntryPoint.
	/// </summary>
	public class EntryPoint
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.Run(new TestAppForm ());
		}

		private static void HandleThreadException(object sender, string caption, Exception ex)
		{
			try 
			{
				ThreadExceptionDialog dlg = new ThreadExceptionDialog (ex);
				dlg.Text = caption;
				if (dlg.ShowDialog () == DialogResult.Abort)
				{
					Application.Exit ();
				}
			} 
			catch {}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			HandleThreadException (sender, "AppDomain Unhandled Exception", ex);
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			HandleThreadException (sender, "Application Thread Unhandled Exception", e.Exception);
		}
	}
}
