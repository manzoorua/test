using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class ErrorMessage : BaseMessage
	{
		public string Info { get; set; }

		public ErrorMessage() {}
	}
}