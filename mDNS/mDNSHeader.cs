using System;
using System.Collections.Generic;
using System.Text;

namespace mDNS
{
    public class mDNSHeader
    {
        public static int ByteLen = 12;
        public ushort id;
        public ushort flags;
        public ushort qdcount; /* Question count */
        public ushort an_rrs;  /* Answer resource records count */
        public ushort ns_rrs;  /* Authoritative NS resource records count */
        public ushort ar_rrs;  /* Additional resource records count */

        public mDNSHeader()
        {
            id = 0;
            flags = 0;
            qdcount = 0;
            an_rrs = 0;
            ns_rrs = 0;
            ar_rrs = 0;
        }

        public mDNSHeader(Byte[] data)
        {
            id      = mDNSPacket.ReadUInt16(data, 0);
            flags   = mDNSPacket.ReadUInt16(data, 2);
            qdcount = mDNSPacket.ReadUInt16(data, 4);
            an_rrs  = mDNSPacket.ReadUInt16(data, 6);
            ns_rrs  = mDNSPacket.ReadUInt16(data, 8);
            ar_rrs  = mDNSPacket.ReadUInt16(data, 10);
        }

        public bool QR
        {
            get
            {
                return ((flags >> 15) == 1);
            }
            set
            {
                if (value)
                    flags |= (1 << 15);
                else
                    flags &= 0x7FFF;
            }
        }

        public OpCode OPCODE
        {
            get
            {
                return (OpCode)((flags >> 11) & 0x0F);
            }
            set
            {
                ushort val = (ushort)value;
                flags &= (ushort)(0x87FF | (val << 11));
            }
        }

        public bool AA
        {
            get
            {
                return (((flags >> 10) & 0x01) == 1);
            }
            set
            {
                if (value)
                    flags |= (1 << 10);
                else
                    flags &= 0xFBFF;
            }
        }

        public bool TC
        {
            get
            {
                return (((flags >> 9) & 0x01) == 1);
            }
            set
            {
                if (value)
                    flags |= (1 << 9);
                else
                    flags &= 0xFDFF;
            }
        }

        public bool RD
        {
            get
            {
                return (((flags >> 8) & 0x01) == 1);
            }
            set
            {
                if (value)
                    flags |= (1 << 8);
                else
                    flags &= 0xFEFF;
            }
        }

        public bool RA
        {
            get
            {
                return (((flags >> 7) & 0x01) == 1);
            }
            set
            {
                if (value)
                    flags |= (1 << 7);
                else
                    flags &= 0xFF7F;
            }
        }

        public RespCode RCODE
        {
            get
            {
                return (RespCode)(flags & 0x0F);
            }
            set
            {
                ushort val = (ushort)value;
                flags &= (ushort)(0xFFF0 | (val & 0x0F));
            }
        }

        public void print()
        {
            Console.WriteLine(String.Format("ID      : {0}", id));
            Console.WriteLine(String.Format("Flags   : {0:X}", flags));
            Console.WriteLine(String.Format("          {0} OP({1}) AA({2}) TC({3}) RD({4}) RA({5}) RESP({6})", QR ? "R" : "Q", OPCODE, AA, TC, RD, RA, RCODE));
            Console.WriteLine(String.Format("QD Count : {0}", qdcount));
            Console.WriteLine(String.Format("AN Count : {0}", an_rrs));
            Console.WriteLine(String.Format("NS Count: {0}", ns_rrs));
            Console.WriteLine(String.Format("AR Count: {0}", ar_rrs));
        }

        public Byte[] data
        {
            get
            {
                var data = new List<Byte>();
                data.AddRange(mDNSPacket.WriteUInt16(id));
                data.AddRange(mDNSPacket.WriteUInt16(flags));
                data.AddRange(mDNSPacket.WriteUInt16(qdcount));
                data.AddRange(mDNSPacket.WriteUInt16(an_rrs));
                data.AddRange(mDNSPacket.WriteUInt16(ns_rrs));
                data.AddRange(mDNSPacket.WriteUInt16(ar_rrs));
                return data.ToArray();
            }
        }

    }
}
