using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Rlc.Utilities
{
	public static class Images
	{
		public enum SpriteDirection { Vertical, Horizontal };
		public static Bitmap CreateSprite(string[] fileNames, 
			SpriteDirection direction)
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

		public static Bitmap CreateThumbNail(string imageFile, 
			RotateFlipType rotateFlip, int thumbNailWidth, int thumbNailHeight)
		{
			// Load the full size image.
			//
			Bitmap fullSizeImg;
			try
			{
				fullSizeImg = new System.Drawing.Bitmap(imageFile);
			}
			catch (Exception ex)
			{
				// If we could not load the full size image then exit early
				//
				return null;
			}

			return CreateThumbNail(fullSizeImg, rotateFlip, thumbNailWidth, thumbNailHeight);
		}

		public static Bitmap CreateThumbNail(Bitmap fullSizeImage, RotateFlipType rotateFlip,
			int thumbNailWidth, int thumbNailHeight)
		{
			// Do the rotation / flip on the image
			//
			fullSizeImage.RotateFlip(rotateFlip);

			// Create a blank thumbnail image
			//
			Bitmap thumbNailImg = new Bitmap(fullSizeImage, 
				thumbNailWidth, thumbNailHeight);

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
					thumbNailWidth, thumbNailHeight);

				// Give a thumbnail size, the following code trys to 
				//	maximize the pixels that are used to generate the 
				//	thumbnail at the same time keeping the aspect 
				//	ratio the same.
				//
				int viewPortWidth = fullSizeImage.Width;
				int viewPortHeight = fullSizeImage.Height;
				double alpha = (double)fullSizeImage.Width / (double)thumbNailWidth;
				double heightTest = alpha * (double)thumbNailHeight;
				if (heightTest > fullSizeImage.Height)
				{
					// Not enough pixels to scale width full size
					//	...center horizontally
					viewPortWidth = (int)((double)thumbNailWidth *
						(double)fullSizeImage.Height / (double)thumbNailHeight);
				}
				else
				{
					// Not enough pixels to scale the height full size
					//	...center vertically
					viewPortHeight = (int)((double)thumbNailHeight * alpha);
				}
				Rectangle source = new Rectangle(
					(fullSizeImage.Width - viewPortWidth) / 2,
					(fullSizeImage.Height - viewPortHeight) / 2,
					viewPortWidth,
					viewPortHeight);

				// Draw the selected portion of the original onto the 
				//	thumbnail surface. This will scale the image.
				//
				g.DrawImage(fullSizeImage, destination, source, GraphicsUnit.Pixel);
			}

			return thumbNailImg;
		}

		/// <summary>
		/// Gets the bounding box for changed pixels with respect
		/// to the new image when compared to the previous. This
		/// bounding box can then be applied to the new image
		/// to crop out the pixels that have not changed.
		/// </summary>
		/// <returns></returns>
/*		public static Rectangle GetBoundingBoxForChanges(Bitmap previousBitmap, Bitmap newBitmap)
		{
			// The search algorithm starts by looking
			//	for the top and left bounds. The search
			//	starts in the upper-left corner and scans
			//	left to right and then top to bottom. It uses
			//	an adaptive approach on the pixels it
			//	searches. Another pass is looks for the
			//	lower and right bounds. The search starts
			//	in the lower-right corner and scans right
			//	to left and then bottom to top. Again, an
			//	adaptive approach on the search area is used.
			//

			// Note: The GetPixel member of the Bitmap class
			//	is too slow for this purpose. This is a good
			//	case of using unsafe code to access pointers
			//	to increase the speed.
			//

			// If the previous image does not exist, then
			//	in essence everything has changed.
			//
			if (previousBitmap == null)
			{
				return new Rectangle(0, 0, newBitmap.Width, newBitmap.Height);
			}

			// Validate the images are the same shape and type.
			//	If they are not the same size, then in essence
			//	everything has changed.
			//
			if (previousBitmap.Width != newBitmap.Width ||
				previousBitmap.Height != newBitmap.Height ||
				previousBitmap.PixelFormat != newBitmap.PixelFormat)
			{
				// Not the same shape...can't do the search.
				//
				return new Rectangle(0, 0, newBitmap.Width, newBitmap.Height);
			}

			// Init the search parameters.
			//
			int width, height;
			lock (newBitmap)
			{
				width = newBitmap.Width;
				height = newBitmap.Height;
			}
			int left = width;
			int right = 0;
			int top = height;
			int bottom = 0;

			BitmapData bmNewData = null;
			BitmapData bmPrevData = null;
			try
			{
				// Lock the bits into memory.
				//
				lock (newBitmap)
				{
					bmNewData = newBitmap.LockBits(
						new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
						ImageLockMode.ReadOnly, newBitmap.PixelFormat);
				}
				lock (previousBitmap)
				{
					bmPrevData = previousBitmap.LockBits(
						new Rectangle(0, 0, previousBitmap.Width, previousBitmap.Height),
						ImageLockMode.ReadOnly, previousBitmap.PixelFormat);
				}

				// The images are ARGB (4 bytes)
				//
				int numBytesPerPixel = 4;

				// Get the number of integers (4 bytes) in each row
				//	of the image.
				//
				int strideNew = bmNewData.Stride / numBytesPerPixel;
				int stridePrev = bmPrevData.Stride / numBytesPerPixel;

				// Get a pointer to the first pixel.
				//
				// Note: Another speed up implemented is that I don't
				//	need the ARGB elements. I am only trying to detect
				//	change. So this algorithm reads the 4 bytes as an
				//	integer and compares the two numbers.
				//
				System.IntPtr scanNew0 = bmNewData.Scan0;
				System.IntPtr scanPrev0 = bmPrevData.Scan0;

				// Enter the unsafe code.
				//
				unsafe
				{
					// Cast the safe pointers into unsafe pointers.
					//
					int* pNew = (int*)(void*)scanNew0;
					int* pPrev = (int*)(void*)scanPrev0;

					// First Pass - Find the left and top bounds
					//	of the minimum bounding rectangle. Adapt the
					//	number of pixels scanned from left to right so
					//	we only scan up to the current bound. We also
					//	initialize the bottom & right. This helps optimize
					//	the second pass.
					//
					// For all rows of pixels (top to bottom)
					//
					for (int y = 0; y < height; ++y)
					{
						// For pixels up to the current bound (left to right)
						//
						for (int x = 0; x < left; ++x)
						{
							// Use pointer arithmetic to index the
							//	next pixel in this row.
							//
							if ((pNew + x)[0] != (pPrev + x)[0])
							{
								// Found a change.
								//
								if (x < left)
								{
									left = x;
								}
								if (x > right)
								{
									right = x;
								}
								if (y < top)
								{
									top = y;
								}
								if (y > bottom)
								{
									bottom = y;
								}
							}
						}

						// Move the pointers to the next row.
						//
						pNew += strideNew;
						pPrev += stridePrev;
					}

					// If we did not find any changed pixels
					//	then no need to do a second pass.
					//
					if (left != width)
					{
						// Second Pass - The first pass found at
						//	least one different pixel and has set
						//	the left & top bounds. In addition, the
						//	right & bottom bounds have been initialized.
						//	Adapt the number of pixels scanned from right
						//	to left so we only scan up to the current bound.
						//	In addition, there is no need to scan past
						//	the top bound.
						//

						// Set the pointers to the first element of the
						//	bottom row.
						//
						pNew = (int*)(void*)scanNew0;
						pPrev = (int*)(void*)scanPrev0;
						pNew += (height - 1) * strideNew;
						pPrev += (height - 1) * stridePrev;

						// For each row (bottom to top)
						//
						for (int y = height - 1; y > top; y--)
						{
							// For each column (right to left)
							//
							for (int x = width - 1; x > right; x--)
							{
								// Use pointer arithmetic to index the
								//	next pixel in this row.
								//
								if ((pNew + x)[0] != (pPrev + x)[0])
								{
									// Found a change.
									//
									if (x > right)
									{
										right = x;
									}
									if (y > bottom)
									{
										bottom = y;
									}
								}
							}

							// Move up one row.
							//
							pNew -= strideNew;
							pPrev -= stridePrev;
						}
					}
				}
			}
			catch (Exception ex)
			{
				int xxx = 0;
			}
			finally
			{
				// Unlock the bits of the image.
				//
				if (bmNewData != null)
				{
					newBitmap.UnlockBits(bmNewData);
				}
				if (bmPrevData != null)
				{
					previousBitmap.UnlockBits(bmPrevData);
				}
			}

			// Validate we found a bounding box. If not
			//	return an empty rectangle.
			//
			int diffImgWidth = right - left + 1;
			int diffImgHeight = bottom - top + 1;
			if (diffImgHeight < 0 || diffImgWidth < 0)
			{
				// Nothing changed
				return Rectangle.Empty;
			}

			// Return the bounding box.
			//
			return new Rectangle(left, top, diffImgWidth, diffImgHeight);
		}*/

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

		public static byte[] ConvertToByteArray(this Bitmap image)
		{
			// Get the image data.
			// Note: Not using JPEG becase we need the 
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
				image = Bitmap.FromStream(ms, true);
			}
			return image as Bitmap;
		}

		public static Bitmap Resize(this Bitmap original, RotateFlipType rotateFlip,
			int thumbNailWidth, int thumbNailHeight)
		{
			// Do the rotation / flip on the image
			//
			original.RotateFlip(rotateFlip);

			// Create a blank thumbnail image
			//
			Bitmap thumbNailImg = new Bitmap(original,
				thumbNailWidth, thumbNailHeight);

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
					thumbNailWidth, thumbNailHeight);

				// Give a thumbnail size, the following code trys to 
				//	maximize the pixels that are used to generate the 
				//	thumbnail at the same time keeping the aspect 
				//	ratio the same.
				//
				int viewPortWidth = original.Width;
				int viewPortHeight = original.Height;
				Rectangle source = new Rectangle(
					0,
					0,
					viewPortWidth,
					viewPortHeight);

				// Draw the selected portion of the original onto the 
				//	thumbnail surface. This will scale the image.
				//
				g.DrawImage(original, destination, source, GraphicsUnit.Pixel);
			}

			return thumbNailImg;
		}
	}
}
