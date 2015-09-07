using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for IQEventEditContext.
	/// </summary>
	public interface IQEventEditContext
	{
		System.ComponentModel.IComponent Container { get; }
		object Instance { get; set; }

		bool Edit ();
	}
}
