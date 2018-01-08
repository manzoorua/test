using System;
using System.Collections.Generic;
using Rlc.Monitor.Messages;

namespace Rlc.Monitor
{
	public class DataRepository
	{
		private readonly Dictionary<Guid, AgentData> _agents = new Dictionary<Guid, AgentData>();
		private readonly List<ChatMessage> _chatContent = new List<ChatMessage>();


		public Dictionary<Guid, AgentData> Agents { get { return _agents; } }
		public List<ChatMessage> ChatContent { get { return _chatContent; } }
	}
}
