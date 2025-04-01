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
        public static bool RemoveClient(string clientId)
        {
            lock (_lock)
            {
                if (clients.TryGetValue(clientId, out var clientInfo))
                {
                    clientInfo.TcpClient.Close();
                    clients.Remove(clientId);
                    Console.WriteLine($"[SERVER] The client { clientId } was removed. Total clients: { clients.Count }");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the ClientInfo for a given client ID, or null if not found.
        /// </summary>
        public static ClientInfo GetClient(string clientId)
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
        public static void Broadcast(string message, TcpClient? sender = null)
        {
            lock (_lock)
            {
                foreach (var pair in clients)
                {
                    if (sender != null && pair.Value.TcpClient == sender)
                    {
                        continue;
                    }

                    SendMessage(pair.Value, message);
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific client.
        /// </summary>
        public static void SendMessage(ClientInfo clientInfo, string message)
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

            Console.WriteLine($"[SERVER] Server sent: { message } to client: { clientInfo.ClientId }");
        }
    }

    public class ClientInfo(string clientId, TcpClient tcpClient)
    {
        public string ClientId { get; } = clientId;
        public TcpClient TcpClient { get; } = tcpClient;
    }
}
