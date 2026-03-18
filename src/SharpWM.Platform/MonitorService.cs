using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SharpWM.Common;

namespace SharpWM.Platform;

[SupportedOSPlatform("windows5.0")]
public static class MonitorService
{
    private delegate bool MonitorEnumProc(nint hMonitor, nint hdcMonitor, nint lprcMonitor, nint dwData);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int left, top, right, bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO_SIMPLE
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    private const uint MONITORINFOF_PRIMARY = 0x00000001;

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(nint hdc, nint lprcClip, MonitorEnumProc lpfnEnum, nint dwData);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfoW(nint hMonitor, ref MONITORINFO_SIMPLE lpmi);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetMonitorInfoW")]
    private static extern bool GetMonitorInfoExW(nint hMonitor, nint lpmi);

    public static IReadOnlyList<MonitorContainer> EnumerateMonitors()
    {
        var monitors = new List<MonitorContainer>();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, _, _, _) =>
        {
            var info = new MONITORINFO_SIMPLE
            {
                cbSize = (uint)Marshal.SizeOf<MONITORINFO_SIMPLE>()
            };

            if (!GetMonitorInfoW(hMonitor, ref info))
                return true;

            var rc = info.rcMonitor;
            monitors.Add(new MonitorContainer
            {
                DeviceName = GetDeviceName(hMonitor),
                Bounds     = new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top),
                IsPrimary  = (info.dwFlags & MONITORINFOF_PRIMARY) != 0
            });

            return true;
        }, IntPtr.Zero);

        return monitors.AsReadOnly();
    }

    private static string GetDeviceName(nint hMonitor)
    {
        // Layout MONITORINFOEXW: cbSize(4) + rcMonitor(16) + rcWork(16) + dwFlags(4) + szDevice(32 wchar = 64 byte) = 104
        const int structSize = 104;
        nint buf = Marshal.AllocHGlobal(structSize);
        try
        {
            Marshal.WriteInt32(buf, structSize);
            if (!GetMonitorInfoExW(hMonitor, buf))
                return string.Empty;

            return Marshal.PtrToStringUni(buf + 40) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }
}