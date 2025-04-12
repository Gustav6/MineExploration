using Microsoft.Xna.Framework.Input;
using MineExploration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class ClientManager
    {
        public static readonly Dictionary<string, ClientInfo> clients = [];
        private static readonly object _lock = new();

        /// <summary>
        /// Adds a new client to the dictionary.
        /// </summary>
        public static void AddClient(string clientIdentification, TcpClient tcpClient)
        {
            var clientInfo = new ClientInfo(clientIdentification, tcpClient);

            lock (_lock)
            {
                clients[clientIdentification] = clientInfo;
            }
        }

        /// <summary>
        /// Removes a client by ID and closes its connection.
        /// </summary>
        public static void RemoveClient(string clientIdentification)
        {
            lock (_lock)
            {
                if (clients.TryGetValue(clientIdentification, out var clientInfo))
                {
                    for (int i = 0; i < clientInfo.attachedIdentifications.Count; i++)
                    {
                        Echo($"{(int)DataSent.DestroyGameObject}:{clientInfo.attachedIdentifications[i]}", clientInfo.TcpClient);
                        Server.ReleaseIdentification(clientInfo.attachedIdentifications[i]);
                    }

                    clientInfo.TcpClient.Close();
                    clients.Remove(clientIdentification);

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[SERVER] Client: [{clientIdentification}] has disconnected. Client count: [{clients.Count}]");
                }
            }
        }

        /// <summary>
        /// Gets the ClientInfo for a given client Identification, or null if not found.
        /// </summary>
        public static ClientInfo? GetClient(string clientIdentification)
        {
            lock (_lock)
            {
                if (clients.TryGetValue(clientIdentification, out var clientInfo))
                {
                    return clientInfo;
                }

                return null;
            }
        }

        /// <summary>
        /// Broadcasts a message to all connected clients.
        /// </summary>
        public static void Echo(string message, TcpClient? sender = null)
        {
            lock (_lock)
            {
                foreach (var pair in clients)
                {
                    if (sender != null && pair.Value.TcpClient == sender)
                    {
                        continue;
                    }

                    SendMessage(message, pair.Value);
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific client.
        /// </summary>
        public static void SendMessage(string message, ClientInfo clientInfo)
        {
            if (clientInfo.TcpClient == null)
            {
                return;
            }

            NetworkStream stream = clientInfo.TcpClient.GetStream();

            if (stream.CanWrite)
            {
                string temp = message + ";";

                byte[] data = Encoding.UTF8.GetBytes(temp);
                stream.Write(data, 0, data.Length);
                stream.FlushAsync();
            }
        }
    }

    public struct ClientInfo(string clientIdentification, TcpClient client)
    {
        public string ClientIdentification { get; } = clientIdentification;
        public TcpClient TcpClient { get; } = client;
        public List<int> attachedIdentifications = [];
    }
}
