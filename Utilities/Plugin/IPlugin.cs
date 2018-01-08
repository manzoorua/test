using System;

namespace Cravens.Utilities.Plugin
{
	public interface IPlugin
	{
		#region Properties

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		string Description
		{
			get;
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		Guid Id
		{
			get;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets the version.
		/// </summary>
		/// <value>The version.</value>
		Version Version
		{
			get;
		}

		#endregion Properties
	}
}