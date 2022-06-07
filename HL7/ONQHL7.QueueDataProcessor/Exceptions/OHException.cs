using System;

namespace ONQHL7.QueueDataProcessor.Exceptions
{
	public sealed class OHException : Exception
	{
		public enum OHExceptionType
		{
			Error = 0,
			Info = 1,
			Warn = 2,
			None = 3
		}
		private OHExceptionType m_eExceptionType = OHExceptionType.Error;
		public OHException()
		{
		}

		public OHException(string a_sMessage)
			: base(a_sMessage)
		{
		}

		public OHException(string a_sMessage, Exception a_mInner)
			: base(a_sMessage, a_mInner)
		{
		}
		public OHException(OHExceptionType a_eExceptionType)
		{
			m_eExceptionType = a_eExceptionType;
		}

		public OHException(OHExceptionType a_eExceptionType, string a_sMessage)
			: base(a_sMessage)
		{
			m_eExceptionType = a_eExceptionType;
		}

		public OHException(OHExceptionType a_eExceptionType, string a_sMessage, Exception a_mInner)
			: base(a_sMessage, a_mInner)
		{
			m_eExceptionType = a_eExceptionType;
		}
	}
}
