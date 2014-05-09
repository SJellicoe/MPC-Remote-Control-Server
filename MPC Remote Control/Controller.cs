using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MPC_Remote_Control
{
    class Controller
    {
        const uint WM_KEYDOWN = 0x0100;
        const int VK_SPACE = 0x20;
        const int VK_PRIOR = 0x21;
        const int VK_NEXT = 0x22;
        const int VK_UP = 0x26;
        const int VK_DOWN = 0x28;
        const int VK_DECIMAL = 0x6E;

 
 
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        

        public void ParseMessage(string command)
        {
            switch (command)
            {
                case "Play/Pause":
                    SendKey(VK_SPACE);
                    break;
                case "Stop":
                    SendKey(VK_DECIMAL);
                    break;
                case "Volume Up":
                    SendKey(VK_UP);
                    break;
                case "Volume Down":
                    SendKey(VK_DOWN);
                    break;
                case "Next":
                    SendKey(VK_NEXT);
                    break;
                case "Previous":
                    SendKey(VK_PRIOR);
                    break;
            }
        }

        private void SendKey(int VK_KEY)
        {
            Process[] mpc = Process.GetProcessesByName("mpc-hc");

            if (mpc.Length == 0) return;
            if (mpc[0] != null)
            {
                PostMessage(mpc[0].MainWindowHandle, WM_KEYDOWN, VK_KEY, 0);
            }
        }

    }
}
