﻿
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
        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        /**************************************************
         * Function: ParseMessage
         * Input: command - string of the command you want to happen
         * Description: Uses PostMessage to send values to the specified process in constants
         * ************************************************/
        public void ParseMessage(string command)
        {
            switch (command)
            {
                case "Play/Pause":
                    SendKey(Constants.VK_SPACE);
                    break;
                case "Stop":
                    SendKey(Constants.VK_DECIMAL);
                    break;
                case "Volume Up":
                    SendKey(Constants.VK_UP);
                    break;
                case "Volume Down":
                    SendKey(Constants.VK_DOWN);
                    break;
                case "Next":
                    SendKey(Constants.VK_NEXT);
                    break;
                case "Previous":
                    SendKey(Constants.VK_PRIOR);
                    break;
            }
        }

        /**************************************************
         * Function: SendKey
         * Input: VK_KEY - int of value to be sent
         * Description: Uses PostMessage to send values to the specified process in constants
         * ************************************************/
        private void SendKey(int VK_KEY)
        {
            Process[] mpc = Process.GetProcessesByName(Constants.PROCESS);

            if (mpc.Length == 0)
            {
                Console.WriteLine(Constants.PROCESS_NOT_RUNNING_ERROR);
                return;
            }
            if (mpc[0] != null)
            {
                
                PostMessage(mpc[0].MainWindowHandle, Constants.WM_KEYDOWN, VK_KEY, 0);
            }
        }

    }
}
