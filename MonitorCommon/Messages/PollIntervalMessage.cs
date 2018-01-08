using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class PollIntervalMessage : BaseMessage
	{
		public static int DefaultIntervalInMilliseconds = 5000;
		public int IntervalInMilliseconds { get; set; }

		public PollIntervalMessage()
		{
			IntervalInMilliseconds = DefaultIntervalInMilliseconds;
		}

		public PollIntervalMessage(AgentData agent, int intervalInMilliseconds)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
			IntervalInMilliseconds = intervalInMilliseconds;
		}
	}
}