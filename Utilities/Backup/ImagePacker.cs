using System;
using System.Drawing;

namespace Rlc.Utilities
{
	public static class Packer
	{
		[Serializable]
		public class ImagePackage
		{
			public Guid AgentId { get; set; }
			public Image Image { get; set; }
			public Rectangle Bounds { get; set; }
		}

		public static class Screen
		{
			public static byte[] Pack(ImagePackage imagePackage)
			{
				// Pack the image data into a byte stream to
				//	be transferred over the wire.
				//
				return Serializer.ConvertToByteArray(imagePackage);
			}

			public static ImagePackage Unpack(byte[] data)
			{
				// Unpack the data that is transferred over the wire.
				//
				return Serializer.ConvertToObject<ImagePackage>(data);

			}
/*
			public static byte[] Pack(Guid agentId, Image image, Rectangle bounds)
			{
				// Pack the image data into a byte stream to
				//	be transferred over the wire.
				//

				// Get the bytes of the Id
				//
				byte[] idData = agentId.ToByteArray();

				// Get the bytes of the image data.
				//	Notice: We are using JPEG compression.
				//
				byte[] imgData;
				using (MemoryStream ms = new MemoryStream())
				{
					image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
					imgData = ms.ToArray();
				}

				// Get the bytes that describe the bounding
				//	rectangle.
				//
				byte[] topData = BitConverter.GetBytes(bounds.Top);
				byte[] botData = BitConverter.GetBytes(bounds.Bottom);
				byte[] leftData = BitConverter.GetBytes(bounds.Left);
				byte[] rightData = BitConverter.GetBytes(bounds.Right);

				// Create the final byte stream.
				// Notice: We are streaming back both the bounding
				//	rectangle and the image data.
				//
				int sizeOfInt = topData.Length;
				byte[] result = new byte[imgData.Length + 4 * sizeOfInt + idData.Length];
				Array.Copy(topData, 0, result, 0, topData.Length);
				Array.Copy(botData, 0, result, sizeOfInt, botData.Length);
				Array.Copy(leftData, 0, result, 2 * sizeOfInt, leftData.Length);
				Array.Copy(rightData, 0, result, 3 * sizeOfInt, rightData.Length);
				Array.Copy(imgData, 0, result, 4 * sizeOfInt, imgData.Length);
				Array.Copy(idData, 0, result, 4 * sizeOfInt + imgData.Length, idData.Length);

				return result;
			}

			public static void Unpack(byte[] data, out Image image, out Rectangle bounds, out Guid agentId)
			{
				// Unpack the data that is transferred over the wire.
				//

				// Create byte arrays to hold the unpacked parts.
				//
				const int numBytesInInt = sizeof(int);
				int idLength = Guid.NewGuid().ToByteArray().Length;
				int imgLength = data.Length - 4 * numBytesInInt - idLength;
				byte[] topPosData = new byte[numBytesInInt];
				byte[] botPosData = new byte[numBytesInInt];
				byte[] leftPosData = new byte[numBytesInInt];
				byte[] rightPosData = new byte[numBytesInInt];
				byte[] imgData = new byte[imgLength];
				byte[] idData = new byte[idLength];

				// Fill the byte arrays.
				//
				Array.Copy(data, 0, topPosData, 0, numBytesInInt);
				Array.Copy(data, numBytesInInt, botPosData, 0, numBytesInInt);
				Array.Copy(data, 2 * numBytesInInt, leftPosData, 0, numBytesInInt);
				Array.Copy(data, 3 * numBytesInInt, rightPosData, 0, numBytesInInt);
				Array.Copy(data, 4 * numBytesInInt, imgData, 0, imgLength);
				Array.Copy(data, 4 * numBytesInInt + imgLength, idData, 0, idLength);

				// Create the bitmap from the byte array.
				//
				MemoryStream ms = new MemoryStream(imgData, 0, imgData.Length);
				ms.Write(imgData, 0, imgData.Length);
				image = (Bitmap)Image.FromStream(ms, true);

				// Create the bound rectangle.
				//
				int top = BitConverter.ToInt32(topPosData, 0);
				int bot = BitConverter.ToInt32(botPosData, 0);
				int left = BitConverter.ToInt32(leftPosData, 0);
				int right = BitConverter.ToInt32(rightPosData, 0);
				int width = right - left + 1;
				int height = bot - top + 1;
				bounds = new Rectangle(left, top, width, height);

				// Create a Guid
				//
				agentId = new Guid(idData);
			}
 */
		}

		[Serializable]
		public class CursorPackage
		{
			public Guid AgentId { get; set; }
			public Image Image { get; set; }
			public int CursorX { get; set; }
			public int CursorY { get; set; }
		}

		public static class Cursor
		{
			public static byte[] Pack(CursorPackage cursorPackage)
			{
				// Pack the image data into a byte stream to
				//	be transferred over the wire.
				//
				return Serializer.ConvertToByteArray(cursorPackage);
			}

			public static CursorPackage Unpack(byte[] data)
			{
				// Unpack the data that is transferred over the wire.
				//
				return Serializer.ConvertToObject<CursorPackage>(data);
			}

/*			public static byte[] Pack(Guid agentId, Image image, int cursorX, int cursorY)
			{
				// Pack the image data into a byte stream to
				//	be transferred over the wire.
				//

				// Get the bytes of the Id
				//
				byte[] idData = agentId.ToByteArray();

				// Get the image data.
				// Notice: Not using JPEG becase we need the 
				//	tranparency. Besides, this image is fairly
				//	small.
				//
				byte[] imgData;
				using (MemoryStream ms = new MemoryStream())
				{
					image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					imgData = ms.ToArray();
				}

				// Get the x,y bytes.
				//
				byte[] xPosData = BitConverter.GetBytes(cursorX);
				byte[] yPosData = BitConverter.GetBytes(cursorY);

				// Create the resulting byte array.
				//
				byte[] result = new byte[imgData.Length + xPosData.Length + yPosData.Length + idData.Length];
				Array.Copy(xPosData, 0, result, 0, xPosData.Length);
				Array.Copy(yPosData, 0, result, xPosData.Length, yPosData.Length);
				Array.Copy(imgData, 0, result, xPosData.Length + yPosData.Length, imgData.Length);
				Array.Copy(idData, 0, result, xPosData.Length + yPosData.Length + imgData.Length, idData.Length);

				return result;
			}

			public static void Unpack(byte[] data, out Image image, out int cursorX, out int cursorY, out Guid agentId)
			{
				// Unpack the data that is transferred over the wire.
				//

				// Create byte arrays to hold the unpacked parts.
				//
				const int numBytesInInt = sizeof(int);
				int idLength = Guid.NewGuid().ToByteArray().Length;
				int imgLength = data.Length - 2 * numBytesInInt - idLength;
				byte[] xPosData = new byte[numBytesInInt];
				byte[] yPosData = new byte[numBytesInInt];
				byte[] imgData = new byte[imgLength];
				byte[] idData = new byte[idLength];

				// Fill the byte arrays.
				//
				Array.Copy(data, 0, xPosData, 0, numBytesInInt);
				Array.Copy(data, numBytesInInt, yPosData, 0, numBytesInInt);
				Array.Copy(data, 2 * numBytesInInt, imgData, 0, imgLength);
				Array.Copy(data, 2 * numBytesInInt + imgLength, idData, 0, idLength);

				// Create the cursor position x,y values.
				//
				cursorX = BitConverter.ToInt32(xPosData, 0);
				cursorY = BitConverter.ToInt32(yPosData, 0);

				// Create the bitmap from the byte array.
				//
				MemoryStream ms = new MemoryStream(imgData, 0, imgData.Length);
				ms.Write(imgData, 0, imgData.Length);
				image = Image.FromStream(ms, true);

				// Create a Guid
				//
				agentId = new Guid(idData);
			}*/
		}
	}
}