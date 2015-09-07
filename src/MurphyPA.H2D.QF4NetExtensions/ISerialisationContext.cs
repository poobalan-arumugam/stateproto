using System;
using System.IO;
using System.Runtime.Serialization;

namespace qf4net
{
	/// <summary>
	/// Summary description for ISerialisationContext.
	/// </summary>
	public interface ISerialisationContext
	{
		IFormatter Formatter { get; } 
		Stream Stream { get; }
	}
}
