using System;
using NLog;

namespace Cravens.Utilities.Logging 
{
	public class NLogLogger : ILogger
	{
		private readonly Logger _logger;

		public NLogLogger()
		{
			_logger = LogManager.GetCurrentClassLogger();
		}

		public void Info(string message)
		{
			_logger.Info(message);
		}

		public void Warn(string message)
		{
			_logger.Warn(message);
		}

		public void Debug(string message)
		{
			_logger.Debug(message);
		}

		public void Error(string message)
		{
			_logger.Error(message);
		}

		public void Error(string message, Exception exception)
		{
			_logger.ErrorException(message, exception);
		}
	}
}
