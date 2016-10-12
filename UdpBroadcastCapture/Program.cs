using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UdpBroadcastCapture.ServiceReference1;

namespace UdpBroadcastCapture
{
    class Program
    {
        // https://msdn.microsoft.com/en-us/library/tst0kwb1(v=vs.110).aspx
        // IMPORTANT Windows firewall must be open on UDP port 7000
        // Use the network EGV5-DMU2 to capture from the local IoT devices
        private const int Port = 5005;
        //private static readonly IPAddress IpAddress = IPAddress.Parse("192.168.5.137"); 
        private static readonly IPAddress IpAddress = IPAddress.Any;
        // Listen for activity on all network interfaces
        // https://msdn.microsoft.com/en-us/library/system.net.ipaddress.ipv6any.aspx
        static void Main()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IpAddress, 5005);

            using (UdpClient socket = new UdpClient(Port))
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                    byte[] datagramReceived = socket.Receive(ref remoteEndPoint);

                    string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                    Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                        remoteEndPoint.Address, remoteEndPoint.Port, message);

                    string[] Parts = message.Split(' ');

                    string temp = Parts[1];
                    string lys = Parts[3];

                    Console.WriteLine(temp +" "+ lys);
                    using (Service1Client client = new Service1Client())
                    {
                        int temperature;
                        int light;
                        Int32.TryParse(temp, out temperature);
                        Int32.TryParse(lys, out light);
                        client.InsertData(temperature, light);
                    }


                    //Parse(message);
                }
            }

        }

        // To parse data from the IoT devices in the teachers room, Elisagårdsvej
        private static void Parse(string response)
        {
            string[] parts = response.Split(' ');
            foreach (string part in parts)
            {
                Console.WriteLine(part);
            }
            string temperatureLine = parts[6];
            string temperatureStr = temperatureLine.Substring(temperatureLine.IndexOf(": ") + 2);
            Console.WriteLine(temperatureStr);

        }

    }
}
