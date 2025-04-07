using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPServer;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;

namespace MineExploration
{
    public class Server
    {
        private readonly static Queue<int> availableIds = new();
        private static int nextObjectId = 1;

        public bool IsRunning { get; private set; }
        private TcpListener listener;

        private string gameData;
        private Dictionary<int, GameObjectType> iDGameObjectTypePair = [];
        private Dictionary<int, Vector2> iDGameObjectPositionPair = [];

        #region ID handling
        public static int NewClientId()
        {
            int id;

            if (availableIds.Count > 0)
            {
                id = availableIds.Dequeue();
            }
            else
            {
                id = nextObjectId++; // Assign a new ID
            }

            return id;
        }

        public static void ReleaseID(int id)
        {
            availableIds.Enqueue(id);

            Console.WriteLine($"[SERVER] Released id: [{id}]");
        }
        #endregion

        #region Start and stop functions
        public void Start(int port, IPAddress address)
        {
            listener = new TcpListener(address, port);
            listener.Start();
            IsRunning = true;

            Console.WriteLine($"[SERVER] Server has started port on { port }, and using ip address { address }");

            Task.Run(AcceptClients);
        }

        public void Stop()
        {
            IsRunning = false;
            listener.Stop();

            foreach (var client in ClientManager.clients.Values)
            {
                client.TcpClient.Close();
            }

            ClientManager.clients.Clear();
        }
        #endregion

        private async Task AcceptClients()
        {
            while (IsRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                
                _ = Task.Run(() => HandleClient(client)); // Start handling this client in a new task
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            string clientId = Guid.NewGuid().ToString();
            ClientManager.AddClient(clientId, client);

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (IsRunning)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) // Client disconnected
                    {
                        break;
                    }

                    ClientInfo? clientInfo = ClientManager.GetClient(clientId);

                    string messageReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string message;

                    // The index 0 in the variable below should withhold the "method"
                    // This would mean that the message shall be formatted as "METHOD:DATA..."
                    string[] parts = messageReceived.Split(':');

                    ServerCommands command = (ServerCommands)int.Parse(parts[0]);

                    //Console.WriteLine($"[CLIENT] Client: { clientId } sent: [{ messageReceived }]"); // (For debugging)
                    Console.WriteLine($"[CLIENT] Client: { clientId } sent command: { command } ");

                    switch (command)
                    {
                        case ServerCommands.FetchID: // For this command index 1 should be the game objects *temp id*

                            if (parts.Length != 3)
                            {
                                break;
                            }

                            int newId = NewClientId();
                            message = $"{(int)DataSent.ID}:{newId}:{parts[1]}";

                            iDGameObjectTypePair.TryAdd(newId, (GameObjectType)int.Parse(parts[2]));

                            byte[] responseBytes = Encoding.UTF8.GetBytes(message);

                            if (clientInfo != null)
                            {
                                ClientManager.SendMessage(message, clientInfo.Value);
                                clientInfo.Value.connectedIDS.Add(newId);
                            }

                            Console.WriteLine($"[SERVER] Sent: [{message}] to client: [{clientId}]");
                            break;
                        case ServerCommands.Echo:

                            message = string.Join(":", parts.Skip(1));

                            ClientManager.Echo(message, client);

                            Console.WriteLine($"[SERVER] Echo: [{message}] from client: [{clientId}]");
                            break;
                        case ServerCommands.ReleaseID:
                            if (parts.Length != 2)
                            {
                                break;
                            }

                            ReleaseID(int.Parse(parts[1]));
                            break;
                        case ServerCommands.FetchGameData:

                            if (clientInfo != null)
                            {
                                ClientManager.SendMessage(gameData, clientInfo.Value);
                            }

                            Console.WriteLine($"[SERVER] Sent: [{gameData}] to client: [{clientId}]");
                            break;
                        default:
                            Console.WriteLine($"[ERROR] Unknown command: {command} sent from: [{clientId}] ");
                            break;
                    }

                    // Set the gameData to the data sent to later send to the clients if needed
                    DataSent dataSent = (DataSent)int.Parse(parts[1]);

                    switch (dataSent)
                    {
                        case DataSent.Move:
                            int id = int.Parse(parts[2]);

                            if (!iDGameObjectTypePair.ContainsKey(id))
                            {
                                iDGameObjectPositionPair.Remove(id);
                                break;
                            }

                            if (iDGameObjectPositionPair.ContainsKey(id))
                            {
                                iDGameObjectPositionPair[id] = new Vector2();
                            }
                            else
                            {
                                iDGameObjectPositionPair.Add(id, new Vector2());
                            }

                            break;
                        case DataSent.Mine:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Client: [{ clientId }], { ex.Message }");
            }
            finally
            {
                ClientManager.RemoveClient(clientId);
            }
        }
    }
}
