using System;

namespace Rlc.Monitor.Messages
{
	[Serializable]
	public class RegisterMessage : BaseMessage
	{
		public string ComputerName { get; set; }
		public string UserInfo { get; set; }

		public RegisterMessage() {}
	}
}