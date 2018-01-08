using System;
using System.Net.Mail;

namespace Cravens.Utilities.Email
{
	public class Email : IEmail
	{
		private readonly string _host;
		private readonly int _port;

		public string LastErrorMessage { get; set;}

		public Email(string host, int port)
		{
			_host = host;
			_port = port;
		}

		public bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string toAddress)
		{
			return Send(fromAddress, fromName, subject, body, isHtml, new[] {toAddress});
		}

		public bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string[] toAddresses)
		{
			bool result = true;

			MailMessage msg = new MailMessage
			                  	{
			                  		From = new MailAddress(fromAddress, fromName),
			                  		Subject = subject,
			                  		Body = body,
			                  		IsBodyHtml = isHtml
			                  	};
			msg.To.Add(string.Join(",", toAddresses));

			SmtpClient smtp = new SmtpClient(_host, _port);
			try
			{
				smtp.Send(msg);
			}
			catch (Exception ex)
			{
				LastErrorMessage = ex.ToString();
				result = false;
			}

			msg.Dispose();

			return result;
		}
	}
}