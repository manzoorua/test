using System;
using System.Collections.Generic;
using Cravens.Utilities.Logging;
using Cravens.Utilities.Plugin;
using CuttingEdge.Conditions;

namespace Rlc.Monitor.Packages.Client
{
	public class ClientMessageProcessorFactory
	{
		private readonly string _pluginFolder;
		private readonly ILogger _logger;

		private readonly Dictionary<Guid, Type> _plugins = new Dictionary<Guid, Type>();

		public ClientMessageProcessorFactory(ILogger logger, string pluginFolder)
		{
			Condition.Requires(pluginFolder, "pluginFolder")
				.IsNotNullOrEmpty()
				.DirectoryExists();

			Condition.Requires(logger, "logger")
				.IsNotNull();

			// Copy local.
			//
			_pluginFolder = pluginFolder;
			_logger = logger;

			// Initialize the plugin list.
			//
			FindPlugins();
		}

		public IClientMessageProcessor Create(Guid pluginId)
		{
			if(_plugins.Count==0)
			{
				// Try to initialize again.
				//
				FindPlugins();
			}

			Type type = _plugins[pluginId];
			if(type==null)
			{
				return null;
			}

			IClientMessageProcessor plugin = (IClientMessageProcessor)Activator.CreateInstance(type);
			return plugin;
		}

		private void FindPlugins()
		{
			PluginFinder<IClientMessageProcessor> finder = new PluginFinder<IClientMessageProcessor>(_logger);
			List<Type> foundTypes = finder.FindPlugins(_pluginFolder, "*.dll");

			foreach (Type type in foundTypes)
			{
				IClientMessageProcessor plugin;
				try
				{
					plugin = (IClientMessageProcessor)Activator.CreateInstance(type);
				}
				catch
				{
					_logger.Error("Could not create an instance of type=" + type);
					plugin = null;
				}
				
				if(plugin!=null)
				{
					// Looks like we can create one of these...register it.
					//
					_plugins[plugin.Id] = type;
				}
			}
		}
	}
}