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

        private static void SendQuery(Socket sock, String service)
        {
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(mdnsIP), mdnsPort);
            mDNSPacket dnsPkt = new mDNSPacket();
            mDNSHeader mdnsHdr = new mDNSHeader();
            mDNSQuestion mdnsQ = new mDNSQuestion(service, QType.ANY, QClass.IN);

            mdnsHdr.AA = false;
            mdnsHdr.TC = false;
            mdnsHdr.RD = false;
            mdnsHdr.RA = false;
            mdnsHdr.RCODE = RespCode.NoError;
            mdnsHdr.OPCODE = OpCode.QUERY;
            mdnsHdr.QR = false;
            mdnsHdr.qdcount = 1;
            mdnsHdr.an_rrs = 0;
            mdnsHdr.ns_rrs = 0;
            mdnsHdr.ar_rrs = 0;

            dnsPkt.Header = mdnsHdr;
            dnsPkt.Question = mdnsQ;

            dnsPkt.print();

            Console.WriteLine("Sending Query: " + service);
            sock.SendTo(dnsPkt.data, remoteEp);
        }

        private static void AnnounceService(Socket sock, String service)
        {
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(mdnsIP), mdnsPort);
            mDNSPacket dnsPkt = new mDNSPacket();
            mDNSHeader mdnsHdr = new mDNSHeader();
            mDNSAnswerRR mdnsAnRR = new mDNSAnswerRR(service, Type.AAAA, (Class.IN | Class.CF), 4500, 16);
            Byte[] rdata = new Byte[16];

            mdnsAnRR.RDATA = rdata;

            mdnsHdr.AA = true;
            mdnsHdr.TC = false;
            mdnsHdr.RD = false;
            mdnsHdr.RA = false;
            mdnsHdr.RCODE = RespCode.NoError;
            mdnsHdr.OPCODE = OpCode.QUERY;
            mdnsHdr.QR = true;
            mdnsHdr.qdcount = 0;
            mdnsHdr.an_rrs = 1;
            mdnsHdr.ns_rrs = 0;
            mdnsHdr.ar_rrs = 0;

            dnsPkt.Header = mdnsHdr;
            dnsPkt.AnswerRR = mdnsAnRR;

            dnsPkt.print();

            Console.WriteLine("Announcing Service: " + service);
            sock.SendTo(dnsPkt.data, remoteEp);
        }

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

            //SendQuery(sock, "_services._iscsi-live._udp.local.");
            AnnounceService(sock, "iscsi-live._192_168_2_84._tcp.local.");

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
