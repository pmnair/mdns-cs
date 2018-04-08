using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace mDNS
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            mDNSResponder responder = new mDNSResponder();

            responder.start();
            Thread.Sleep(10000);
            responder.stop();
            */

            mDNSPacket dnsPkt = new mDNSPacket();
            mDNSHeader mdnsHdr = new mDNSHeader();
            mDNSAnswerRR mdnsAnRR = new mDNSAnswerRR("_services._dns-sd._udp.local.", Type.TXT, Class.IN, 4500, 16);
            mDNSAdditionalRR mdnsArRR = new mDNSAdditionalRR("_services._dns-sd._udp.local.", Type.TXT, Class.IN, 4500, 16);
            Byte[] rdata = new Byte[16];

            mdnsAnRR.RDATA = rdata;
            mdnsArRR.RDATA = rdata;

            mdnsHdr.AA = true;
            mdnsHdr.TC = false;
            mdnsHdr.RD = false;
            mdnsHdr.RA = false;
            mdnsHdr.RCODE = RespCode.NoError;
            mdnsHdr.QR = true;
            mdnsHdr.qdcount = 0;
            mdnsHdr.an_rrs = 3;
            mdnsHdr.ns_rrs = 0;
            mdnsHdr.ar_rrs = 4;

            dnsPkt.Header = mdnsHdr;
            dnsPkt.AnswerRR = mdnsAnRR;
            dnsPkt.AnswerRR = mdnsAnRR;
            dnsPkt.AnswerRR = mdnsAnRR;
            dnsPkt.AdditionalRR = mdnsArRR;
            dnsPkt.AdditionalRR = mdnsArRR;
            dnsPkt.AdditionalRR = mdnsArRR;
            dnsPkt.AdditionalRR = mdnsArRR;

            dnsPkt.print();

            mDNSPacket dnsPkt1 = new mDNSPacket(dnsPkt.data);
            dnsPkt1.print();

        }
    }
}
