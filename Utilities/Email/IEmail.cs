namespace Cravens.Utilities.Email
{
	public interface IEmail
	{
		string LastErrorMessage { get; }
		bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string toAddress);
		bool Send(string fromAddress, string fromName, string subject, string body, bool isHtml, string[] toAddresses);
	}
}
