using System.Net.Sockets;
using System.Text;
using ServerToGame;

namespace TCPServer
{
    public class ClientManager
    {
        public static readonly Dictionary<string, Client> clients = [];
        private static readonly object _lock = new();

        /// <summary>
        /// Adds a new client to the dictionary.
        /// </summary>
        public static Client AddClient(string clientIdentification, TcpClient tcpClient)
        {
            var client = new Client(clientIdentification, tcpClient);

            lock (_lock)
            {
                clients[clientIdentification] = client;

                return client;
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
                    //for (int i = 0; i < clientInfo.attachedIdentifications.Count; i++)
                    //{
                    //    Echo($"{(int)MessageType.DestroyGameObject}:{clientInfo.attachedIdentifications[i]}", clientInfo.TcpClient);
                    //    ObjectHandler.ReleaseIdentification(clientInfo.attachedIdentifications[i]);
                    //}

                    clientInfo.TcpClient.Close();
                    clients.Remove(clientIdentification);
                }
            }
        }

        /// <summary>
        /// Gets the ClientInfo for a given client Identification, or null if not found.
        /// </summary>
        public static Client? GetClient(string clientIdentification)
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
    }

    public struct Client(string clientIdentification, TcpClient client)
    {
        public string Identification { get; } = clientIdentification;
        public TcpClient TcpClient { get; } = client;
        public List<int> attachedIdentifications = [];

        public readonly bool IsConnected => TcpClient != null && TcpClient.Connected;
    }
}
