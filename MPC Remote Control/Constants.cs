using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPC_Remote_Control
{
    static class Constants
    {
        //Constants for keys
        public const uint WM_KEYDOWN = 0x0100;
        public const int VK_SPACE = 0x20;
        public const int VK_PRIOR = 0x21;
        public const int VK_NEXT = 0x22;
        public const int VK_UP = 0x26;
        public const int VK_DOWN = 0x28;
        public const int VK_DECIMAL = 0x6E;

        public const string PROCESS = "mpc-hc";
        public const string PROCESS_NOT_RUNNING_ERROR = "MPC is not running.";
        public const string INVALID_IP_CHOICE_ERROR = "Your IP selection was invalid.";
        
    }
}
