using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class DirectoryCommandMessage : BaseMessage
	{
		public enum Action {FetchCurrentData, ChangeDirectory, FetchContent, CopyContent}

		public Action Command { get; set; }

		public string CurrentDirectory { get; set; }

		public string[] ContentToFetch { get; set; }

		public DirectoryCommandMessage() {}

		public DirectoryCommandMessage(AgentData agent)
		{
			AgentId = agent.AgentId;
			TimeStamp = DateTime.Now;
		}
	}
}