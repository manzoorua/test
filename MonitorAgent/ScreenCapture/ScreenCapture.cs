using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MonitorAgent.ScreenCapture
{
	public class ScreenCapture : IScreenCapture
	{
		private struct Size
		{
			public int Cx;
			public int Cy;
		}

		/// <summary>
		/// Captures the desktop (uses GDI Interop)
		/// </summary>
		/// <returns></returns>
		public Bitmap CaptureDesktop()
		{
			Bitmap bmp = null;
			IntPtr hDC = IntPtr.Zero;
			try
			{
				Size size;
				hDC = Win32Stuff.GetDC(Win32Stuff.GetDesktopWindow());
				IntPtr hMemDC = GdiStuff.CreateCompatibleDC(hDC);

				size.Cx = Win32Stuff.GetSystemMetrics
					(Win32Stuff.SmCxscreen);

				size.Cy = Win32Stuff.GetSystemMetrics
					(Win32Stuff.SmCyscreen);

				IntPtr hBitmap = GdiStuff.CreateCompatibleBitmap(hDC, size.Cx, size.Cy);

				if (hBitmap != IntPtr.Zero)
				{
					IntPtr hOld = GdiStuff.SelectObject
						(hMemDC, hBitmap);

					GdiStuff.BitBlt(hMemDC, 0, 0, size.Cx, size.Cy, hDC,
					                0, 0, GdiStuff.SrcCopy);

					GdiStuff.SelectObject(hMemDC, hOld);
					GdiStuff.DeleteDC(hMemDC);
					bmp = Image.FromHbitmap(hBitmap);
					GdiStuff.DeleteObject(hBitmap);
					GC.Collect();
				}
			}
			finally
			{
				if (hDC != IntPtr.Zero)
				{
					Win32Stuff.ReleaseDC(Win32Stuff.GetDesktopWindow(), hDC);
				}
			}
			return bmp;
		}

		/// <summary>
		/// Captures the cursor (uses GDI Interop)
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		public Bitmap CaptureCursor(ref int x, ref int y)
		{
			Bitmap bmp = null;
			Win32Stuff.CursorInfo ci = new Win32Stuff.CursorInfo();
			Win32Stuff.IconInfo icInfo;
			ci.cbSize = Marshal.SizeOf(ci);
			if (Win32Stuff.GetCursorInfo(out ci))
			{
				if (ci.flags == Win32Stuff.CursorShowing)
				{
					IntPtr hicon = Win32Stuff.CopyIcon(ci.hCursor);
					if (Win32Stuff.GetIconInfo(hicon, out icInfo))
					{
						if (icInfo.hbmMask != IntPtr.Zero)
						{
							GdiStuff.DeleteObject(icInfo.hbmMask);
						}
						if (icInfo.hbmColor != IntPtr.Zero)
						{
							GdiStuff.DeleteObject(icInfo.hbmColor);
						}
						x = ci.ptScreenPos.x - icInfo.xHotspot;
						y = ci.ptScreenPos.y - icInfo.yHotspot;

						Icon ic = Icon.FromHandle(hicon);
						if (ic.Width > 0 && ic.Height > 0)
						{
							bmp = ic.ToBitmap();
						}
						Win32Stuff.DestroyIcon(hicon);
					}
				}
			}

			return bmp;
		}

		/// <summary>
		/// Captures the desktop with cursor (uses GDI Interop)
		/// </summary>
		/// <returns></returns>
		public Bitmap CaptureDesktopWithCursor()
		{
			int cursorX = 0;
			int cursorY = 0;
			Graphics g;

			Bitmap desktopBmp = CaptureDesktop();
			Bitmap cursorBmp = CaptureCursor(ref cursorX, ref cursorY);
			if (desktopBmp != null)
			{
				if (cursorBmp != null)
				{
					Rectangle r = new Rectangle(cursorX, cursorY, cursorBmp.Width, cursorBmp.Height);
					g = Graphics.FromImage(desktopBmp);
					g.DrawImage(cursorBmp, r);
					g.Flush();

					return desktopBmp;
				}
				return desktopBmp;
			}

			return null;

		}
	}
}