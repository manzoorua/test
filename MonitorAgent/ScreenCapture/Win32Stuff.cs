using System;
using System.Runtime.InteropServices;


namespace MonitorAgent.ScreenCapture
{
	public static class Win32Stuff
	{
		#region Class Variables
		public const int SmCxscreen = 0;
		public const int SmCyscreen = 1;

		public const Int32 CursorShowing = 0x00000001;
        
		[StructLayout(LayoutKind.Sequential)]
		public struct IconInfo
		{
			public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
			public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
			public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
			public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
			public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public Int32 x;
			public Int32 y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CursorInfo
		{
			public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
			public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
			public IntPtr hCursor;          // Handle to the cursor. 
			public Point ptScreenPos;       // A Point structure that receives the screen coordinates of the cursor. 
		}

		#endregion

		#region Class Functions

		[DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", EntryPoint = "GetDC")]
		public static extern IntPtr GetDC(IntPtr ptr);

		[DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
		public static extern int GetSystemMetrics(int abc);

		[DllImport("user32.dll", EntryPoint = "GetWindowDC")]
		public static extern IntPtr GetWindowDC(Int32 ptr);

		[DllImport("user32.dll", EntryPoint = "ReleaseDC")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);


		[DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
		public static extern bool GetCursorInfo(out CursorInfo pci);

		[DllImport("user32.dll", EntryPoint = "CopyIcon")]
		public static extern IntPtr CopyIcon(IntPtr hIcon);

		[DllImport("user32.dll", EntryPoint = "GetIconInfo")]
		public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

		[DllImport("user32.dll", EntryPoint = "DestroyIcon")]
		public static extern bool DestroyIcon(IntPtr hIcon);

		#endregion
	}
}