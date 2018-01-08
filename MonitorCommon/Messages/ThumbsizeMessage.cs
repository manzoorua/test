using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class ThumbsizeMessage : BaseMessage
	{
		public static int DefaultWidth = 300;
		public static int DefaultHeight = 225;

		public int Width { get; set; }
		public int Height { get; set; }

		public ThumbsizeMessage()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
		}

		public ThumbsizeMessage(AgentData agent, int width, int height)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
			Width = width;
			Height = height;
		}
	}
}