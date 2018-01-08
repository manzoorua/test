using System;
using System.Diagnostics;

namespace Cravens.Utilities.Logging 
{
	public class EventLogLogger : ILogger
	{
		private readonly string _applicationName;

		public EventLogLogger(string applicationName)
		{
			_applicationName = applicationName;	
		}

		#region ILogger Members

		public void Info(string message)
		{
			WriteToEventLog(message, EventLogEntryType.Information);
		}

		public void Warn(string message)
		{
			WriteToEventLog(message, EventLogEntryType.Warning);
		}

		public void Debug(string message)
		{
			WriteToEventLog(message, EventLogEntryType.Information);
		}

		public void Error(string message)
		{
			WriteToEventLog(message, EventLogEntryType.Error);
		}

		public void Error(string message, Exception exception)
		{
			Error(message + " ---> " + exception);
		}
		#endregion

		private void WriteToEventLog(string message, EventLogEntryType logType)
		{
			EventLog eventLog = new EventLog {Source = _applicationName};
			eventLog.WriteEntry(message, logType);
		}
	}
}
