using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cravens.Utilities.Images
{
	public class ImageSprite
	{
		public enum SpriteDirection { Vertical, Horizontal };
		public static Bitmap CreateSprite(string[] fileNames, SpriteDirection direction)
		{
			// First load all images to determine extents of the images
			//
			List<Bitmap> images = new List<Bitmap>();
			int maxHeight = 0;
			int maxWidth = 0;
			foreach (string fileName in fileNames)
			{
				Bitmap bitmap = new Bitmap(fileName);
				images.Add(bitmap);

				maxHeight = Math.Max(bitmap.Height, maxHeight);
				maxWidth = Math.Max(bitmap.Width, maxWidth);
			}

			// Create a bitmap to hold the final image.
			//
			int finalHeight = maxHeight;
			int finalWidth = maxWidth;
			if (direction == SpriteDirection.Horizontal)
			{
				finalWidth *= images.Count;
			}
			else
			{
				finalHeight *= images.Count;
			}
			Bitmap sprite = new Bitmap(finalWidth, finalHeight,
			                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			// Draw the bitmaps onto the images
			//
			using (Graphics g = Graphics.FromImage(sprite))
			{
				// First set the background to transparent. That way
				//	if the images are not the same size, those pixels
				//	will be transparent.
				//
				g.Clear(Color.Transparent);

				// Add each image to the sprite
				//
				int offset = 0;
				foreach (Bitmap image in images)
				{
					if (direction == SpriteDirection.Horizontal)
					{
						g.DrawImage(image, new Rectangle(offset, 0,
						                                 image.Width, image.Height));
						offset += maxWidth;
					}
					else
					{
						g.DrawImage(image, new Rectangle(0, offset,
						                                 image.Width, image.Height));
						offset += maxHeight;
					}
				}
			}

			return sprite;
		}
	}
}