using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace mDNS
{
    public class mDNSPacket
    {
        private Byte[] rawData;
        private mDNSHeader mdnsHdr;
        private List<mDNSQuestion> mdnsQs;
        private List<mDNSAnswerRR> mdnsAnRRs;
        private List<mDNSAuthorityRR> mdnsNsRRs;
        private List<mDNSAdditionalRR> mdnsArRRs;

        public mDNSPacket()
        {
            rawData = new Byte[1];
            mdnsHdr = new mDNSHeader();
            mdnsQs = new List<mDNSQuestion>();
            mdnsAnRRs = new List<mDNSAnswerRR>();
            mdnsNsRRs = new List<mDNSAuthorityRR>();
            mdnsArRRs = new List<mDNSAdditionalRR>();
        }

        public mDNSPacket(Byte[] data)
        {
            int offset = mDNSHeader.ByteLen;
            int i;

            rawData = data;
            mdnsHdr = new mDNSHeader(data);
            mdnsQs = new List<mDNSQuestion>();
            mdnsAnRRs = new List<mDNSAnswerRR>();
            mdnsNsRRs = new List<mDNSAuthorityRR>();
            mdnsArRRs = new List<mDNSAdditionalRR>();
            Console.WriteLine("== Header ==");
            mdnsHdr.print();

            Console.WriteLine("== Questions ==");
            for (i = 0; i < mdnsHdr.qdcount; i++)
            {
                mdnsQs.Add(new mDNSQuestion(data, offset));
                offset += mdnsQs[i].ByteLen;
                mdnsQs[i].print();
            }

            Console.WriteLine("== AnswerRRs ==");
            for (i = 0; i < mdnsHdr.an_rrs; i++)
            {
                mdnsAnRRs.Add(new mDNSAnswerRR(data, offset));
                offset += mdnsAnRRs[i].ByteLen;
                mdnsAnRRs[i].print();
            }

            Console.WriteLine("== AuthorityRRs ==");
            for (i = 0; i < mdnsHdr.ns_rrs; i++)
            {
                mdnsNsRRs.Add(new mDNSAuthorityRR(data, offset));
                offset += mdnsNsRRs[i].ByteLen;
                mdnsNsRRs[i].print();
            }

            Console.WriteLine("== AdditionalRRs ==");
            for (i = 0; i < mdnsHdr.ar_rrs; i++)
            {
                mdnsArRRs.Add(new mDNSAdditionalRR(data, offset));
                offset += mdnsArRRs[i].ByteLen;
                mdnsArRRs[i].print();
            }
            Console.WriteLine("===================\n");
        }

        public mDNSHeader Header
        {
            set
            {
                mdnsHdr = value;
            }
        }

        public mDNSQuestion Question
        {
            set
            {
                mdnsQs.Add(value);
            }
        }

        public mDNSAnswerRR AnswerRR
        {
            set
            {
                mdnsAnRRs.Add(value);
            }
        }

        public mDNSAuthorityRR AuthorityRR
        {
            set
            {
                mdnsNsRRs.Add(value);
            }
        }

        public mDNSAdditionalRR AdditionalRR
        {
            set
            {
                mdnsArRRs.Add(value);
            }
        }

        public void print()
        {
            Console.WriteLine("== Header ==");
            mdnsHdr.print();
            Console.WriteLine("== Questions ==");
            for (int i = 0; i < mdnsHdr.qdcount; i++)
                mdnsQs[i].print();
            Console.WriteLine("== AnswerRRs ==");
            for (int i = 0; i < mdnsHdr.an_rrs; i++)
                mdnsAnRRs[i].print();
            Console.WriteLine("== AuthorityRRs ==");
            for (int i = 0; i < mdnsHdr.ns_rrs; i++)
                mdnsNsRRs[i].print();
            Console.WriteLine("== AdditionalRRs ==");
            for (int i = 0; i < mdnsHdr.ar_rrs; i++)
                mdnsArRRs[i].print();
            Console.WriteLine("===================\n");
        }

        public Byte[] data
        {
            get
            {
                var data = new List<Byte>();

                data.AddRange(mdnsHdr.data);
                for (int i = 0; i < mdnsHdr.qdcount; i++)
                    data.AddRange(mdnsQs[i].data);
                for (int i = 0; i < mdnsHdr.an_rrs; i++)
                    data.AddRange(mdnsAnRRs[i].data);
                for (int i = 0; i < mdnsHdr.ns_rrs; i++)
                    data.AddRange(mdnsNsRRs[i].data);
                for (int i = 0; i < mdnsHdr.ar_rrs; i++)
                    data.AddRange(mdnsArRRs[i].data);
                return data.ToArray();
            }
        }

        internal static short HostToNetworkOrder(short host)
        {
            return (short)(((host & 0xff) << 8) | ((host >> 8) & 0xff));
        }

        public static Byte[] WriteUInt16(ushort val)
        {
            return BitConverter.GetBytes(HostToNetworkOrder((short)val));
        }

        public static ushort ReadUInt16(Byte[] data, int off)
        {
            return (ushort)(data[off] << 8 | data[off+1]);
        }

        internal static int HostToNetworkOrder(int host)
        {
            return (int)(((host & 0xff) << 24) | (((host >> 8) & 0xff) << 16)  | (((host >> 16) & 0xff) << 8) | ((host >> 24) & 0xff));
        }

        public static Byte[] WriteUInt32(uint val)
        {
            return BitConverter.GetBytes(HostToNetworkOrder((int)val));
        }

        public static uint ReadUInt32(Byte[] data, int off)
        {
            return (uint)(data[off] << 24 | data[off+1] << 16 | data[off+2] << 8 | data[off+3]);
        }

        public static Byte[] WriteName(string src)
        {
            if (!src.EndsWith(".", StringComparison.Ordinal))
                src += ".";

            if (src == ".")
                return new byte[1];

            var sb = new StringBuilder();
            int intI, intJ, intLen = src.Length;
            sb.Append('\0');
            for (intI = 0, intJ = 0; intI < intLen; intI++, intJ++)
            {
                sb.Append(src[intI]);
                if (src[intI] == '.')
                {
                    sb[intI - intJ] = (char)(intJ & 0xff);
                    intJ = -1;
                }
            }
            sb[sb.Length - 1] = '\0';
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static string ReadName(Byte[] data, ref int offset)
        {
            var bytes = new List<byte>();
            int length = 0;
            int idx = offset;

            // get  the length of the first label
            while ((length = data[idx++]) != 0)
            {
                // top 2 bits set denotes domain name compression and to reference elsewhere
                if ((length & 0xc0) == 0xc0)
                {
                    int off = ((length & 0x3f) << 8) | data[idx++];
                    string name = ReadName(data, ref off);

                    offset = off;
                    // work out the existing domain name, copy this pointer
                    if (bytes.Count > 0)
                    {
                        return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count) + name;
                    }
                    return name;
                }

                // if not using compression, copy a char at a time to the domain name
                while (length > 0)
                {
                    bytes.Add(data[idx++]);
                    length--;
                }
                bytes.Add((byte)'.');
            }
            offset = idx;
            if (bytes.Count == 0)
                return ".";
            return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
        }
    }
}
