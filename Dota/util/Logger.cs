using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP.util
{
    class Logger
    {

        public void logSucess(String message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format(message, args));
            Console.ResetColor();
        }
        public void logError(String message,params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(String.Format(message, args));
            Console.ResetColor();
        }
        public void logInfo(String message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Format(message, args));
            Console.ResetColor();
        }
    }
}
