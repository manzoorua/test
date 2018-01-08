using System.Collections.Generic;

namespace Cravens.Utilities.Email
{
	public class EmailFake : IEmail
	{
		public class EmailData
		{
			public string FromAddress { get; set; }
			public string FromName { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public bool IsHtml { get; set; }
			public string[] ToAddresses { get; set; }
		}
		public List<EmailData> Emails { get; set; }

		public string LastErrorMessage { get { return ""; } }

		public EmailFake()
		{
			Emails = new List<EmailData>();
		}

		public bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string toAddress)
		{
			return Send(fromAddress, fromName, subject, body, isHtml, new[] { toAddress });
		}

		public bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string[] toAddresses)
		{
			EmailData emailData = new EmailData
			                      	{
			                      		FromAddress = fromAddress,
			                      		FromName = fromName,
			                      		Subject = subject,
			                      		Body = body,
			                      		IsHtml = isHtml,
			                      		ToAddresses = toAddresses
			                      	};
			Emails.Add(emailData);
			return true;
		}
	}
}