using System.Collections.Generic;
using Cravens.Utilities.Plugin;

namespace Rlc.Monitor.Packages.Server
{
	public interface IServerMessageProcessor: IPlugin
	{
		List<Message> Process(Message message);
	}
}