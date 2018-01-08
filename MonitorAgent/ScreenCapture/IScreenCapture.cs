using System.Drawing;

namespace MonitorAgent.ScreenCapture
{
	public interface IScreenCapture
	{
		/// <summary>
		/// Captures the desktop (uses GDI Interop)
		/// </summary>
		/// <returns></returns>
		Bitmap CaptureDesktop();

		/// <summary>
		/// Captures the cursor (uses GDI Interop)
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		Bitmap CaptureCursor(ref int x, ref int y);

		/// <summary>
		/// Captures the desktop with cursor (uses GDI Interop)
		/// </summary>
		/// <returns></returns>
		Bitmap CaptureDesktopWithCursor();
	}
}