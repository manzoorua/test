using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class DirectoryDataMessage : BaseMessage
	{
		public string CurrentFolder { get; set; }
		public string[] Folders { get; set; }
		public string[] Files { get; set; }

		public DirectoryDataMessage() {}
	}
}