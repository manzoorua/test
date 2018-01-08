using System;
using System.Collections.Generic;
using System.IO;
using Rlc.Utilities.Log;

namespace Rlc.Utilities.Plugin
{
	/// <summary>
	/// Plugin manager is used to dynamically load plugins of Type T
	/// where Type T implemnts the IPlugin interface.
	/// </summary>
	/// <typeparam name="T">Plugin type to manage</typeparam>
	public class PluginManager<T>
		where T : IPlugin
	{
		private readonly ILogger _logger;
		private readonly string _pluginPath = String.Empty;
		private readonly List<PluginWrapper<T>> _pluginWrapperList = new List<PluginWrapper<T>>();

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PluginManager&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public PluginManager(ILogger logger)
			: this(null, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PluginManager&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="pluginPath">The plugin path.</param>
		/// <param name="logger">The logger.</param>
		public PluginManager(string pluginPath, ILogger logger)
		{
			_pluginPath = pluginPath;
			if (!string.IsNullOrEmpty(pluginPath))
			{
				DirectoryInfo dInfo = new DirectoryInfo(pluginPath);

				if (!dInfo.Exists)
				{
					throw new DirectoryNotFoundException(
						String.Format("Plugin directory not found. {0}", pluginPath));
				}
			}

			_logger = logger;
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return _pluginWrapperList.Count;
			}
		}

		/// <summary>
		/// Gets the plugin id list.
		/// </summary>
		/// <value>The plugin id list.</value>
		public List<Guid> PluginIdList
		{
			get
			{
				List<Guid> idList = new List<Guid>();

				foreach (PluginWrapper<T> w in _pluginWrapperList)
				{
					idList.Add(w.Id);
				}

				return idList;
			}
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Creates the instance by plugin id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public T CreateInstanceByPluginId(Guid id)
		{
			T plugin = default(T);

			foreach (PluginWrapper<T> wrapper in _pluginWrapperList)
			{
				if (wrapper.Id == id)
				{
					// Use the factory to create an instance of the class.
					//
					plugin = wrapper.Factory.CreateInstance();

					break;
				}
			}

			return plugin;
		}

		/// <summary>
		/// Loads the plugins.
		/// </summary>
		/// <param name="searchPattern">The search pattern.</param>
		public void LoadPlugins(string searchPattern)
		{
			LoadPlugins(searchPattern, null, null);
		}

		/// <summary>
		/// Loads the plugins.
		/// </summary>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="publicKeyToMatch">The public key to match.</param>
		public void LoadPlugins(string searchPattern, byte[] publicKeyToMatch)
		{
			LoadPlugins(searchPattern, publicKeyToMatch, null);
		}

		/// <summary>
		/// Gets the type by plugin id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public Type GetTypeByPluginId(Guid id)
		{
			Type result = null;

			foreach (PluginWrapper<T> wrapper in _pluginWrapperList)
			{
				if (wrapper.Id == id)
				{
					// Use the factory to create an instance of the class.
					//
					result = wrapper.Factory.Type;

					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Load all the plugins of type T.
		/// </summary>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="publicKeyToMatch">The public key to match.</param>
		/// <param name="args">The args.</param>
		public void LoadPlugins(string searchPattern, byte[] publicKeyToMatch, object[] args)
		{
			// Logging statements.
			//
			_logger.Debug("plugin path=" + _pluginPath);
			_logger.Debug("search pattern=" + searchPattern);
			_logger.Debug("type=" + typeof(T));
			if (publicKeyToMatch != null)
			{
				_logger.Debug("public key=" + publicKeyToMatch);
			}

			// clear the plugin wrapper list.
			//
			_pluginWrapperList.Clear();

			// Create the plugin finder class 
			PluginFinder<T> finder = new PluginFinder<T>(_logger);

			// Get all the plugin types.
			//
			List<Type> pluginTypeList = finder.FindPlugins(_pluginPath, searchPattern, publicKeyToMatch);
			_logger.Debug("number of plugins found that match=" + pluginTypeList.Count);

			// Iterate over the types and create a plugin wrapper.
			//
			foreach (Type type in pluginTypeList)
			{
				AddPlugin(type, args);
			}
		}

		public bool AddPlugin(Type pluginType)
		{
			return AddPlugin(pluginType, null);
		}

		public bool AddPlugin(Type pluginType, object[] args)
		{
			if (!typeof(T).IsAssignableFrom(pluginType))
			{
				return false;
			}

			// Create a class factory instance for type T.
			//
			PluginClassFactory<T> factory = new PluginClassFactory<T>(pluginType);

			// Use the factory to create an instance of the plugin to
			// get at its Id and name.
			//
			IPlugin plugin = factory.CreateInstance(args);

			if (plugin != null)
			{
				// Create the wrapper to hold the plugin info and class factory.
				//
				PluginWrapper<T> wrapper = new PluginWrapper<T> { Id = plugin.Id, Factory = factory };

				// Set the members of the wrapper.
				//

				// Save a reference to the wrapper in the list.
				//
				_pluginWrapperList.Add(wrapper);

				_logger.Debug("Plugin: " + plugin.Name + " [" + plugin.Id + "], Version=" + plugin.Version);

				return true;
			}
			_logger.Debug("Could not create instance of plugin in the PluginClassFactory!");

			return false;
		}

		#endregion Public Methods

		#region Other

		/// <summary>
		/// Helper class wrapping a plugin information and the class factory
		/// to create a plugin.
		/// </summary>
		/// <typeparam name="TT">The type of the T.</typeparam>
		class PluginWrapper<TT>
		{
			#region Fields

			/// <summary>
			/// Class factory to create 
			/// </summary>
			public PluginClassFactory<TT> Factory;

			/// <summary>
			/// Id of the plugin.
			/// </summary>
			public Guid Id;

			#endregion Fields
		}

		#endregion Other
	}
}