using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cravens.Utilities.Logging;

namespace Cravens.Utilities.Plugin
{
	/// <summary>
	/// Helper clas sto search for plugins of type T.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PluginFinder<T>
	{
		private readonly ILogger _logger;
		readonly List<Type> _typeList = new List<Type>();

		public PluginFinder(ILogger logger)
		{
			_logger = logger;
		}

		#region Public Methods

		public List<Type> FindPlugins(string searchPath, string searchPattern)
		{
			return FindPlugins(searchPath, searchPattern, null);
		}

		/// <summary>
		/// Loads the plugins.
		/// </summary>
		/// <param name="searchPath">The search path.</param>
		/// <param name="searchPatthen">The search patthen.</param>
		/// <param name="publicKeyToMatch">The public key to match.</param>
		/// <returns></returns>
		public List<Type> FindPlugins(string searchPath, string searchPatthen, byte[] publicKeyToMatch)
		{
			_typeList.Clear();

			foreach (string filePath in Directory.GetFiles(searchPath, searchPatthen))
			{
				EnumeratePlugins(filePath, publicKeyToMatch);
			}

			return _typeList;
		}

		#endregion Public Methods

		/// <summary>
		/// Load the assembly and enumerate the types looking for
		/// type T.
		/// </summary>
		/// <param name="filePath">The assembly file path.</param>
		/// <param name="publicKeyToMatch">The public key to match.</param>
		private void EnumeratePlugins(string filePath, byte[] publicKeyToMatch)
		{
			try
			{
				// Load the asembly into the current application domain.
				//
				FileInfo fileInfo = new FileInfo(filePath);

				if (fileInfo.Exists)
				{
					Assembly asm = Assembly.LoadFile(fileInfo.FullName);

					// Make sure the assembly is signed by the desired public
					//	key.
					//
					bool isKeyOk = true;
					if (publicKeyToMatch != null)
					{
						isKeyOk = false;
						AssemblyName asmName = asm.GetName();
						byte[] thisPublicKey = asmName.GetPublicKey();
						if (thisPublicKey.Length == publicKeyToMatch.Length)
						{
							isKeyOk = true;
							for (int i = 0; i < thisPublicKey.Length; i++)
							{
								if (thisPublicKey[i] != publicKeyToMatch[i])
								{
									_logger.Debug("Plugin is not signed with same key!");
									isKeyOk = false;
									break;
								}
							}
						}
					}

					if (isKeyOk)
					{
						bool addedType = false;
						foreach (Type type in asm.GetTypes())
						{
							if (!type.IsAbstract)
							{
								if (!type.IsInterface && typeof(T).IsAssignableFrom(type))
								{
									_typeList.Add(type);
									addedType = true;
								}
								else
								{
									foreach (Type iface in type.GetInterfaces())
									{
										if (iface.Equals(typeof (T)))
										{
											_typeList.Add(type);
											addedType = true;
										}
									}
								}
							}
						}
						if (!addedType)
						{
							_logger.Debug("Not added...no matching type found! filePath=" + filePath);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Excpetion while trying to enumerate plugins. filePath=" + filePath, ex);
			}
		}
	}
}