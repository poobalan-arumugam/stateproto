using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for ITransitionGlyph.
	/// </summary>
	public interface ITransitionGlyph : IGroupGlyph
	{
		string EventSignal { get; set; }
		string EventSource { get; set; }
		string Event { get; }
		string QualifiedEvent { get; }

		string EventType { get; set; }
		string GuardCondition { get; set; }
		string Action { get; set; }

		int EvaluationOrderPriority { get; set; }
		bool IsInnerTransition { get; set; }
		bool IsProperInnerTransition ();

		TransitionType TransitionType { get; set; }

		string TimeOutExpression { get; set; }

		string DisplayText ();
		string CompleteEventText (bool includeName, bool includeTimeout);
	}
}
