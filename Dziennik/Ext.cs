using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Media;
using System.Windows;
using System.Windows.Interop;

namespace Dziennik
{
    public static class Ext
    {
        public static string ValueOrDefault(this XElement element, string def)
        {
            return (element.Value == null ? def : element.Value);
        }

        public static bool BoolParseOrDefault(string input, bool def)
        {
            bool output;
            if (bool.TryParse(input, out output))
            {
                return output;
            }
            return def;
        }
        public static int IntParseOrDefault(string input, int def)
        {
            int output;
            if (int.TryParse(input, out output))
            {
                return output;
            }
            return def;
        }
        public static long LongParseOrDefault(string input, long def)
        {
            long output;
            if (long.TryParse(input, out output))
            {
                return output;
            }
            return def;
        }

        public static string RemoveAllWhitespaces(string value)
        {
            if (value == null) return value;

            value = value.Replace(" ", "");
            value = value.Replace("\t", "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");

            return value;
        }

        public static void ClearDirectory(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (var file in info.GetFiles()) file.Delete();
            foreach (var sub in info.GetDirectories()) sub.Delete(true);
        }

        public static void LoadSoundToStream(string path, ref Stream targetStream, ref SoundPlayer targetSound)
        {
            if (targetStream != null)
            {
                targetStream.Dispose();
                targetStream = null;
            }
            if (targetSound != null)
            {
                targetSound.Dispose();
                targetSound = null;
            }
            if (File.Exists(path))
            {
                targetStream = new MemoryStream();
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    byte[] buffer = new byte[4096];
                    int readBytes = -1;
                    while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        targetStream.Write(buffer, 0, readBytes);
                    }
                }

                targetSound = new SoundPlayer(targetStream);
            }
        }
        public static void PlaySound(SoundPlayer player)
        {
            try
            {
                Stream stream = player.Stream; // workaround over The Wave header is corrupted exception.
                stream.Position = 0;
                player.Stream = null;
                player.Stream = stream;
                player.Play();
            }
            catch { }
        }

        //thanks to: SwDevMan81
        //http://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality
        public static bool IsEqualTo(this SecureString ss1, SecureString ss2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(ss1);
                bstr2 = Marshal.SecureStringToBSTR(ss2);
                int length1 = Marshal.ReadInt32(bstr1, -4);
                int length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (int x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }

        #region Window Flashing API
        private const UInt32 FLASHW_STOP = 0; //Stop flashing. The system restores the window to its original state.
        private const UInt32 FLASHW_CAPTION = 1; //Flash the window caption.
        private const UInt32 FLASHW_TRAY = 2; //Flash the taskbar button.
        private const UInt32 FLASHW_ALL = 3; //Flash both the window caption and taskbar button.
        private const UInt32 FLASHW_TIMER = 4; //Flash continuously, until the FLASHW_STOP flag is set.
        private const UInt32 FLASHW_TIMERNOFG = 12; //Flash continuously until the window comes to the foreground.

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize; //The size of the structure in bytes.
            public IntPtr hwnd; //A Handle to the Window to be Flashed. The window can be either opened or minimized.
            public UInt32 dwFlags; //The Flash Status.
            public UInt32 uCount; // number of times to flash the window
            public UInt32 dwTimeout; //The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static void FlashWindow(this Window win, UInt32 count = UInt32.MaxValue)
        {
            //Don't flash if the window is active
            if (win.IsActive) return;

            WindowInteropHelper h = new WindowInteropHelper(win);

            FLASHWINFO info = new FLASHWINFO
            {
                hwnd = h.Handle,
                dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG,
                uCount = count,
                dwTimeout = 0
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            FlashWindowEx(ref info);
        }
        public static void StopFlashingWindow(this Window win)
        {
            WindowInteropHelper h = new WindowInteropHelper(win);

            FLASHWINFO info = new FLASHWINFO();
            info.hwnd = h.Handle;
            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            info.dwFlags = FLASHW_STOP;
            info.uCount = UInt32.MaxValue;
            info.dwTimeout = 0;

            FlashWindowEx(ref info);
        }
        #endregion
    }
}
