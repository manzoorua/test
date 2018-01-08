using System.Collections.Generic;
using Cravens.Utilities.Plugin;

namespace Rlc.Monitor.Packages.Client
{
	public interface IClientMessageProcessor : IPlugin
	{
		List<Message> Process(Message package);
	}
}