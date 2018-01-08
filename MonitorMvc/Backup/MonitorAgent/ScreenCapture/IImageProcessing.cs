using System.Drawing;

namespace MonitorAgent.ScreenCapture
{
	public interface IImageProcessing
	{
		/// <summary>
		/// Gets the bounding box for changed pixels with respect
		/// to the new image when compared to the previous. This
		/// bounding box can then be applied to the new image
		/// to crop out the pixels that have not changed.
		/// </summary>
		/// <returns></returns>
		Rectangle GetBoundingBoxForChanges(Bitmap previousBitmap, Bitmap newBitmap);
	}
}