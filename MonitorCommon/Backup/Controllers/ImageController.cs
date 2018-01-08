using System;
using System.Web.Mvc;
using MonitorMvc.Models;
using System.Drawing;
using System.IO;
using Rlc.Monitor;
using Rlc.Monitor.Messages;
using System.Drawing.Imaging;

namespace MonitorMvc.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/

        public ActionResult Thumb(Guid agentId)
        {
			if (ModelHelpers.Data.Agents.ContainsKey(agentId))
			{
				AgentData agentData = ModelHelpers.FindAgent(agentId);
				Bitmap bitmap = null;
				if (agentData.ThumbNail != null)
				{
					lock (agentData.ThumbNail)
					{
						bitmap = new Bitmap(agentData.ThumbNail);
					}
				}
				if (bitmap == null)
				{
					return new EmptyResult();
				}

				MemoryStream ms = new MemoryStream();

				ImageCodecInfo jpgEncoder = null;
				ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
				foreach (ImageCodecInfo codec in codecs)
				{
					if(codec.FormatID == ImageFormat.Jpeg.Guid)
					{
						jpgEncoder = codec;
						break;
					}
				}
				if (jpgEncoder != null)
				{
					Encoder encoder = Encoder.Quality;
					EncoderParameters encoderParameters = new EncoderParameters(1);
					EncoderParameter encoderParameter = new EncoderParameter(encoder, 50L);
					encoderParameters.Param[0] = encoderParameter;

					bitmap.Save(ms, jpgEncoder, encoderParameters);


					ms.Seek(0, SeekOrigin.Begin);
					FileStreamResult fileStreamResult = new FileStreamResult(ms, "image/jpeg");
					return fileStreamResult;
				}
			}
			return null;
        }

		public JsonResult DetailsMode(Guid agentId)
		{
			AgentData agentData = ModelHelpers.FindAgent(agentId);

			lock (agentData)
			{
				PollModeMessage pollMode = new PollModeMessage(agentData, PollModeMessage.PollModeOptions.PostFullsize);
				PollIntervalMessage pollRate = new PollIntervalMessage(agentData, 1000);

				agentData.MessagesToDeliver.Add(pollMode);
				agentData.MessagesToDeliver.Add(pollRate);

				ModelHelpers.SaveAgent(agentData);
			}

			return Json(true);
		}

		public JsonResult ThumbMode(Guid agentId)
		{
			AgentData agentData = ModelHelpers.FindAgent(agentId);

			lock (agentData)
			{
				PollModeMessage pollMode = new PollModeMessage(agentData, PollModeMessage.PollModeOptions.PostThumbnail);
				PollIntervalMessage pollRate = new PollIntervalMessage(agentData, 10000);

				agentData.MessagesToDeliver.Add(pollMode);
				agentData.MessagesToDeliver.Add(pollRate);

				ModelHelpers.SaveAgent(agentData);
			}

			return Json(true);
		}

		public ActionResult ThumbHtml(Guid agentId)
		{
			if (ModelHelpers.Data.Agents.ContainsKey(agentId))
			{
				AgentData agentData = ModelHelpers.FindAgent(agentId);

				return View("ComputerThumbView", agentData);
			}

			return null;
		}
    }
}
