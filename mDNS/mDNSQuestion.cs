using System;
using System.Collections.Generic;
using System.Text;

namespace mDNS
{
    public class mDNSQuestion
    {
        public int ByteLen;
        string name;
        public string QName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (!name.EndsWith(".", StringComparison.Ordinal))
                    name += ".";
            }
        }
        public QType QType;
        public QClass QClass;

        public mDNSQuestion(string qname, QType qtype, QClass qclass)
        {
            name = qname;
            QType = qtype;
            QClass = qclass;
        }

        public mDNSQuestion(Byte[] data, int offset)
        {
            int idx = offset;
            name = mDNSPacket.ReadName(data, ref idx);
            QType = (QType)mDNSPacket.ReadUInt16(data, idx);
            QClass = (QClass)mDNSPacket.ReadUInt16(data, (idx+2));
            ByteLen = ((idx + 4) - offset);
        }

        public Byte[] data
        {
            get
            {
                var data = new List<Byte>();
                data.AddRange(mDNSPacket.WriteName(QName));
                data.AddRange(mDNSPacket.WriteUInt16((ushort)QType));
                data.AddRange(mDNSPacket.WriteUInt16((ushort)QClass));
                return data.ToArray();
            }
        }

        public void print()
        {
            Console.WriteLine(string.Format("{0,-32}\t{1}\t{2}", QName, QClass, QType));
        }
    }
}
