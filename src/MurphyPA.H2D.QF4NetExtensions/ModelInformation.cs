using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for ModelInformation.
	/// </summary>
	[Serializable]
	public class ModelInformation
	{
		string _FileName;
		public string FileName { get { return _FileName; } }

		string _Guid;
		public string Guid { get { return _Guid; } }

		string _ModelVersion;
		public string ModelVersion { get { return _ModelVersion; } }

		public ModelInformation(string fileName, string guid, string modelVersion)
		{
			_FileName = fileName;
			_Guid = guid;
			_ModelVersion = modelVersion;
		}
	}
}
