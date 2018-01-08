using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Cravens.Utilities.Images;
using Cravens.Utilities.ZipHelpers;
using MonitorAgent.ScreenCapture;
using MonitorAgent.WebService;
using BaseMessage=MonitorAgent.WebService.BaseMessage;
using ChatMessage=MonitorAgent.WebService.ChatMessage;
using EmptyMessage=MonitorAgent.WebService.EmptyMessage;
using ImageDataMessage=MonitorAgent.WebService.ImageDataMessage;
using ImageNoChangeMessage=MonitorAgent.WebService.ImageNoChangeMessage;
using PollIntervalMessage=MonitorAgent.WebService.PollIntervalMessage;
using PollModeMessage=MonitorAgent.WebService.PollModeMessage;
using RegisterMessage=MonitorAgent.WebService.RegisterMessage;
using ThumbsizeMessage=MonitorAgent.WebService.ThumbsizeMessage;
using Timer=System.Timers.Timer;
using System.IO;

namespace MonitorAgent
{
	public class Agent
	{
		private readonly CommandHandlerSoapClient _webService;
		private readonly IScreenCapture _screenCapture;
		private readonly IImageProcessing _imageProcessor;
		private readonly Guid _agentId;
		private readonly Timer _timer = new Timer(5000);
		private readonly object _startStopLock = new object();
		private Size _thumbNailSize = new Size(300, 225);
		private enum PollModeOptions { Register, Poll, Thumb, Image };
		private PollModeOptions _pollMode = PollModeOptions.Register;
		private Bitmap _previousScreenShot;
		private string _currentDirectory = Environment.CurrentDirectory;

		public string PollMode { get { return _pollMode.ToString(); } }
		public Size ImageFullSize { get; private set; }
		public Size PartialImageSize { get; private set; }
		public DateTime LastPoll { get; private set; }
		public int PollInterval
		{
			get
			{
				if(_timer.Enabled)
				{
					return (int)_timer.Interval;
				}
				return 0;
			}
		}
		public string CurrentDirectory { get { return _currentDirectory; } }

		public delegate void PollEvent(Agent agent);
		public event PollEvent OnPollEvent;



		public Agent(Guid agentId)
		{
			_agentId = agentId;
			_webService = new CommandHandlerSoapClient();
			_screenCapture = new ScreenCapture.ScreenCapture();
			_imageProcessor = new ImageProcessing();
			_timer.Elapsed += Poll;
		}

		public bool Start()
		{
			lock(_startStopLock)
			{
				if(_timer.Enabled)
				{
					return false;
				}
				_timer.Enabled = true;
			}
			return true;
		}

		public bool Stop()
		{
			lock(_startStopLock)
			{
				if(!_timer.Enabled)
				{
					return false;
				}
				_timer.Enabled = false;
			}
			return true;
		}

		#region Private Members

		private void Poll(object sender, System.Timers.ElapsedEventArgs e)
		{
			switch (_pollMode)
			{
				case PollModeOptions.Register:
					{
						Register();
						break;
					}
				case PollModeOptions.Poll:
					{
						CheckForMessages();
						break;
					}
				case PollModeOptions.Thumb:
				case PollModeOptions.Image:
					{
						PostImage();
						break;
					}
			}
			LastPoll = DateTime.Now;

			if(OnPollEvent!=null)
			{
				OnPollEvent(this);
			}
		}

		/// <summary>
		/// Registers this instance.
		/// </summary>
		private void Register()
		{
			// Register with the server.
			//
			_previousScreenShot = null;
			RegisterMessage registerMessage = new RegisterMessage
			{
				AgentId = _agentId,
				ComputerName = Environment.MachineName,
				TimeStamp = DateTime.Now,
				UserInfo = Environment.UserDomainName + "\\" + Environment.UserName
			};
			PostMessage(registerMessage);
		}

		/// <summary>
		/// Polls this instance.
		/// </summary>
		private void CheckForMessages()
		{
			EmptyMessage emptyMessage = new EmptyMessage { AgentId = _agentId, TimeStamp = DateTime.Now };
			PostMessage(emptyMessage);
		}

		/// <summary>
		/// Posts the image.
		/// </summary>
		private void PostImage()
		{
			// Capture a new screen image.
			//
			Bitmap screenShot = _screenCapture.CaptureDesktopWithCursor();

			if (_pollMode == PollModeOptions.Thumb)
			{
				// Scale the image
				//
				screenShot = screenShot.Resize(
					RotateFlipType.RotateNoneFlipNone,
					_thumbNailSize.Width,
					_thumbNailSize.Height);
			}
			ImageFullSize = new Size(screenShot.Width, screenShot.Height);

			// Compare to the previous bitmap to determine the
			//	bounding box for changed pixels. This helps minimize
			//	the number of bytes that have to send.
			//
			Rectangle rect = _imageProcessor.GetBoundingBoxForChanges(_previousScreenShot, screenShot);
			PartialImageSize = new Size(rect.Width, rect.Height);
			_previousScreenShot = screenShot;
			if (rect != Rectangle.Empty)
			{
				// Create an initialize an image data message.
				//
				ImageDataMessage imageDataMessage = new ImageDataMessage
				{
					AgentId = _agentId,
					TimeStamp = DateTime.Now,
					IsThumbnail = (_pollMode == PollModeOptions.Thumb) ? true : false,
					FullWidth = screenShot.Width,
					FullHeight = screenShot.Height
				};

				if (rect.Width == screenShot.Width &&
					rect.Height == screenShot.Height)
				{
					// Post the whole screen
					//
					imageDataMessage.ImageData = screenShot.ConvertToByteArray();
					imageDataMessage.IsPartial = false;
					imageDataMessage.X = 0;
					imageDataMessage.Y = 0;
				}
				else
				{
					// Post only the part of the screen that has changed.
					//
					Bitmap changedPart = ImageResize.Crop(screenShot, rect);
					imageDataMessage.ImageData = changedPart.ConvertToByteArray();
					imageDataMessage.IsPartial = true;
					imageDataMessage.X = rect.X;
					imageDataMessage.Y = rect.Y;
				}

				PostMessage(imageDataMessage);
			}
			else
			{
				ImageNoChangeMessage imageNoChangeMessage = new ImageNoChangeMessage
				{
					AgentId = _agentId,
					TimeStamp = DateTime.Now
				};
				PostMessage(imageNoChangeMessage);
			}
		}

		/// <summary>
		/// Posts a message to the server.
		/// Messages are received in the response and processed.
		/// </summary>
		/// <param name="message">The message.</param>
		private void PostMessage(BaseMessage message)
		{
			BaseMessage[] messages = null;
			try
			{
				messages = _webService.Poll(message);
			}
			catch (Exception ex)
			{
				// Service must be down.
				//
				MessageBox.Show(ex.ToString());
				_pollMode = PollModeOptions.Register;
			}

			// Process commands.
			//
			if (messages != null)
			{
				ProcessMessages(messages);
			}
		}

		/// <summary>
		/// Processes the messages.
		/// </summary>
		/// <param name="messages">The messages.</param>
		private void ProcessMessages(IEnumerable<BaseMessage> messages)
		{
			if (messages != null)
			{
				foreach (BaseMessage message in messages)
				{
					if (message is PollIntervalMessage)
					{
						ProcessPollIntervalMessage(message as PollIntervalMessage);
					}
					else if (message is ThumbsizeMessage)
					{
						ProcessThumbsizeMessage(message as ThumbsizeMessage);
					}
					else if (message is ChatMessage)
					{
						ProcessChatMessage(message as ChatMessage);
					}
					else if (message is PollModeMessage)
					{
						ProcessPollModeMessage(message as PollModeMessage);
					}
					else if(message is DirectoryCommandMessage)
					{
						ProcessDirectoryCommandMessage(message as DirectoryCommandMessage);
					}
				}
			}
		}

		/// <summary>
		/// Processes the poll mode message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private void ProcessPollModeMessage(PollModeMessage message)
		{
			if (message.PollMode == WebService.PollModeOptions.PollForCommands)
			{
				_pollMode = PollModeOptions.Poll;
			}
			else if (message.PollMode == WebService.PollModeOptions.PostFullsize)
			{
				_pollMode = PollModeOptions.Image;
			}
			else if (message.PollMode == WebService.PollModeOptions.PostThumbnail)
			{
				_pollMode = PollModeOptions.Thumb;
			}
			else if (message.PollMode == WebService.PollModeOptions.Register)
			{
				_pollMode = PollModeOptions.Register;
			}
		}

		/// <summary>
		/// Processes the chat message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private void ProcessChatMessage(ChatMessage message)
		{
			return;
		}

		/// <summary>
		/// Processes the thumbsize message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private void ProcessThumbsizeMessage(ThumbsizeMessage message)
		{
			Size size = new Size(message.Width, message.Height);
			_thumbNailSize = size;
			return;
		}

		/// <summary>
		/// Processes the poll interval message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private void ProcessPollIntervalMessage(PollIntervalMessage message)
		{
			_timer.Enabled = false;
			_timer.Interval = message.IntervalInMilliseconds;
			_timer.Enabled = true;
			return;
		}

		/// <summary>
		/// Processes the directory command message.
		/// </summary>
		/// <param name="message">The message.</param>
		private void ProcessDirectoryCommandMessage(DirectoryCommandMessage message)
		{
			switch(message.Command)
			{
				case WebService.Action.FetchCurrentData:
					{
						DirectoryDataMessage ddm = GetDirectoryDataMessage();

						PostMessage(ddm);
						break;
					}
				case WebService.Action.ChangeDirectory:
					{
						if(Directory.Exists(message.CurrentDirectory))
						{
							_currentDirectory = message.CurrentDirectory;
							if(!_currentDirectory.EndsWith("\\") || !_currentDirectory.EndsWith("/"))
							{
								_currentDirectory += "/";
							}

							DirectoryDataMessage ddm = GetDirectoryDataMessage();

							PostMessage(ddm);
						}
						break;
					}
				case WebService.Action.FetchContent:
					{
						ZipHelper zipHelper = new ZipHelper();
						foreach (string file in message.ContentToFetch)
						{
							if(Directory.Exists(file))
							{
								// Directory
								string folderName = Path.GetFileName(file);
								zipHelper.AddFolder(file, folderName);
							}
							else if(File.Exists(file))
							{
								zipHelper.AddFile(file, "");
							}
						}

						MemoryStream memoryStream = new MemoryStream();
						zipHelper.Save(memoryStream);

						FileDataMessage fdm = new FileDataMessage
						                      	{
						                      		AgentId = _agentId,
						                      		FileName = "files.zip",
						                      		FileData = memoryStream.GetBuffer(),
						                      		TimeStamp = DateTime.Now
						                      	};
						PostMessage(fdm);

						break;
					}
			}
		}

		private DirectoryDataMessage GetDirectoryDataMessage()
		{
			string[] folders = Directory.GetDirectories(_currentDirectory);
			string[] files = Directory.GetFiles(_currentDirectory);
			DirectoryDataMessage ddm = new DirectoryDataMessage
			                           	{
			                           		AgentId = _agentId,
			                           		TimeStamp = DateTime.Now,
			                           		CurrentFolder = _currentDirectory,
			                           		Folders = new ArrayOfString(),
			                           		Files = new ArrayOfString()
			                           	};
			ddm.Folders.AddRange(folders);
			ddm.Files.AddRange(files);
			return ddm;
		}

		#endregion
	}
}
