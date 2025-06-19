using System.Runtime.InteropServices;

namespace Mosico;

internal static class WinAPI
{
    public static class GWL
    {
        public static readonly int
            WNDPROC = -4,
            HINSTANCE = -6,
            HWNDPARENT = -8,
            STYLE = -16,
            EXSTYLE = -20,
            USERDATA = -21,
            ID = -12;
    }

    public static class WS_EX
    {
        public static readonly int
            ACCEPTFILES = 0x00000010,
            APPWINDOW = 0x00040000,
            CLIENTEDGE = 0x00000200,
            COMPOSITED = 0x02000000,
            CONTEXTHELP = 0x00000400,
            CONTROLPARENT = 0x00010000,
            DLGMODALFRAME = 0x00000001,
            LAYERED = 0x00080000,
            LAYOUTRTL = 0x00400000,
            LEFT = 0x00000000,
            LEFTSCROLLBAR = 0x00004000,
            LTRREADING = 0x00000000,
            MDICHILD = 0x00000040,
            NOACTIVATE = 0x08000000,
            NOINHERITLAYOUT = 0x00100000,
            NOPARENTNOTIFY = 0x00000004,
            OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
            PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
            RIGHT = 0x00001000,
            RIGHTSCROLLBAR = 0x00000000,
            RTLREADING = 0x00002000,
            STATICEDGE = 0x00020000,
            TOOLWINDOW = 0x00000080,
            TOPMOST = 0x00000008,
            TRANSPARENT = 0x00000020,
            WINDOWEDGE = 0x00000100;
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    // This static method is required because Win32 does not support
    // GetWindowLongPtr directly
    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 8)
            return GetWindowLongPtr64(hWnd, nIndex);
        else
            return GetWindowLongPtr32(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    // This helper static method is required because the 32-bit version of user32.dll does not contain this API
    // (on any versions of Windows), so linking the method will fail at run-time. The bridge dispatches the request
    // to the correct function (GetWindowLong in 32-bit mode and GetWindowLongPtr in 64-bit mode)
    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }
}
