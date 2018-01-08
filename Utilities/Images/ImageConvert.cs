using System.Drawing;
using System.IO;

namespace Cravens.Utilities.Images
{
	public static class ImageConvert
	{
		public static byte[] ConvertToByteArray(this Bitmap image)
		{
			// Get the image data.
			// Notice: Not using Png becase we need the 
			//	tranparency. Besides, this image is fairly
			//	small.
			//
			byte[] imgData;
			using (MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				imgData = ms.ToArray();
			}

			return imgData;
		}

		public static Bitmap ConvertToBitmap(byte[] data)
		{
			// Create the bitmap from the byte array.
			//
			Image image;
			using (MemoryStream ms = new MemoryStream(data, 0, data.Length))
			{
				ms.Write(data, 0, data.Length);
				image = Image.FromStream(ms, true);
			}
			return image as Bitmap;
		}
	}
}