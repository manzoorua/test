using System;
using System.Collections.Generic;
using Cravens.Utilities.Logging;
using CuttingEdge.Conditions;

namespace Rlc.Monitor.Packages.Client
{
	public class MessageBundleProcessor
	{
		private readonly string _pluginFolder;
		private readonly ILogger _logger;

		private readonly ClientMessageProcessorFactory _clientMessageProcessorFactory;

		public DateTime LastUpdate { get; set; }

		public MessageBundleProcessor(string clientSidePluginFolder, ILogger logger)
		{
			Condition.Requires(clientSidePluginFolder).IsNotNullOrEmpty().DirectoryExists();
			_pluginFolder = clientSidePluginFolder;

			Condition.Requires(logger).IsNotNull();
			_logger = logger;

			_clientMessageProcessorFactory = new ClientMessageProcessorFactory(_logger, _pluginFolder);
		}

		public MessageBundle Process(MessageBundle messageBundle)
		{
			Condition.Requires(messageBundle)
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
				IClientMessageProcessor clientMessageProcessor =
					_clientMessageProcessorFactory.Create(message.PluginId);
				if (clientMessageProcessor != null)
				{
					List<Message> temp;
					try
					{
						temp = clientMessageProcessor.Process(message);
					}
					catch (Exception ex)
					{
						_logger.Error(clientMessageProcessor.Name + " : Threw an exception. ", ex);
						temp = null;
					}
					if (temp != null)
					{
						result.Messages.AddRange(temp);
					}
				}
			}

			return result;
		}
	}
}