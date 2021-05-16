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
            switch (format)
            {
                case 1: return "CF_TEXT";
                case 2: return "CF_BITMAP";
                case 3: return "CF_METAFILEPICT";
                case 4: return "CF_SYLK";
                case 5: return "CF_DIF";
                case 6: return "CF_TIFF";
                case 7: return "CF_OEMTEXT";
                case 8: return "CF_DIB";
                case 9: return "CF_PALETTE";
                case 10: return "CF_PENDATA";
                case 11: return "CF_RIFF";
                case 12: return "CF_WAVE";
                case 13: return "CF_UNICODETEXT";
                case 14: return "CF_ENHMETAFILE";
                case 15: return "CF_HDROP";
                case 16: return "CF_LOCALE";
                case 17: return "CF_DIBV5";
                case 0x80: return "CF_OWNERDISPLAY";
                case 0x81: return "CF_DSPTEXT";
                case 0x82: return "CF_DSPBITMAP";
                case 0x83: return "CF_DSPMETAFILEPICT";
                case 0x8E: return "CF_DSPENHMETAFILE";
                case 0x200: return "CF_PRIVATEFIRST";
                case 0x2FF: return "CF_PRIVATELAST";
                case 0x300: return "CF_GDIOBJFIRST";
                case 0x3FF: return "CF_GDIOBJLAST";
                default:
                    var buf = new StringBuilder(80);
                    buf.Length = ClipboardNative.GetClipboardFormatName(format, buf, buf.Capacity);
                    return buf.ToString();
            }
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