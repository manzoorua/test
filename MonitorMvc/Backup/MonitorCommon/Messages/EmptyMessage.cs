using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class EmptyMessage : BaseMessage
	{
		public EmptyMessage() { }

		public EmptyMessage(AgentData agent)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
		}
	}
}