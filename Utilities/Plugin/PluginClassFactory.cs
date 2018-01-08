using System;

namespace Cravens.Utilities.Plugin
{
	/// <summary>
	/// Generic class factory for instantiating a type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class PluginClassFactory<T>
	{
		#region Public Properties

		public Type Type { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PluginClassFactory&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="instanceType">Type of the instance.</param>
		public PluginClassFactory(Type instanceType)
		{
			Type = instanceType;
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns>An instace of type T</returns>
		public T CreateInstance()
		{
			return (T)Activator.CreateInstance(Type);
		}

		public T CreateInstance(object[] args)
		{
			if (args == null)
			{
				return (T)Activator.CreateInstance(Type);
			}
			return (T)Activator.CreateInstance(Type, args);
		}

		#endregion Public Methods
	}
}