using System;
using System.Web.Mvc;
using MonitorMvc.Models;
using System.IO;
using Rlc.Monitor;
using MonitorAgent.WebService;

namespace MonitorMvc.Controllers
{
    public class BrowseController : Controller
    {
        //
        // GET: /Browse/

        public ActionResult Index()
        {
			HomeViewModel hvm = new HomeViewModel();
			return View(hvm);
		}

		public ActionResult Browse(Guid agentId)
		{
			// Request the folder data from the computer.
			//
			AgentData agentData = ModelHelpers.FindAgent(agentId);

			lock (agentData)
			{
				// Don't send any more images...only poll for commands.
				//
				PollModeMessage pmm = new PollModeMessage(agentData, PollModeMessage.PollModeOptions.PollForCommands);
				agentData.MessagesToDeliver.Add(pmm);

				// Change the polling rate.
				//
				PollIntervalMessage pim = new PollIntervalMessage(agentData, 500);
				agentData.MessagesToDeliver.Add(pim);

				// Fetch the data for the current directory.
				//
				DirectoryCommandMessage dcm = new DirectoryCommandMessage(agentData)
				                              	{
				                              		Command = DirectoryCommandMessage.Action.FetchCurrentData
				                              	};
				agentData.MessagesToDeliver.Add(dcm);

				ModelHelpers.SaveAgent(agentData);
			}

			return View(agentData);
		}

		public string GetCurrentDirectory(Guid agentId)
		{
			if(Request.IsAjaxRequest())
			{
				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				string currentDirectory;
				lock (agentData)
				{
					currentDirectory = agentData.CurrentDirectory;
				}

				return currentDirectory;
			}
			return "";
		}

		public ActionResult GetDirectoryNav(Guid agentId)
		{
			if (Request.IsAjaxRequest())
			{
				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				return View("DirectoryNav", agentData);
			}
			return new EmptyResult();
		}

		public ActionResult GetDirectoryInfo(Guid agentId)
		{
			if (Request.IsAjaxRequest())
			{
				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				return View("DirectoryInfo", agentData);
			}
			return new EmptyResult();
		}

		public void ChangeDirectory(Guid agentId, string folder)
		{
			if (Request.IsAjaxRequest())
			{
				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				lock (agentData)
				{
					// Fetch the data for the current directory.
					//
					DirectoryCommandMessage dcm = new DirectoryCommandMessage(agentData)
					                              	{
					                              		Command = DirectoryCommandMessage.Action.ChangeDirectory,
					                              		CurrentDirectory = folder
					                              	};
					agentData.MessagesToDeliver.Add(dcm);

					ModelHelpers.SaveAgent(agentData);
				}
			}
		}

		public void RequestFiles(Guid agentId, string files)
		{
			if (Request.IsAjaxRequest())
			{
				string[] content = files.Split(';');

				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				lock (agentData)
				{
                    // Fetch the data for the current directory.
                    //
                    DirectoryCommandMessage dcm = new DirectoryCommandMessage(agentData)
                    {
                        Command = DirectoryCommandMessage.Action.FetchContent,
                        ContentToFetch = content
					                              	};
					agentData.MessagesToDeliver.Add(dcm);

					ModelHelpers.SaveAgent(agentData);
				}
			}
		}

		public ActionResult GetPackageList(Guid agentId)
		{
			if (Request.IsAjaxRequest())
			{
				// Request the folder data from the computer.
				//
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				return View("PackageInfo", agentData);
			}
			return new EmptyResult();
		}

		public FileResult DownloadPackage(Guid agentId)
		{
			// Request the folder data from the computer.
			//
			AgentData agentData = ModelHelpers.FindAgent(agentId);

			FileStreamResult fsr;

			lock (agentData)
			{
				MemoryStream memoryStream = new MemoryStream(agentData.FileDataMessage.FileData);
				fsr = new FileStreamResult(memoryStream, "application/zip")
				      	{
				      		FileDownloadName = agentData.FileDataMessage.FileName
				      	};

				agentData.FileDataMessage = null;
			}

			return fsr;
		}
    }
}
