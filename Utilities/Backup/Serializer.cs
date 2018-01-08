using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rlc.Utilities
{
	public static class Serializer
	{
		public static byte[] ConvertToByteArray(object obj)
		{
			byte[] result = null;
			if (obj != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, obj);
					result = memoryStream.ToArray();
				}
			}

			return result;
		}

		public static T ConvertToObject<T>(byte[] bytes) where T:class
		{
			T instance;
			using(MemoryStream memoryStream = new MemoryStream(bytes, false))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memoryStream.Seek(0, SeekOrigin.Begin);
				instance = (T)binaryFormatter.Deserialize(memoryStream);
			}

			return instance;
		}
	}
}