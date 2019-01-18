using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader
{
    internal static class Network
    {
        public static IPAddress ParseIpAddress(string ipAddress)
        {
            return IPAddress.Parse(ipAddress);
        }

        public static int ParseUdpPort(string udpPort)
        {
            int udpPortNumber = int.Parse(udpPort);

            if (udpPortNumber >= 1 && udpPortNumber <= 65535)
            {
                return udpPortNumber;
            }
            else
            {
                throw new ArgumentOutOfRangeException("udpPortNumber");
            }
        }

        public static int ParseTimeout(string timeout)
        {
            int timeoutValue = int.Parse(timeout);

            if (timeoutValue > 0)
            {
                return timeoutValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
        }

        public static byte[] QueryRadio(byte[] toRadio)
        {
            string ipAddress = Settings.IpAddress.ToString();
            int udpPort = Settings.UdpPort;
            int timeout = Settings.Timeout;
            Output.DebugLine("ip address: {0}, udp port: {1}, receive timeout: {2}", ipAddress, udpPort, timeout);
            using (UdpClient udpClient = new UdpClient(ipAddress, udpPort))
            {
                Output.DebugLine("sending {0} bytes to radio - {1}", toRadio.Length, BitConverter.ToString(toRadio));
                udpClient.Client.ReceiveTimeout = timeout;
                udpClient.Send(toRadio, toRadio.Length);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] fromRadio = udpClient.Receive(ref remoteEndPoint);
                Output.DebugLine("received {0} bytes from radio - {1}", fromRadio.Length, BitConverter.ToString(fromRadio));
                return fromRadio;
            }
        }
    }
}
