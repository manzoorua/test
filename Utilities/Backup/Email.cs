using System.Web.Mail;

namespace Rlc.Utilities
{
	public static class Email
	{
		public static void Send(string messageFrom,
		                        string messageTo,
		                        string messageSubject,
		                        string messageBody)
		{
			MailMessage message = new MailMessage
			                      	{
			                      		From = messageFrom,
			                      		To = messageTo,
			                      		Subject = messageSubject,
			                      		BodyFormat = MailFormat.Text,
			                      		Body = messageBody
			                      	};

			try
			{
				SmtpMail.SmtpServer = "smtp.gmail.com";
				SmtpMail.Send(message);
			}
			catch (System.Web.HttpException exHttp)
			{
				System.Console.WriteLine("Exception occurred:" +
				                         exHttp.Message);
			}
		}
	}
}