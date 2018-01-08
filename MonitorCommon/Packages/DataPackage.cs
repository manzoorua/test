using System;
using Cravens.Utilities.General;

namespace Rlc.Monitor.Packages
{
	public class Message
	{
		// Time stamp
		//
		public DateTime TimeStamp { get; set; }

		// Which plugin.
		//
		public Guid PluginId { get; set; }

		// Generic data package (each plugin will unpack the byte array)
		//
		public byte[] Data { get; set; }

		public static Message Create(DateTime timeStamp, Guid pluginId, object data)
		{
			Message result = new Message
			                     	{
			                     		TimeStamp = timeStamp,
										PluginId = pluginId,
										Data = Serializer.ConvertToByteArray(data)
			                     	};
			return result;
		}

		public static Message Create(Guid pluginId, object data)
		{
			return Create(DateTime.Now, pluginId, data);
		}

		public static T UnpackObject<T>(Message dataPackage) where T : class
		{
			return Serializer.ConvertToObject<T>(dataPackage.Data);
		}
	}
}