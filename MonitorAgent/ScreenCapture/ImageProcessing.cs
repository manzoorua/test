using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MonitorAgent.ScreenCapture
{
	public class ImageProcessing : IImageProcessing
	{
		/// <summary>
		/// Gets the bounding box for changed pixels with respect
		/// to the new image when compared to the previous. This
		/// bounding box can then be applied to the new image
		/// to crop out the pixels that have not changed.
		/// </summary>
		/// <returns></returns>
		public Rectangle GetBoundingBoxForChanges(Bitmap previousBitmap, Bitmap newBitmap)
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

			// Notice: The GetPixel member of the Bitmap class
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
				const int numBytesPerPixel = 4;

				// Get the number of integers (4 bytes) in each row
				//	of the image.
				//
				int strideNew = bmNewData.Stride / numBytesPerPixel;
				int stridePrev = bmPrevData.Stride / numBytesPerPixel;

				// Get a pointer to the first pixel.
				//
				// Notice: Another speed up implemented is that I don't
				//	need the ARGB elements. I am only trying to detect
				//	change. So this algorithm reads the 4 bytes as an
				//	integer and compares the two numbers.
				//
				IntPtr scanNew0 = bmNewData.Scan0;
				IntPtr scanPrev0 = bmPrevData.Scan0;

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
			catch (Exception)
			{
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
		}
	}
}