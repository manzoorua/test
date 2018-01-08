using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class ImageDataMessage : BaseMessage
	{
		public byte[] ImageData { get; set; }
		public bool IsThumbnail { get; set; }
		public bool IsPartial { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int FullWidth { get; set; }
		public int FullHeight { get; set; }
        
		public ImageDataMessage() { }

		public ImageDataMessage(AgentData agent, bool isThumbnail, byte[] imageData, int fullWidth, int fullHeight)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
			IsThumbnail = isThumbnail;
			ImageData = imageData;
			IsPartial = false;
			FullWidth = fullWidth;
			FullHeight = fullHeight;
		}
	}
}