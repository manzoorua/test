using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class FileDataMessage : BaseMessage
	{
		public string FileName { get; set; }
		public byte[] FileData { get; set; }

		public FileDataMessage() { }
	}
}