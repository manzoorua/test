using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Drawing;
using Cravens.Utilities.Images;
using Cravens.Utilities.Logging;
using Rlc.Monitor;
using Rlc.Monitor.Messages;
using Rlc.Monitor.Packages;
using MonitorMvc.Models;
using System.Xml.Serialization;
using Rlc.Monitor.Packages.Server;
using System.Web;

namespace MonitorMvc.Services
{
	/// <summary>
	/// Summary description for PostThumbnail
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class CommandHandler : WebService
	{
		private MessageBundleProcessor _messageBundleProcessor;

		[WebMethod]
		[XmlInclude(typeof(MessageBundle))]
		public MessageBundle Process(MessageBundle messageBundle)
		{
			if(_messageBundleProcessor == null)
			{
				string pluginFolder = HttpContext.Current.Request.MapPath("~App_Data\\plugins");
				ILogger logger = new DebugLogger();
				_messageBundleProcessor = new MessageBundleProcessor(pluginFolder, logger);
			}

			return _messageBundleProcessor.Process(messageBundle);
		}




		[WebMethod]
		[XmlInclude(typeof(EmptyMessage))]
		[XmlInclude(typeof(RegisterMessage))]
		[XmlInclude(typeof(ImageDataMessage))]
		[XmlInclude(typeof(PollModeMessage))]
		[XmlInclude(typeof(PollIntervalMessage))]
		[XmlInclude(typeof(ThumbsizeMessage))]
		[XmlInclude(typeof(ImageNoChangeMessage))]
		[XmlInclude(typeof(ChatMessage))]
		[XmlInclude(typeof(DirectoryCommandMessage))]
		[XmlInclude(typeof(DirectoryDataMessage))]
		[XmlInclude(typeof(FileDataMessage))]
		public List<BaseMessage> Poll(BaseMessage message)
		{
			if (message is EmptyMessage)
			{
				return ProcessEmptyMessage(message as EmptyMessage);
			}
			if (message is RegisterMessage)
			{
				return ProcessRegisterMessage(message as RegisterMessage);
			}
			if (message is ImageDataMessage)
			{
				return ProcessImageDataMessage(message as ImageDataMessage);
			}
			if (message is ImageNoChangeMessage)
			{
				return ProcessImageNoChangeMessage(message as ImageNoChangeMessage);
			}
			if (message is ChatMessage)
			{
				return ProcessChatMessage(message as ChatMessage);
			}
			if(message is DirectoryDataMessage)
			{
				return ProcessDirectoryDataMessage(message as DirectoryDataMessage);
			}
			if(message is FileDataMessage)
			{
				return ProcessFileDataMessage(message as FileDataMessage);
			}
			return ProcessEmptyMessage(message as EmptyMessage);
		}

		private static List<BaseMessage> ProcessEmptyMessage(BaseMessage message)
		{
			List<BaseMessage> msgs;

			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			lock (agentData)
			{
				msgs = new List<BaseMessage>();
				msgs.AddRange(agentData.MessagesToDeliver);

				agentData.MessagesToDeliver.Clear();
				ModelHelpers.SaveAgent(agentData);
			}

			return msgs;
		}

		private static List<BaseMessage> ProcessRegisterMessage(RegisterMessage message)
		{
			List<BaseMessage> msgs;
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			lock (agentData)
			{
				agentData.AgentId = message.AgentId;
				agentData.LastUpdate = DateTime.MinValue;
				agentData.ComputerName = message.ComputerName;
				agentData.UserName = message.UserInfo;
				agentData.ThumbNail = null;

				// Send back commands.
				//
				msgs = new List<BaseMessage>
				       	{
				       		new PollModeMessage(agentData, PollModeMessage.PollModeOptions.PostThumbnail),
				       		new ThumbsizeMessage(agentData, ThumbsizeMessage.DefaultWidth, ThumbsizeMessage.DefaultHeight),
				       		new PollIntervalMessage(agentData, PollIntervalMessage.DefaultIntervalInMilliseconds)
				       	};
				msgs.AddRange(agentData.MessagesToDeliver);

				agentData.MessagesToDeliver.Clear();
				ModelHelpers.SaveAgent(agentData);
			}

			return msgs;
		}

		private static List<BaseMessage> ProcessImageDataMessage(ImageDataMessage message)
		{
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			List<BaseMessage> msgs = new List<BaseMessage>();

			Bitmap thumbNail = ImageConvert.ConvertToBitmap(message.ImageData);
			bool isOk = true;
			if(message.IsPartial)
			{
				// Combine with the current image to get the new one.
				//
				try
				{
					Bitmap previous;
					if (agentData.ThumbNail != null)
					{
						lock (agentData.ThumbNail)
						{
							previous = new Bitmap(agentData.ThumbNail);
						}
					}
					else
					{
						previous = new Bitmap(message.FullWidth, message.FullHeight);
					}
					Rectangle bounds = new Rectangle(message.X, message.Y, thumbNail.Width, thumbNail.Height);
					using (Graphics g = Graphics.FromImage(previous))
					{
						g.DrawImage(thumbNail, bounds);
						g.Flush();
					}
					thumbNail = previous;
				}
				catch (Exception)
				{
					// image is in use elsewhere exception....
					isOk = false;
				}
			}
			if (isOk)
			{
				lock (agentData)
				{
					msgs.AddRange(agentData.MessagesToDeliver);
					agentData.MessagesToDeliver.Clear();

					agentData.LastUpdate = message.TimeStamp;
					agentData.ThumbNail = thumbNail;

					ModelHelpers.SaveAgent(agentData);

				}
			}

			return msgs;
		}

		private static List<BaseMessage> ProcessImageNoChangeMessage(ImageNoChangeMessage message)
		{
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			List<BaseMessage> msgs = new List<BaseMessage>();

			lock (agentData)
			{
				msgs.AddRange(agentData.MessagesToDeliver);
				agentData.MessagesToDeliver.Clear();

				agentData.LastUpdate = message.TimeStamp;

				ModelHelpers.SaveAgent(agentData);

			}

			return msgs;
		}

		private static List<BaseMessage> ProcessChatMessage(ChatMessage message)
		{
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			List<BaseMessage> msgs = new List<BaseMessage>();

			lock (agentData)
			{
				msgs.AddRange(agentData.MessagesToDeliver);
				agentData.MessagesToDeliver.Clear();

				ModelHelpers.AddChatMessage(message);

				ModelHelpers.SaveAgent(agentData);
			}
			ChatMessage response = new ChatMessage 
			                       	{
			                       		AgentId = Guid.Empty, 
			                       		TimeStamp = DateTime.Now, 
			                       		Text = "Message recieved!" 
			                       	};
			ModelHelpers.AddChatMessage(response);

			return msgs;
		}

		private static List<BaseMessage> ProcessDirectoryDataMessage(DirectoryDataMessage message)
		{
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			List<BaseMessage> msgs = new List<BaseMessage>();

			lock (agentData)
			{
				// Get any messages that are queued.
				//
				msgs.AddRange(agentData.MessagesToDeliver);
				agentData.MessagesToDeliver.Clear();

				// Set the last update time.
				//
				agentData.LastUpdate = message.TimeStamp;

				// Store the directory info data.
				//
				agentData.CurrentDirectory = message.CurrentFolder;
				agentData.Folders = message.Folders;
				agentData.Files = message.Files;	

				// Save the agent.
				//
				ModelHelpers.SaveAgent(agentData);

			}

			return msgs;
		}

		private static List<BaseMessage> ProcessFileDataMessage(FileDataMessage message)
		{
			AgentData agentData = ModelHelpers.FindAgent(message.AgentId);
			List<BaseMessage> msgs = new List<BaseMessage>();

			lock (agentData)
			{
				// Get any messages that are queued.
				//
				msgs.AddRange(agentData.MessagesToDeliver);
				agentData.MessagesToDeliver.Clear();

				// Set the last update time.
				//
				agentData.LastUpdate = message.TimeStamp;

				// Store the file content data.
				//
				agentData.FileDataMessage = message;

				// Save the agent.
				//
				ModelHelpers.SaveAgent(agentData);
			}

			return msgs;
		}
	}
}