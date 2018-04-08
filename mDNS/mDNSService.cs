using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mDNS
{
    public class mDNSResponder
    {
        private Thread responder;
        private static bool serviceUp;
        private static int mdnsPort = 5353;
        private static string mdnsIP = "224.0.0.251";

        private static void mdnsListener()
        {
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, mdnsPort);
            Socket sock = new Socket(localEp.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            MulticastOption mcastGrp = new MulticastOption(IPAddress.Parse(mdnsIP));
            IPEndPoint remoteIpEp = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remoteEp = (EndPoint)remoteIpEp;
            Byte[] data = new Byte[65536];

            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.Bind(localEp);
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastGrp);
            Console.WriteLine("Listener Started");
            while (serviceUp)
            {
                var rc = sock.ReceiveFrom(data, ref remoteEp);
                Console.WriteLine("\nFrom: '" + remoteEp.ToString() + "' len: " + rc);
                mDNSPacket mdnsPkt = new mDNSPacket(data);
            }
            Console.WriteLine("Listener Exiting");
        }

        public mDNSResponder()
        {
            responder = new Thread(new ThreadStart(mdnsListener));
        }

        public void start()
        {
            serviceUp = true;
            responder.Start();
        }

        public void stop()
        {
            //serviceUp = false;
            responder.Join();
        }
    }
}
