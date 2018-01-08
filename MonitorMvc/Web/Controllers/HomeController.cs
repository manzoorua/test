using System.Web.Mvc;
using MonitorMvc.Models;
using Rlc.Monitor;
using Rlc.Monitor.Messages;

namespace MonitorMvc.Controllers
{
	[HandleError]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			HomeViewModel hvm = new HomeViewModel();

			// Put all the agents in thumbnail mode.
			//
			foreach (AgentData agentData in hvm.Agents)
			{
				lock (agentData)
				{
					// Send thumbnails to the server.
					//
					PollModeMessage pmm = new PollModeMessage(agentData, PollModeMessage.PollModeOptions.PostThumbnail);
					agentData.MessagesToDeliver.Add(pmm);

					// Change the polling rate.
					//
					PollIntervalMessage pim = new PollIntervalMessage(agentData, PollIntervalMessage.DefaultIntervalInMilliseconds);
					agentData.MessagesToDeliver.Add(pim);

					ModelHelpers.SaveAgent(agentData);
				}
			}
			return View(hvm);
		}

		public ActionResult ClearAgentData()
		{
			ModelHelpers.ClearData();
			return RedirectToAction("Index");
		}

		public ActionResult About()
		{	
			return View();
		}
	}
}
