using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Cravens.Utilities.Images
{
	public static class ImageResize
	{
		/// <summary>
		/// Creates the thumb nail.
		/// </summary>
		/// <param name="imageFile">The image file.</param>
		/// <param name="rotateFlip">The rotate flip.</param>
		/// <param name="thumbNailWidth">Width of the thumb nail.</param>
		/// <param name="thumbNailHeight">Height of the thumb nail.</param>
		/// <returns></returns>
		public static Bitmap CreateThumbNail(	string imageFile, RotateFlipType rotateFlip, 
												int thumbNailWidth, int thumbNailHeight)
		{
			// Load the full size image.
			//
			Bitmap fullSizeImg;
			try
			{
				fullSizeImg = new Bitmap(imageFile);
			}
			catch (Exception ex)
			{
				// If we could not load the full size image then exit early
				//
				return null;
			}

			return CreateThumbNail(fullSizeImg, rotateFlip, thumbNailWidth, thumbNailHeight);
		}

		/// <summary>
		/// Creates the thumb nail of the image while keeping the aspect
		/// ratio in tact. The resulting image may be cropped.
		/// </summary>
		/// <param name="fullSizeImage">The full size image.</param>
		/// <param name="rotateFlip">The rotate flip.</param>
		/// <param name="thumbNailWidth">Width of the thumb nail.</param>
		/// <param name="thumbNailHeight">Height of the thumb nail.</param>
		/// <returns></returns>
		public static Bitmap CreateThumbNail(Bitmap fullSizeImage, RotateFlipType rotateFlip,
			int thumbNailWidth, int thumbNailHeight)
		{
			// Give a thumbnail size, the following code trys to 
			//	maximize the pixels that are used to generate the 
			//	thumbnail at the same time keeping the aspect 
			//	ratio the same.
			//
			int viewPortWidth = thumbNailWidth;
			int viewPortHeight = thumbNailHeight;
			double alpha = thumbNailWidth / (double)thumbNailWidth;
			double heightTest = alpha * thumbNailHeight;
			if (heightTest > thumbNailHeight)
			{
				// Not enough pixels to scale width full size
				//	...center horizontally
				viewPortWidth = (int)(thumbNailWidth *
									  (double)thumbNailHeight / thumbNailHeight);
			}
			else
			{
				// Not enough pixels to scale the height full size
				//	...center vertically
				viewPortHeight = (int)(thumbNailHeight * alpha);
			}
			Rectangle source = new Rectangle(
				(thumbNailWidth - viewPortWidth) / 2,
				(thumbNailHeight - viewPortHeight) / 2,
				viewPortWidth,
				viewPortHeight);

			return Resize(fullSizeImage, source, rotateFlip, thumbNailWidth, thumbNailHeight);
		}

		/// <summary>
		/// Resizes the specified original. The original aspect ratio is not kept.
		/// </summary>
		/// <param name="original">The original.</param>
		/// <param name="rotateFlip">The rotate flip.</param>
		/// <param name="newWidth">The new width.</param>
		/// <param name="newHeight">The new height.</param>
		/// <returns></returns>
		public static Bitmap Resize(this Bitmap original, RotateFlipType rotateFlip,
			int newWidth, int newHeight)
		{
			// Give a thumbnail size, the following code trys to 
			//	maximize the pixels that are used to generate the 
			//	thumbnail at the same time keeping the aspect 
			//	ratio the same.
			//
			Rectangle source = new Rectangle(
				0,
				0,
				original.Width,
				original.Height);

			return Resize(original, source, rotateFlip, newWidth, newHeight);
		}

		/// <summary>
		/// Resizes the specified original.
		/// </summary>
		/// <param name="original">The original.</param>
		/// <param name="source">The source rectangle with respect to the original.</param>
		/// <param name="rotateFlip">The rotate flip.</param>
		/// <param name="newWidth">The new width.</param>
		/// <param name="newHeight">The new height.</param>
		/// <returns></returns>
		public static Bitmap Resize(this Bitmap original, Rectangle source, RotateFlipType rotateFlip, int newWidth, int newHeight)
		{
			// Do the rotation / flip on the image
			//
			original.RotateFlip(rotateFlip);

			// Create a blank thumbnail image
			//
			Bitmap thumbNailImg = new Bitmap(original,
				newWidth, newHeight);

			// Get the GDI drawing surface from the thumbnail image.
			//
			using (Graphics g = Graphics.FromImage(thumbNailImg))
			{
				// Initialize the drawing surface.
				//
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = SmoothingMode.HighQuality;

				// Fill the image with white.
				//
				g.Clear(Color.White);

				// Create a rectangle the size of the new thumbnail
				//
				Rectangle destination = new Rectangle(0, 0,
					newWidth, newHeight);

				// Draw the selected portion of the original onto the 
				//	thumbnail surface. This will scale the image.
				//
				g.DrawImage(original, destination, source, GraphicsUnit.Pixel);
			}

			return thumbNailImg;
		}

		/// <summary>
		/// Crops the specified image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="cropBounds">The crop bounds.</param>
		/// <returns></returns>
		public static Bitmap Crop(Bitmap image, Rectangle cropBounds)
		{
			// Get the minimum rectangular area
			//
			Bitmap croppedImage = new Bitmap(cropBounds.Width, cropBounds.Height);
			using (Graphics graphics = Graphics.FromImage(croppedImage))
			{
				graphics.DrawImage(image, 0, 0, cropBounds, GraphicsUnit.Pixel);
			}
			return croppedImage;
		}
	}
}
