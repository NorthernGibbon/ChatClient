﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatClient.Net.IO;

namespace ChatClient.Net
{
    public class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgRecievedEvent;
        public event Action userDisconnectEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("Server ip goes here", 7800);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.getPacketBytes());
                }

                ReadPackets();
            }
        }
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;

                        case 5:
                            msgRecievedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("Ah yes...");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);

            _client.Client.Send(messagePacket.getPacketBytes());
        }
    }
}
