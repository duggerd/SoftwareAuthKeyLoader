using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader
{
    internal class Output
    {
        public enum Level
        {
            None,
            Error,
            Info,
            Debug
        }

        public static void Write(Level level, string format, params object[] args)
        {
            if (Settings.OutputLevel >= level)
            {
                Console.Write(format, args);
            }
        }

        public static void WriteLine(Level level)
        {
            if (Settings.OutputLevel >= level)
            {
                Console.WriteLine();
            }
        }

        public static void WriteLine(Level level, string format, params object[] args)
        {
            if (Settings.OutputLevel >= level)
            {
                Console.WriteLine(format, args);
            }
        }

        public static void Error(string format, params object[] args)
        {
            Write(Level.Error, format, args);
        }

        public static void ErrorLine(string format, params object[] args)
        {
            WriteLine(Level.Error, format, args);
        }

        public static void ErrorLine()
        {
            WriteLine(Level.Error);
        }

        public static void Info(string format, params object[] args)
        {
            Write(Level.Info, format, args);
        }

        public static void InfoLine(string format, params object[] args)
        {
            WriteLine(Level.Info, format, args);
        }

        public static void InfoLine()
        {
            WriteLine(Level.Info);
        }

        public static void Debug(string format, params object[] args)
        {
            Write(Level.Debug, format, args);
        }

        public static void DebugLine(string format, params object[] args)
        {
            WriteLine(Level.Debug, format, args);
        }

        public static void DebugLine()
        {
            WriteLine(Level.Debug);
        }
    }
}
