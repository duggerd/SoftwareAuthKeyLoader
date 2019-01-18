using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader
{
    internal class Settings
    {
        public static string ApplicationVersion { get; private set; }

        public static string ApplicationTarget { get; private set; }

        public static Output.Level OutputLevel { get; set; }

        public static IPAddress IpAddress { get; set; }

        public static int UdpPort { get; set; }

        public static int Timeout { get; set; }

        static Settings()
        {
            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
