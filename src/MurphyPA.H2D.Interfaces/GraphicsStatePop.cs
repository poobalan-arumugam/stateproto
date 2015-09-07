using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for GraphicsStatePop.
	/// </summary>
	public class GraphicsStatePop : IDisposable
	{
		IGraphicsContext _Context;

		public GraphicsStatePop (IGraphicsContext context)
		{
			_Context = context;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_Context.PopGraphicsState ();
		}

		#endregion
	}
}
