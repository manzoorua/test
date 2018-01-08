using System;
using System.Collections.Generic;

namespace Rlc.Monitor.Packages
{
	[Serializable]
	public class MessageBundle
	{
		public Guid AgentId { get; set; }

		public List<Message> Messages { get; set; }

		public MessageBundle()
		{
			Messages = new List<Message>();
		}
	}
}