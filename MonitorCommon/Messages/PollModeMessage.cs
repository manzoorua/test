using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class PollModeMessage : BaseMessage
	{
		public enum PollModeOptions { Register, PollForCommands, PostThumbnail, PostFullsize };
		public PollModeOptions PollMode { get; set; }

		public PollModeMessage()
		{
		}

		public PollModeMessage(AgentData agent, PollModeOptions option)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
			PollMode = option;
		}
	}
}