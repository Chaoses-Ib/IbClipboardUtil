using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ib.Windows.SystemServices.MemoryManagement;


namespace Ib.Windows.DataExchange
{
    static class Clipboard
    {
        public static bool Open(IntPtr hWndNewOwner = default)
        {
            return ClipboardNative.OpenClipboard(hWndNewOwner);
        }

        public static bool Close()
        {
            return ClipboardNative.CloseClipboard();
        }

        public static List<ClipboardFormat> GetFormats()
        {
            uint format = 0;
            var list = new List<ClipboardFormat>();
            while ((format = ClipboardNative.EnumClipboardFormats(format)) != 0)
            {
                list.Add(format);
            }
            return list;
        }
    }

    class ClipboardFormat
    {
        uint format;

        public ClipboardFormat(uint format) => this.format = format;

        public static implicit operator uint(ClipboardFormat f) => f.format;
        public static implicit operator ClipboardFormat(uint format) => new ClipboardFormat(format);

        public string GetName()
        {
            var buf = new StringBuilder(80);
            buf.Length = ClipboardNative.GetClipboardFormatName(format, buf, buf.Capacity);
            return buf.ToString();
        }

        public byte[] GetData()
        {
            IntPtr h = ClipboardNative.GetClipboardData(format);
            IntPtr p = GlobalNative.GlobalLock(h);
            if (p == IntPtr.Zero) return default;

            int size = (int)GlobalNative.GlobalSize(h);
            byte[] bin = new byte[size];
            Marshal.Copy(p, bin, 0, size);

            GlobalNative.GlobalUnlock(h);

            return bin;
        }
    }

    static class ClipboardNative
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder lpszFormatName, int cchMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint EnumClipboardFormats(uint format);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);
    }
}

namespace Ib.Windows.SystemServices.MemoryManagement
{
    static class GlobalNative
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);
    }
}