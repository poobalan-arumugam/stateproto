using System;

namespace qf4net
{
	/// <summary>
	/// LQHsmHelper.
	/// </summary>
	public class LQHsmHelper
	{
        public static void FillMementoWithStateName (ILQHsmMemento memento, Type hsmType, string currentStateName, object arg)
        {
            System.Reflection.MethodInfo methodInfo = hsmType.GetMethod ("S_" + currentStateName);
            if (methodInfo == null)
            {
                string msg = string.Format ("State name [{0}] not found in Hsm {1}/{2}", currentStateName, hsmType, arg);
                throw new InvalidOperationException (msg);
            }
            memento.CurrentStateMethod = methodInfo;
        }

        public static void FillMementoWithStateName (ILQHsmMemento memento, ILQHsm hsm, string currentStateName)
        {
            FillMementoWithStateName (memento, hsm.GetType (), currentStateName, hsm);
        }
    }
}
