using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class ChatMessage : BaseMessage
	{
		public string Text { get; set; }

		public ChatMessage() {}
	}
}