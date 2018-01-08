using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitFactory.Logging;
using System.IO;

namespace Rlc.Utilities.Log
{
	public static class Logger
	{
		// Create an instance of the logging framework.
		//
		private static readonly CompositeLogger _logger = new CompositeLogger();
		private static CompositeLogger Instance
		{
			get { return _logger; }
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		static Logger()
		{
			// Create a global formatter for use by all loggers.
			//
			Instance.Formatter = new TabSeperatedLogEntryFormatter();
		}

		/// <summary>
		/// Adds the rolling file logger.
		/// </summary>
		/// <param name="logFolder">The log folder.</param>
		public static void AddRollingFileLogger(string logFolder)
		{
			const string applicationName = "Unknown";
			AddRollingFileLogger(logFolder, applicationName);
		}

		/// <summary>
		/// Adds the rolling file logger.
		/// </summary>
		/// <param name="logFolder">The log folder.</param>
		/// <param name="applicationName">Name of the application.</param>
		public static void AddRollingFileLogger(string logFolder, string applicationName)
		{
			const string dateTemplate = "yyyyMM";
			const LogSeverity minSeverity = LogSeverity.Info;
			AddRollingFileLogger(logFolder, applicationName, dateTemplate, minSeverity);
		}

		/// <summary>
		/// Adds the rolling file logger.
		/// </summary>
		/// <param name="logFolder">The log folder.</param>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="dateTemplate">The date template (yyyyMM-rolls the log every month, yyyyMMdd-rolls the log every day.</param>
		/// <param name="minSeverityToLog">The min severity to log.</param>
		public static void AddRollingFileLogger(string logFolder, string applicationName, string dateTemplate, LogSeverity minSeverityToLog)
		{
			string logFileTemplate = logFolder + "logfile{0}.txt";

			RollingFileLogger.RollOverDateStrategy ro =
				new RollingFileLogger.RollOverDateStrategy(logFileTemplate, dateTemplate);
			BitFactory.Logging.Logger fileLogger = new RollingFileLogger(ro)
			                                       	{
			                                       		Application = applicationName,
			                                       		SeverityThreshold = (BitFactory.Logging.LogSeverity) minSeverityToLog,
			                                       		Formatter = new TabSeperatedLogEntryFormatter()
			                                       	};

			Instance.AddLogger("File", fileLogger);
			Instance.Application = applicationName;
		}

		/// <summary>
		/// Parses the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public static List<LogEntry> ParseFile(string fileName)
		{
			TabSeperatedLogEntryFormatter formatter = new TabSeperatedLogEntryFormatter();
			List<LogEntry> all = formatter.ParseFile(fileName);

			SortList(all);

			return all;
		}

		/// <summary>
		/// Parses the files in folder.
		/// </summary>
		/// <param name="folderName">Name of the folder.</param>
		/// <returns></returns>
		public static List<LogEntry> ParseFilesInFolder(string folderName)
		{
			List<LogEntry> all = new List<LogEntry>();
			if (Directory.Exists(folderName))
			{
				string[] fileNames = Directory.GetFiles(folderName, "*.txt");
				foreach (string fileName in fileNames)
				{
					List<LogEntry> entries = ParseFile(fileName);
					all.AddRange(entries);
				}
			}

			SortList(all);

			return all;
		}

		/// <summary>
		/// Sorts the list.
		/// </summary>
		/// <param name="data">The data.</param>
		private static void SortList(List<LogEntry> data)
		{
			data.Sort((e1, e2) => e2.Date.CompareTo(e1.Date));
		}

		/// <summary>
		/// Logs the critical.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogCritical(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Critical, anObject);
		}

		/// <summary>
		/// Logs the critical.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogCritical(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Critical, aCategory, anObject);
		}

		/// <summary>
		/// Logs the debug.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogDebug(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Debug, anObject);
		}

		/// <summary>
		/// Logs the debug.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogDebug(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Debug, aCategory, anObject);
		}

		/// <summary>
		/// Logs the error.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogError(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Error, anObject);
		}

		/// <summary>
		/// Logs the error.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogError(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Error, aCategory, anObject);
		}

		/// <summary>
		/// Logs the fatal.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogFatal(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Fatal, anObject);
		}

		/// <summary>
		/// Logs the fatal.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogFatal(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Fatal, aCategory, anObject);
		}

		/// <summary>
		/// Logs the info.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogInfo(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Info, anObject);
		}

		/// <summary>
		/// Logs the info.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogInfo(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Info, aCategory, anObject);
		}

		/// <summary>
		/// Logs the status.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogStatus(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Status, anObject);
		}

		/// <summary>
		/// Logs the status.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogStatus(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Status, aCategory, anObject);
		}

		/// <summary>
		/// Logs the warning.
		/// </summary>
		/// <param name="anObject">An object.</param>
		public static void LogWarning(Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Warning, anObject);
		}

		/// <summary>
		/// Logs the warning.
		/// </summary>
		/// <param name="aCategory">A category.</param>
		/// <param name="anObject">An object.</param>
		public static void LogWarning(Object aCategory, Object anObject)
		{
			Instance.Log(BitFactory.Logging.LogSeverity.Warning, aCategory, anObject);
		}
	}

	/// <summary>
	/// Log Entry
	/// </summary>
	public class LogEntry
	{
		public DateTime Date { get; set; }
		public LogSeverity Severity { get; set; }
		public string Category { get; set; }
		public string Message { get; set; }
		public string Application { get; set; }
	}

	/// <summary>
	/// Severity of the log entry.
	/// </summary>
	public enum LogSeverity
	{
		Debug = 1,
		Info = 2,
		Status = 3,
		Warning = 4,
		Error = 5,
		Critical = 6,
		Fatal = 7
	}

	/// <summary>
	/// Tab seperated log entry.
	/// </summary>
	class TabSeperatedLogEntryFormatter : CharacterSeperatedLogEntryFormatter
	{
		public TabSeperatedLogEntryFormatter()
			: base("\t")
		{
		}
	}

	/// <summary>
	/// This class formats the log entry into a string before it
	/// is serialized to the end point (file, smtp, ...). All elements
	/// are seperated by the string passed into the constructor.
	/// </summary>
	class CharacterSeperatedLogEntryFormatter : LogEntryFormatter
	{
		private readonly string _seperatorChar;

		protected override String AsString(BitFactory.Logging.LogEntry logEntry)
		{
			StringBuilder bldr = new StringBuilder();
			bldr.Append(DateString(logEntry) + _seperatorChar);
			bldr.Append(logEntry.SeverityString + _seperatorChar);
			if (logEntry.Category == null)
			{
				bldr.Append("Unknown" + _seperatorChar);
			}
			else
			{
				string cat = "Unknown";
				try
				{
					cat = logEntry.Category.ToString();
				}
				catch (Exception)
				{}
				cat += _seperatorChar;
				bldr.Append(cat);
			}
			bldr.Append(logEntry.Message + _seperatorChar);
			if (string.IsNullOrEmpty(logEntry.Application))
			{
				bldr.Append("Unknown" + _seperatorChar);
			}
			else
			{
				bldr.Append(logEntry.Application);
			}

			return bldr.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CharacterSeperatedLogEntryFormatter"/> class.
		/// </summary>
		/// <param name="seperator">The seperator.</param>
		public CharacterSeperatedLogEntryFormatter(string seperator)
		{
			_seperatorChar = seperator;
		}

		/// <summary>
		/// Parses a log file and return a list of entries
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public List<LogEntry> ParseFile(string fileName)
		{
			List<LogEntry> logEntrys = new List<LogEntry>();
			if (File.Exists(fileName))
			{
				string[] lines = File.ReadAllLines(fileName);
				foreach (string line in lines)
				{
					string[] token = new[] { _seperatorChar };
					string[] parts = line.Split(token, StringSplitOptions.RemoveEmptyEntries);
					if (parts.Count() == 5)
					{
						LogEntry logEntry = new LogEntry();

						DateTime date;
						if (!DateTime.TryParse(parts[0], out date))
						{
							// Expected a MM/DD/YYYY format but may have a
							//	DD/MM/YYYY instead
							string[] dateParts = parts[0].Split('/');
							if(dateParts.Length!=3)
							{
								continue;
							}
							string rearranged = dateParts[1] + "/" + dateParts[0] + "/" + dateParts[2];
							if (!DateTime.TryParse(rearranged, out date))
							{
								continue;
							}
						}
						logEntry.Date = date;
						logEntry.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), parts[1]);
						logEntry.Category = parts[2];
						logEntry.Message = parts[3];
						logEntry.Application = parts[4];

						logEntrys.Add(logEntry);
					}
				}
			}

			return logEntrys;
		}
	}
}