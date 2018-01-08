using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rlc.Monitor;
using System.Collections.ObjectModel;
using Rlc.Monitor.Messages;

namespace MonitorMvc.Models
{
	public static class ModelHelpers
	{
		public static DataRepository Data
		{
			get
			{
				DataRepository data = (DataRepository)HttpContext.Current.Cache["Data"];
				if (data == null)
				{
					data = new DataRepository();
					HttpContext.Current.Cache["Data"] = data;
				}

				return data;
			}
		}

		public static void ClearData()
		{
			HttpContext.Current.Cache.Remove("Data");
		}

		public static AgentData FindAgent(Guid agentId)
		{
			AgentData agentData;
			if (Data.Agents.ContainsKey(agentId))
			{
				agentData = Data.Agents[agentId];
			}
			else
			{
				agentData = new AgentData {AgentId = agentId, UserName = "Unknown", ComputerName = "Unknown"};

				// Force a registration
				//
				agentData.MessagesToDeliver.Add(new PollModeMessage(agentData, PollModeMessage.PollModeOptions.Register));
			}
			return agentData;
		}

		public static void SaveAgent(AgentData agent)
		{
			Data.Agents[agent.AgentId] = agent;
		}

		public static void AddChatMessage(ChatMessage message)
		{
			// Add to the global chat record.
			//
			Data.ChatContent.Add(message);

			// Add to every agent's message queue
			//
			foreach (AgentData agent in Agents)
			{
				lock (agent)
				{
					agent.MessagesToDeliver.Add(message);
					SaveAgent(agent);
				}
			}
		}

		public static void ClearChatConent()
		{
			Data.ChatContent.Clear();
		}

		public static ReadOnlyCollection<AgentData> Agents
		{
			get
			{
				List<AgentData> result = Data.Agents.Select(x => x.Value).ToList();

				return result.AsReadOnly();
			}
		}

		public static ReadOnlyCollection<ChatMessage> Chats
		{
			get
			{
				return Data.ChatContent.AsReadOnly();
			}
		}

	}
}
