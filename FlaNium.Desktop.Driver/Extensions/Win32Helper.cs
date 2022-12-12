using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlaNium.Desktop.Driver.Extensions
{
    internal class Win32Helper
    {
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MINIMIZE = 6;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void MaximizeWindow(IntPtr hwnd)
        {
            ShowWindow(hwnd, SW_SHOWMAXIMIZED);
        }

    }
}
