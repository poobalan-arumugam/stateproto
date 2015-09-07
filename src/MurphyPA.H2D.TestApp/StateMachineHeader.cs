using System;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for StateMachineHeader.
	/// </summary>
	public class StateMachineHeader
	{
		string _ImplementationVersion = "0.2";
		public string ImplementationVersion 
		{
			get { return _ImplementationVersion; }
		}

		string _ModelGuid = Guid.NewGuid ().ToString ();
		public string ModelGuid 
		{
			get { return _ModelGuid; }
			set { _ModelGuid = value; }
		}

		string _ModelFileName;
		public string ModelFileName 
		{
			get { return _ModelFileName; }
			set { _ModelFileName = value; }
		}

		bool _HasSubMachines;
		public bool HasSubMachines 
		{
			get { return _HasSubMachines; }
			set { _HasSubMachines = value; }
		}

		int _StateMachineVersion = 0;
		public int StateMachineVersion 
		{
			get { return _StateMachineVersion; }
			set { _StateMachineVersion = value; }
		}

		string _BaseStateMachine;
		public string BaseStateMachine 
		{
			get { return _BaseStateMachine; }
			set { _BaseStateMachine = value; }
		}

		string _Name;
		public string Name 
		{ 
			get { return _Name; }
			set { _Name = value; }
		}

		string _NameSpace = "MurphyPA.SM";
		public string NameSpace 
		{
			get { return _NameSpace; }
			set { _NameSpace = value; }
		}

		string _UsingNameSpaces = "";
		public string UsingNameSpaces 
		{
			get { return _UsingNameSpaces; }
			set { _UsingNameSpaces = value; }
		}

		string _Comment = "";
		public string Comment 
		{
			get { return _Comment; }
			set { _Comment = value; }
		}

		string _Fields = "";
		public string Fields 
		{
			get { return _Fields; }
			set { _Fields = value; }
		}

		bool _ReadOnly;
		public bool ReadOnly 
		{ 
			get { return _ReadOnly; }
			set { _ReadOnly = value; }
		}

		string _Assembly;
		public string Assembly 
		{ 
			get { return _Assembly; }
			set { _Assembly = value; }
		}
	}
}
