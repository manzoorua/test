using System.Collections;
using System.IO;
using Ionic.Zip;

namespace Cravens.Utilities.ZipHelpers
{
	/// <summary>
	/// Zipping helpers
	/// </summary>
	public class ZipHelper
	{
		#region Fields

		private readonly ZipFile _zipFile;

		#endregion Fields

		public ZipHelper()
		{
			_zipFile = new ZipFile();
		}

		#region Public Methods

		/// <summary>
		/// Add a file or a folder. All folder contents are recursively added.
		/// </summary>
		/// <param name="strName"></param>
		/// <returns></returns>
		public bool Add(string strName)
		{
			if (File.Exists(strName))
			{
				return AddFile(strName);
			}
			return AddFolder(strName);
		}

		/// <summary>
		/// Add files or folders in any combination. All folder contents are recursively added.
		/// </summary>
		/// <param name="strNames"></param>
		/// <returns></returns>
		public bool Add(string[] strNames)
		{
			// Check each object individually
			bool ret = true;

			foreach (string strName in strNames)
			{
				ret = ret && Add(strName);
			}

			return ret;
		}

		/// <summary>
		/// Add files or folders in any combination. All folder contents are recursively added.
		/// </summary>
		/// <param name="strNames"></param>
		/// <returns></returns>
		public bool Add(ArrayList strNames)
		{
			// Check each object individually
			bool ret = true;
			for(int i=0;i<strNames.Count;i++)
			{
				ret = ret && Add(strNames[i].ToString());
			}

			return ret;
		}

		/// <summary>
		/// Add a single file to the zip container.
		/// </summary>
		/// <param name="strFile"></param>
		/// <returns></returns>
		public bool AddFile(string strFile)
		{
			try
			{
				_zipFile.AddFile(strFile);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Adds the file stream.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="directoryPathInArchive">The directory path in archive.</param>
		/// <param name="fileStream">The file stream.</param>
		/// <returns></returns>
		public bool AddFileStream(string fileName, string directoryPathInArchive, Stream fileStream)
		{
			try
			{
				_zipFile.AddEntry(fileName, directoryPathInArchive, fileStream);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Adds the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="directoryPathInArchive">The directory path in archive.</param>
		/// <returns></returns>
		public bool AddFile(string fileName, string directoryPathInArchive)
		{
			try
			{
				_zipFile.AddFile(fileName, directoryPathInArchive);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Add multiple files to the zip container.
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public bool AddFiles(string[] files)
		{
			bool ret = true;

			foreach (string strFile in files)
			{
				ret = ret && AddFile(strFile);
			}

			return ret;
		}

		/// <summary>
		/// Adds the files.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <param name="directoryPathInArchive">The directory path in archive.</param>
		/// <returns></returns>
		public bool AddFiles(string[] files, string directoryPathInArchive)
		{
			bool ret = true;

			foreach (string strFile in files)
			{
				ret = ret && AddFile(strFile, directoryPathInArchive);
			}

			return ret;
		}

		/// <summary>
		/// Add multiple files to the zip container.
		/// </summary>
		/// <param name="arrFiles"></param>
		/// <returns></returns>
		public bool AddFiles(ArrayList arrFiles)
		{
			bool ret = true;

			for (int i = 0; i < arrFiles.Count; i++)
			{
				ret = ret && AddFile(arrFiles[i].ToString());
			}

			return ret;
		}

		/// <summary>
		/// Adds the files.
		/// </summary>
		/// <param name="arrFiles">The arr files.</param>
		/// <param name="directoryPathInArchive">The directory path in archive.</param>
		/// <returns></returns>
		public bool AddFiles(ArrayList arrFiles, string directoryPathInArchive)
		{
			bool ret = true;

			for (int i = 0; i < arrFiles.Count; i++)
			{
				ret = ret && AddFile(arrFiles[i].ToString(), directoryPathInArchive);
			}

			return ret;
		}

		/// <summary>
		/// Add the files of a folder and possibly all child folders to the zip container.
		/// </summary>
		/// <param name="folderName">Name of the folder.</param>
		/// <returns></returns>
		public bool AddFolder(string folderName)
		{
			try
			{
				_zipFile.AddDirectory(folderName);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Adds the folder.
		/// </summary>
		/// <param name="folderName">Name of the folder.</param>
		/// <param name="directoryPathInArchive">The directory path in archive.</param>
		/// <returns></returns>
		public bool AddFolder(string folderName, string directoryPathInArchive)
		{
			try
			{
				_zipFile.AddDirectory(folderName, directoryPathInArchive);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Add the files of multiple folder and possibly all child folders to the zip container.
		/// </summary>
		/// <param name="strFolders">The STR folders.</param>
		/// <returns></returns>
		public bool AddFolders(string[] strFolders)
		{
			bool ret = true;

			foreach (string strFolder in strFolders)
			{
				ret = ret && AddFolder(strFolder);
			}

			return ret;
		}

		/// <summary>
		/// Add the files of multiple folder and possibly all child folders to the zip container.
		/// </summary>
		/// <param name="arrFolders">The arr folders.</param>
		/// <returns></returns>
		public bool AddFolders(ArrayList arrFolders)
		{
			bool ret = true;

			for (int i = 0; i < arrFolders.Count; i++)
			{
				ret = ret && AddFolder(arrFolders[i].ToString());
			}

			return ret;
		}

		/// <summary>
		/// Commit and save the contents added to the zip file
		/// </summary>
		/// <returns></returns>
		public bool Save(string fileName)
		{
			try
			{
				_zipFile.Save(fileName);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Saves the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public bool Save(Stream stream)
		{
			try
			{
				_zipFile.Save(stream);
			}
			catch
			{
				return false;
			}

			return true;
		}
		#endregion Public Methods
	}
}