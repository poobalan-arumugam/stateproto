using System;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// Summary description for ILQHsmDictionary.
	/// </summary>
	public class ILQHsmDictionary : DictionaryBase
	{
		public ILQHsm this [string name]
		{
			get 
			{
				return (ILQHsm) InnerHashtable [name];
			}
		}

		public void Add (string name, ILQHsm hsm)
		{
			InnerHashtable.Add (name, hsm);
		}
	}
}
