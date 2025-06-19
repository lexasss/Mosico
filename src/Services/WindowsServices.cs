using System.Windows;
using System.Windows.Interop;

namespace Mosico.Services;

internal static class WindowsServices
{
    public static void SetWindowTransparent(Window window)
    {
        IntPtr hwnd = new WindowInteropHelper(window).Handle;
        var extendedStyle = WinAPI.GetWindowLongPtr(hwnd, WinAPI.GWL.EXSTYLE);
        WinAPI.SetWindowLongPtr(hwnd, WinAPI.GWL.EXSTYLE, extendedStyle | WinAPI.WS_EX.TRANSPARENT);
    }
}
