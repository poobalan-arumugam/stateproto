using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for ModelInformationAttribute.
	/// </summary>
	[AttributeUsage (AttributeTargets.Class)]
	public class ModelInformationAttribute : Attribute
	{
		ModelInformation _ModelInformation;
		public ModelInformation ModelInformation 
		{
			get 
			{ 
				return _ModelInformation;
			}
		}

		public ModelInformationAttribute(string fileName, string guid, string modelVersion)
		{
			_ModelInformation = new ModelInformation (fileName, guid, modelVersion);
		}
	}
}
