using System;

namespace Rlc.Utilities.Log
{
	/// <summary>
	/// Writes info to the debug window.
	/// </summary>
	public class DebugLogger : ILogger
	{
		#region ILogger Members

		public void Debug(string message)
		{
			string msg = "DEBUG\t" + message;
			WriteMessage(msg);
		}

		public void Error(string message, Exception e)
		{
			string msg = "ERROR\t" + message + e;
			WriteMessage(msg);
		}

		public void Error(string message)
		{
			string msg = "ERROR\t" + message;
			WriteMessage(msg);
		}

		public void Info(string message)
		{
			string msg = "INFO\t" + message;
			WriteMessage(msg);
		}

		public void Warn(string message)
		{
			string msg = "WARN\t" + message;
			WriteMessage(msg);
		}

		#endregion

		private static void WriteMessage(string message)
		{
			System.Diagnostics.Debug.Write(DateTime.Now + "\t" + message);
		}
	}
}