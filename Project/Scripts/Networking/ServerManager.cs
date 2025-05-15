using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ServerToGame;

namespace MineExploration
{
    public static class ServerManager
    {
        private static readonly int bufferSize = 1024;
        private static NetworkStream stream;
        private static TcpClient client;
        public static bool Connected { get; private set; }

        /// <summary>
        /// Tries to connect to the specified TCP server within a given timeout.
        /// </summary>
        /// <param name="host">The IP or hostname of the server.</param>
        /// <param name="port">The TCP port of the server.</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <param name="client">An out parameter that returns the connected TcpClient if successful.</param>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        public static async Task<bool> TryToConnect(string host, int port, int timeoutMs)
        {
            client = new TcpClient();
            try
            {
                using var cts = new CancellationTokenSource(timeoutMs);

                await client.ConnectAsync(host, port).WaitAsync(cts.Token);

                stream = client.GetStream();
                _ = Task.Run(ReceiveMessages);

                Connected = true;

                PayloadRegistry.Register(MessageType.AssignServerIdentification, () => new AssignServerIdentification());
                PayloadRegistry.Register(MessageType.MoveGameObject, () => new ObjectMoveRequest());
                PayloadRegistry.Register(MessageType.UpdateObject, () => new ObjectUpdate());

                //SendMessage($"{(int)ServerCommands.FetchGameData}");

                return true;
            }
            catch
            {
                Disconnect();

                return false;
            }
        }

        public static void Disconnect()
        {
            client.Close();
            client = null;
            Connected = false;
        }

        private static async Task ReceiveMessages()
        {
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer);

                if (client == null)
                {
                    break;
                }

                byte[] messageData = buffer[..bytesRead];
                NetworkMessage message = NetworkMessage.FromBytes(messageData);

                MessageHandler.ProcessMessage(message);
            }
        }

        public static void SendMessage(NetworkMessage message)
        {
            if (stream == null || !stream.CanWrite)
            {
                return;
            }

            stream.Write(message.ToBytes());
        }
    }
}
