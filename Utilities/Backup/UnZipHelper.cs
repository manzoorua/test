using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

namespace Rlc.Utilities
{
	/// <summary>
	/// Unzipping helpers
	/// </summary>
	public class UnZipHelper
	{
		#region Fields

		private List<string> _fileList;
		private readonly TempFiles _tempFiles;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public UnZipHelper()
		{
			// Create a temp file helper object. This object
			//  deletes files that have been added to it when
			//  the garbage collector disposes of it.
			//
			_tempFiles = new TempFiles();
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Get the list of files that were extracted.
		/// </summary>
		/// <returns>Array of full path strings.</returns>
		public List<string> FileList
		{
			get { return _fileList; }
		}

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Extracts the file names from the archive.
		/// </summary>
		/// <param name="zipFileName">Name of the zip file.</param>
		/// <returns></returns>
		public List<string> ExtractFileNames(string zipFileName)
		{
			List<string> fileNames = new List<string>();
			try
			{
				using (ZipFile zip = ZipFile.Read(zipFileName))
				{
					foreach (ZipEntry entry in zip)
					{
						fileNames.Add(entry.FileName);
					}
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("exception: " + ex);
			}

			return fileNames;
		}

		/// <summary>
		/// Extract files from a compressed zip file.
		/// </summary>
		/// <param name="zipFileName">Zip file to be unpacked.</param>
		/// <param name="targetDirectory">Directory to place zipped contents.</param>
		/// <returns></returns>
		public bool ExtractZip(string zipFileName, string targetDirectory)
		{
			try 
			{
				using (ZipFile zip = ZipFile.Read(zipFileName))
				{
					_fileList = new List<string>();
					foreach (ZipEntry entry in zip)
					{
						entry.Extract(targetDirectory);
						_fileList.Add(Path.Combine(targetDirectory, entry.FileName));
					}
				}
			}
			catch (Exception ex1)
			{
				Console.Error.WriteLine("exception: " + ex1);
			}

			return true;
		}

		/// <summary>
		/// Extract files from a compressed zip file. The garbage collector
		/// will delete the files when they are no longer being referenced.
		/// </summary>
		/// <param name="zipFileName">Zip file to be unpacked.</param>
		/// <param name="targetDirectory">Directory to place zipped contents.</param>
		/// <returns></returns>
		public bool ExtractZipTemp(string zipFileName, string targetDirectory)
		{
			// Extract the files
			//
			if (ExtractZip(zipFileName, targetDirectory))
			{
				// Add the extracted files to the temp file object
				//
				_tempFiles.AddFiles(FileList);

				return true;
			}
			return false;
		}

		#endregion Public Methods
	}
}