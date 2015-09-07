// -----------------------------------------------------------------------------
//                            qf4net Library
//
// Port of Samek's Quantum Framework to C#. The implementation takes the liberty
// to depart from Miro Samek's code where the specifics of desktop systems 
// (compared to embedded systems) seem to warrant a different approach.
// Please see accompanying documentation for details.
// 
// Reference:
// Practical Statecharts in C/C++; Quantum Programming for Embedded Systems
// Author: Miro Samek, Ph.D.
// http://www.quantum-leaps.com/book.htm
//
// -----------------------------------------------------------------------------
//
// Copyright (C) 2003-2004, The qf4net Team
// All rights reserved
// Lead: Rainer Hessmer, Ph.D. (rainer@hessmer.org)
// 
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions
//   are met:
//
//     - Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer. 
//
//     - Neither the name of the qf4net-Team, nor the names of its contributors
//        may be used to endorse or promote products derived from this
//        software without specific prior written permission. 
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
//   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
//   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
//   OF THE POSSIBILITY OF SUCH DAMAGE.
// -----------------------------------------------------------------------------


using System;
	
namespace qf4net
{
	/// <summary>
	///  
	/// </summary>
	[Serializable]
	public class QEvent : IQEvent
	{
        private const string QActivityIdSlotName = "QF4Net.QEvents.QActivityId";
        private const string QTransactionSlotName = "QF4Net.QEvents.QTransaction";

        private string m_QSignal;
		private object m_QData;
		private DateTime m_QSent;
        private string m_QKey;
        private string m_QActivityId;
        private IQFTransaction m_QTransaction;

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public QEvent(string qSignal)
		{
			Init (null, qSignal);
		}

		public QEvent(string qSignal, object qData)
		{
			Init (null, qSignal);
			m_QData = qData;
		}

		public QEvent (string qSource, string qSignal, object qData)
		{
			Init (qSource, qSignal);
			m_QData = qData;
		}

		public QEvent (string qSource, string qSignal, object qData, DateTime qSent)
		{
			Init (qSource, qSignal);
			m_QData = qData;
			m_QSent = qSent;
		}

        public QEvent (string qSource, string qKey, string qSignal, object qData)
        {
            Init (qSource, qKey, qSignal);
            m_QData = qData;            
        }

        public QEvent (string qSource, string qKey, string qSignal, object qData, DateTime qSent)
        {
            Init (qSource, qKey, qSignal);
            m_QData = qData;
            m_QSent = qSent;
        }

		public QEvent (System.Runtime.Remoting.Messaging.IMethodCallMessage msg)
		{
			Init (null, msg.MethodName);
			m_QData = msg;
		}

		public QEvent (System.Runtime.Remoting.Messaging.IMethodReturnMessage ret)
		{
			Init (null, ret.MethodName);
			m_QData = ret;
		}

        protected void Init (string qSource, string qSignal)
        {
            Init (qSource, null, qSignal);
        }

		protected void Init (string qSource, string qKey, string qSignal)
		{
			if (qSource != null)
			{
				m_QSignal = qSource + "." + qSignal;
			} 
			else 
			{
				m_QSignal = qSignal;
			}

            m_QKey = qKey;

			m_QSent = DateTime.Now;

            LoadActivityId ();
            LoadPendingTransaction ();
		}

        public static string GetActivityId ()
        {
            LocalDataStoreSlot slot = System.Threading.Thread.GetNamedDataSlot (QActivityIdSlotName);
            object possibleCardinalValue = System.Threading.Thread.GetData (slot);
            if (possibleCardinalValue == null)
            {
                return null;
            }
            return possibleCardinalValue.ToString ();
        }

        public static IQFTransaction GetThreadTransaction ()
        {
            LocalDataStoreSlot slot = System.Threading.Thread.GetNamedDataSlot (QTransactionSlotName);
            object possibleTransaction = System.Threading.Thread.GetData (slot);
            if (possibleTransaction == null)
            {
                return null;
            }
            return possibleTransaction as IQFTransaction;
        }
      
        public static void SetThreadTransaction (IQFTransaction transaction)
        {
            LocalDataStoreSlot slot = System.Threading.Thread.GetNamedDataSlot (QTransactionSlotName);
            System.Threading.Thread.SetData (slot, transaction);            
        }

        private void LoadPendingTransaction ()
        {
            m_QTransaction = GetThreadTransaction ();
        }

        private void LoadActivityId ()
        {
            string possibleCardinalValue = GetActivityId ();
            if (possibleCardinalValue == null)
            {
                m_QActivityId = Guid.NewGuid ().ToString ();
            } 
            else
            {
                m_QActivityId = possibleCardinalValue.ToString ();
            }            
        }

        public void ApplyActivityId ()
        {
            LocalDataStoreSlot slot = System.Threading.Thread.GetNamedDataSlot (QActivityIdSlotName);
            System.Threading.Thread.SetData (slot, m_QActivityId);            
        }

        public void ClearActivityId ()
        {
            LocalDataStoreSlot slot = System.Threading.Thread.GetNamedDataSlot (QActivityIdSlotName);
            System.Threading.Thread.SetData (slot, null);                        
        }

	    public void Commit()
	    {
            if (null != m_QTransaction)
            {
                m_QTransaction.Commit ();
            }
	    }

	    public void Abort()
	    {
            if (null != m_QTransaction)
            {
                m_QTransaction.Abort ();
            }
	    }

	    /// <summary>
		/// The identifier of the <see cref="QEvent"/> type.
		/// </summary>
		public string QSignal
		{
			get { return m_QSignal; }
		}

		public object QData { 
			get { return m_QData; }
		}

		public DateTime QSent 
		{ 
			get 
			{
				return m_QSent;
			}
		}

        public string QKey 
        {
            get 
            {
                return m_QKey;
            }
        }

        public string QActivityId
        {
            get
            {
                return m_QActivityId;
            }
        }


		/// <summary>
		/// The QSignal in string form. It allows for simpler debugging and logging. 
		/// </summary>
		/// <returns>The signal as string.</returns>
		public override string ToString()
		{
			switch (QSignal)
			{
				case QSignals.Init:    return "Init";
				case QSignals.Entry:   return "Entry";
				case QSignals.Exit:    return "Exit";
				default: return QSignal.ToString();
			}
		}
	}
}
