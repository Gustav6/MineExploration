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
        public static void AddClient(string clientId, TcpClient tcpClient)
        {
            var clientInfo = new ClientInfo(clientId, tcpClient);

            lock (_lock)
            {
                clients[clientId] = clientInfo;
            }

            Console.WriteLine($"[SERVER] The client: { clientId } has connected. Total clients: { clients.Count }");
        }

        /// <summary>
        /// Removes a client by ID and closes its connection.
        /// </summary>
        public static void RemoveClient(string clientId)
        {
            lock (_lock)
            {
                if (clients.TryGetValue(clientId, out var clientInfo))
                {
                    for (int i = 0; i < clientInfo.connectedIDS.Count; i++)
                    {
                        Echo($"{(int)DataSent.DestroyGameObject}:{clientInfo.connectedIDS[i]}", clientInfo.TcpClient);
                        Server.ReleaseID(clientInfo.connectedIDS[i]);
                    }

                    clientInfo.TcpClient.Close();
                    clients.Remove(clientId);

                    Console.WriteLine($"[SERVER] The client {clientId} has disconnected. Total clients: {clients.Count}");
                }
            }
        }

        /// <summary>
        /// Gets the ClientInfo for a given client ID, or null if not found.
        /// </summary>
        public static ClientInfo? GetClient(string clientId)
        {
            lock (_lock)
            {
                if (clients.TryGetValue(clientId, out var clientInfo))
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
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                stream.FlushAsync();
            }
        }
    }

    public struct ClientInfo(string clientId, TcpClient tcpClient)
    {
        public string ClientId { get; } = clientId;
        public TcpClient TcpClient { get; } = tcpClient;
        public List<int> connectedIDS = new();
    }
}
