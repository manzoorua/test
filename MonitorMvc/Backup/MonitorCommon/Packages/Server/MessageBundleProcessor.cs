using System;
using System.Collections.Generic;
using Cravens.Utilities.Logging;
using CuttingEdge.Conditions;

namespace Rlc.Monitor.Packages.Server
{
	public class MessageBundleProcessor
	{
		private readonly string _pluginFolder;
		private readonly ILogger _logger;

		private readonly ServerMessageProcessorFactory _serverMessageProcessorFactory;

		public MessageBundleProcessor(string pluginFolder, ILogger logger)
		{
			// Validate inputs.
			//
			Condition.Requires(pluginFolder, "pluginFolder")
				.IsNotNullOrEmpty()
				.DirectoryExists();

			Condition.Requires(logger, "logger")
				.IsNotNull();

			// Copy local.
			//
			_pluginFolder = pluginFolder;
			_logger = logger;

			// Create the plugin factory.
			//
			_serverMessageProcessorFactory = new ServerMessageProcessorFactory(_logger, _pluginFolder);
		}

		public MessageBundle Process(MessageBundle messageBundle)
		{
			Condition.Requires(messageBundle, "messageBundle")
				.IsNotNull();

			// Create result.
			//
			MessageBundle result = new MessageBundle
			                       	{
			                       		AgentId = messageBundle.AgentId, 
										Messages = new List<Message>()
			                       	};

			// Process each message.
			//
			foreach (Message message in messageBundle.Messages)
			{
				IServerMessageProcessor serverMessageProcessor =
					_serverMessageProcessorFactory.Create(message.PluginId);
				if(serverMessageProcessor!=null)
				{
					List<Message> temp;
					try
					{
						temp = serverMessageProcessor.Process(message);
					}
					catch (Exception ex)
					{
						_logger.Error(serverMessageProcessor.Name + " : Threw an exception. ", ex);
						temp = null;
					}
					if(temp!=null)
					{
						result.Messages.AddRange(temp);
					}
				}
			}

			return result;
		}
	}
}