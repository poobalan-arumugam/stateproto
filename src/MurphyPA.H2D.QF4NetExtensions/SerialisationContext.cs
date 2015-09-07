using System;
using System.IO;
using System.Runtime.Serialization;

namespace qf4net
{
	/// <summary>
	/// SerialisationContext.
	/// </summary>
	public class SerialisationContext : ISerialisationContext
	{
		public SerialisationContext(IFormatter formatter, Stream stream)
		{
			_Formatter = formatter;
			_Stream = stream;
		}

		#region ISerialisationContext Members

		IFormatter _Formatter;
		public IFormatter Formatter
		{
			get
			{
				return _Formatter;
			}
		}

		Stream _Stream;
		public Stream Stream
		{
			get
			{
				return _Stream;
			}
		}

		#endregion
	}
}
