using System;
using System.Collections.Generic;
using System.Drawing;
using Rlc.Monitor.Messages;
using Rlc.Monitor.Packages;

namespace Rlc.Monitor
{
	public class AgentData
	{
		public Guid AgentId { get; set; }
		public string ComputerName { get; set; }
		public string UserName { get; set; }
		public Bitmap ThumbNail { get; set; }
		public DateTime LastUpdate { get; set; }

		public List<MessageBundle> PackagesToDeliver { get; set; }


		public List<BaseMessage> MessagesToDeliver { get; set; }

        public string CurrentDirectory { get; set; }
		public string[] Folders { get; set; }
		public string[] Files { get; set; }

		public FileDataMessage FileDataMessage { get; set;}

		public AgentData()
		{
			MessagesToDeliver = new List<BaseMessage>();
			PackagesToDeliver = new List<MessageBundle>();
		}
	}
}