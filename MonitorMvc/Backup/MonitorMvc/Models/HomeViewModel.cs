using System.Collections.ObjectModel;
using Rlc.Monitor;
using Rlc.Monitor.Messages;

namespace MonitorMvc.Models
{
	public class HomeViewModel
	{
		public ReadOnlyCollection<AgentData> Agents { get; set; }
		public ReadOnlyCollection<ChatMessage> ChatContent { get; set; }

		public HomeViewModel()
		{
			Agents = ModelHelpers.Agents;
			ChatContent = ModelHelpers.Chats;
		}
	}
}
