using System;
using System.Collections.Generic;
using System.IO;

namespace Cravens.Utilities.IO
{
	/// <summary>
	/// Summary description for TempFiles
	/// </summary>
	public sealed class TempFiles : IDisposable
	{
		#region Fields

		private readonly List<string> _files;
		private bool _removeEmptyFolders;

		#endregion Fields

		public bool RemoveEmptyFolders
		{
			get { return _removeEmptyFolders; }
			set { _removeEmptyFolders = value; }
		}

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public TempFiles()
		{
			_files = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TempFiles"/> class.
		/// </summary>
		/// <param name="removeEmptyFolders">if set to <c>true</c> [remove empty folders].</param>
		public TempFiles(bool removeEmptyFolders)
		{
			_files = new List<string>();
			_removeEmptyFolders = removeEmptyFolders;
		}

		/// <summary>
		/// Destructor....force the cleanup of the files
		/// </summary>
		~TempFiles()
		{
			Dispose();
		}

		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Add a file to the list to be deleted.
		/// </summary>
		/// <param name="strFileName"></param>
		public void AddFile(string strFileName)
		{
			_files.Add(strFileName);
		}

		/// <summary>
		/// Add a list of files to the delete list
		/// </summary>
		/// <param name="files"></param>
		public void AddFiles(List<string> files)
		{
			foreach (string strFile in files)
			{
				AddFile(strFile);
			}
		}

		/// <summary>
		/// Add a file to the list of files to be deleted.
		/// </summary>
		/// <param name="files"></param>
		public void AddFiles(string[] files)
		{
			foreach (string strFile in files)
			{
				AddFile(strFile);
			}
		}

		/// <summary>
		/// Delete the files now
		/// </summary>
		public void DeleteFiles()
		{
			TempFiles fileNotDeleted = null;
			foreach (string strFileName in _files)
			{
				try
				{
					File.Delete(strFileName);
				}
				catch
				{
					if (fileNotDeleted == null)
					{
						fileNotDeleted = new TempFiles(RemoveEmptyFolders);
					}
					fileNotDeleted.AddFile(strFileName);
				}
			}

			if (_removeEmptyFolders)
			{
				DeleteEmptyFolders();
			}

			_files.Clear();
		}

		/// <summary>
		/// Deletes the empty folders.
		/// </summary>
		public void DeleteEmptyFolders()
		{
			foreach (string fileName in _files)
			{
				string directory = Path.GetDirectoryName(fileName);
				if (Directory.Exists(directory))
				{
					while (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
					{
						Directory.Delete(directory);
						directory = Path.Combine(directory, "..");
						directory = Path.GetFullPath(directory);
					}
				}
			}
		}

		/// <summary>
		/// Override that does the file deletion
		/// </summary>
		public void Dispose()
		{
			// Delete the files before disposing of the array
			DeleteFiles();
		}

		#endregion Public Methods
	}
}