using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for IQEventEditor.
	/// </summary>
	public interface IQEventEditor
	{
		bool Edit (IQEventEditContext context);
		bool SupportsParse { get; }
		object Parse (string value);
	}
}
