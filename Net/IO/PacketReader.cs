using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    public class PacketReader : BinaryReader
    {

        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgByuffer;
            var length = ReadInt32();
            msgByuffer = new byte[length];
            _ns.Read(msgByuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgByuffer);

            return msg;
        }
    }
}
