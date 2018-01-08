using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class ImageRequestMessage : BaseMessage
	{
		public bool IsPartial { get; set;}

		public ImageRequestMessage() {}
	}
}