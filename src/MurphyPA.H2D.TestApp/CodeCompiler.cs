using System;
using System.Collections;
using System.CodeDom.Compiler;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for CodeCompiler.
	/// </summary>
	public class CodeCompiler
	{
		public CompilerResults Compile (DiagramModel model)
		{
			ConvertToCode convert = new ConvertToCode (model, false);
			string code = convert.Convert ();
			Type loggingUtilType = typeof (LoggingUserBase);
			Type qf4netType = typeof (qf4net.QHsm);
			Type qfExtensionsType = typeof (qf4net.LQHsm);
			CompilerResults results = Compile (code, new Type[] {loggingUtilType, qf4netType, qfExtensionsType});
			return results;
		}

		protected virtual CompilerResults Compile (string code, Type[] types) 
		{
			Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider ();
			ICodeCompiler compiler = provider.CreateCompiler ();
			ArrayList assemblies = new ArrayList ();
			assemblies.Add ("System.dll");
			foreach (Type type in types)
			{
				assemblies.Add (type.Assembly.Location);
			}
			string[] assemblyNames = (string[]) assemblies.ToArray (typeof (string));
			CompilerParameters options = new CompilerParameters (assemblyNames);
			options.GenerateInMemory = true;
			CompilerResults results = compiler.CompileAssemblyFromSource (options, code);
			return results;
		}

	}
}
