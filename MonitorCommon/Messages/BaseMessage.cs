using System;

namespace Rlc.Monitor.Messages
{
	public class BaseMessage
	{
		public Guid AgentId { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}