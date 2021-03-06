using System;
using System.Collections.Generic;
using System.Text;

namespace mDNS
{
    public class mDNSRR
    {
        public string NAME;
        public Type Type;
        public Class Class;
        uint ttl;
        public ushort RDLENGTH;
        public int TimeLived;
        public int ByteLen;
        Byte[] rdata;

        public uint TTL
        {
            get
            {
                return (uint)Math.Max(0, ttl - TimeLived);
            }
            set
            {
                ttl = value;
            }
        }

        public Byte[] RDATA
        {
            get
            {
                return rdata;
            }
            set
            {
                rdata = value;
            }
        }

        public mDNSRR(Byte[] data, int offset)
        {
            int idx = offset;

            TimeLived = 0;
            NAME = mDNSPacket.ReadName(data, ref idx);
            Type = (Type)mDNSPacket.ReadUInt16(data, idx);
            Class = (Class)mDNSPacket.ReadUInt16(data, idx+2);
            TTL = mDNSPacket.ReadUInt32(data, idx + 4);
            RDLENGTH = mDNSPacket.ReadUInt16(data, idx + 8);
            rdata = new Byte[RDLENGTH];
            Array.Copy(data, idx + 10, rdata, 0, RDLENGTH);
            ByteLen = ((idx + 10 + RDLENGTH) - offset);
        }

        public mDNSRR(string name, Type type, Class cls, uint ttl, ushort data_len)
        {
            NAME = name;
            Type = type;
            Class = cls;
            TTL = ttl;
            RDLENGTH = data_len;
        }

        public Byte[] data
        {
            get
            {
                var data = new List<Byte>();
                data.AddRange(mDNSPacket.WriteName(NAME));
                data.AddRange(mDNSPacket.WriteUInt16((ushort)Type));
                data.AddRange(mDNSPacket.WriteUInt16((ushort)Class));
                data.AddRange(mDNSPacket.WriteUInt32((uint)TTL));
                data.AddRange(mDNSPacket.WriteUInt16((ushort)RDLENGTH));
                data.AddRange(RDATA);
                return data.ToArray();
            }
        }

        public void print()
        {
            Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", NAME, TTL, Class, Type, RDLENGTH));
        }
    }

    public class mDNSAnswerRR : mDNSRR
    {
        public mDNSAnswerRR(Byte[] data, int offset)
            : base(data, offset)
        {

        }

        public mDNSAnswerRR(string name, Type type, Class cls, uint ttl, ushort data_len)
            : base(name, type, cls, ttl, data_len)
        {

        }
    }

    public class mDNSAuthorityRR : mDNSRR
    {
        public mDNSAuthorityRR(Byte[] data, int offset)
            : base(data, offset)
        {

        }

        public mDNSAuthorityRR(string name, Type type, Class cls, uint ttl, ushort data_len)
            : base(name, type, cls, ttl, data_len)
        {

        }
    }

    public class mDNSAdditionalRR : mDNSRR
    {
        public mDNSAdditionalRR(Byte[] data, int offset)
            : base(data, offset)
        {

        }

        public mDNSAdditionalRR(string name, Type type, Class cls, uint ttl, ushort data_len)
            : base(name, type, cls, ttl, data_len)
        {

        }
    }
}
