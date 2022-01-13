using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class WindowTitle
{
	[DllImport("user32.dll")]
	private static extern bool SetWindowText(IntPtr hwnd, string lpString);

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	private static IntPtr _windowHandle;

	public static void Set(string title)
	{
		if (Application.isEditor)
			return;

		if (_windowHandle == IntPtr.Zero)
			_windowHandle = GetActiveWindow();

		SetWindowText(_windowHandle, title);
	}
}