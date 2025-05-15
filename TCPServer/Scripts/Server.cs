using System.Net.Sockets;
using System.Net;
using ServerToGame;
using System.Collections.Concurrent;
using System.Linq;

namespace TCPServer
{
    public class Server
    {
        public bool Active { get; private set; }

        private readonly int bufferSize = 1024;
        private TcpListener listener;

        public const int tickRate = 60; // 60 ticks per second
        public const float tickDelta = 1f / tickRate;
        public const int tickDelay = 1000 / tickRate;

        #region Start And Stop
        public Task Start(int port, IPAddress address)
        {
            listener = new TcpListener(address, port);
            listener.Start();
            Active = true;

            Log($"Started, port: [{port}], ip address: [{address}]", LogType.Server);

            Task.Run(ListenForClients);
            return Task.CompletedTask;
        }

        public void Stop()
        {
            listener.Stop();
            Active = false;

            foreach (var client in ClientManager.clients.Values)
            {
                client.TcpClient.Close();
            }

            ClientManager.clients.Clear();
        }
        #endregion

        public static void Tick()
        {
            MessageHandler.ProcessMessages();

            ObjectManager.Update(tickDelta);

            SendUpdatesToClients();
        }

        private static void SendUpdatesToClients()
        {
            if (!ObjectManager.GetUpdatedObjects().Any())
            {
                return;
            }

            foreach (Object obj in ObjectManager.GetUpdatedObjects())
            {
                NetworkMessage updateMessage = new()
                {
                    Type = MessageType.UpdateObject,
                    Payload = new ObjectUpdate()
                    {
                        ObjectIdentification = obj.Identification,
                        Position = obj.Position,
                        Type = obj.Type,
                    }
                };

                Echo(updateMessage);
            }

            // Reset dirty flags AFTER updates are sent
            ObjectManager.ClearDirtyFlags();
        }

        private async Task ListenForClients()
        {
            while (Active)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                string clientIdentification = Guid.NewGuid().ToString(); // Give a client a unique id
                Client client = ClientManager.AddClient(clientIdentification, tcpClient);

                _ = Task.Run(() => ListenForMessages(client));

                Log($"Client: [{clientIdentification}] has connected. Client count: [{ClientManager.clients.Count}]", LogType.Server);
            }
        }

        private async Task ListenForMessages(Client client)
        {
            NetworkStream stream = client.TcpClient.GetStream();
            byte[] buffer = new byte[bufferSize];

            try
            {
                while (client.IsConnected)
                {
                    int bytesRead = await stream.ReadAsync(buffer);

                    if (bytesRead > 0)
                    {
                        byte[] messageData = buffer[..bytesRead];
                        NetworkMessage message = NetworkMessage.FromBytes(messageData);
                        MessageQueue.Enqueue(client, message);
                    }
                    else
                    {
                        break; // Client disconnected
                    }
                }
            }
            catch (Exception exception)
            {
                Log($"Exception: ({exception.Message}), originates from: [{client.Identification}]", LogType.Error);
            }
            finally
            {
                ClientManager.RemoveClient(client.Identification);
            }
        }

        #region Server message methods
        /// <summary>
        /// Broadcasts a message to all connected clients.
        /// </summary>
        public static void Echo(NetworkMessage message, Client? sender = null)
        {
            foreach (Client client in ClientManager.clients.Values)
            {
                if (sender != null && client.TcpClient == sender.Value.TcpClient)
                {
                    continue;
                }

                SendMessage(message, client);
            }
        }

        /// <summary>
        /// Sends a message to a specific client.
        /// </summary>
        public static void SendMessage(NetworkMessage message, Client client)
        {
            if (client.TcpClient == null)
            {
                return;
            }

            NetworkStream stream = client.TcpClient.GetStream();

            if (!stream.CanWrite)
            {
                return;
            }

            Log($"Sent: [{message.Type}], to: [{client.Identification}]", LogType.Server);

            stream.Write(message.ToBytes());
            stream.FlushAsync();
        }
        #endregion

        #region Log messages on server
        public static void Log(string message, LogType logType)
        {
            Console.Write($"[{DateTime.Now:HH:mm:ss}]");

            switch (logType)
            {
                case LogType.Client:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogType.Server:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            Console.WriteLine($"[{logType.ToString().ToUpper()}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        #endregion
    }

    public enum LogType
    {
        Client,
        Server,
        Error
    }
}
