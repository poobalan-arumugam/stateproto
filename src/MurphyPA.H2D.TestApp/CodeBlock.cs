using System;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for CodeBlock.
	/// </summary>
	public class CodeBlock
	{
		public CodeBlock(string codeBlock)
		{
			_Value = codeBlock;
		}

		string _Value;
		public string Value { get { return _Value; } }

		int _UsageCount;
		public int UsageCount { get { return _UsageCount; } }

		public void Use () 
		{
			_UsageCount ++;
		}
	}
}
