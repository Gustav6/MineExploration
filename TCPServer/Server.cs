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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace MineExploration
{
    public class Server
    {
        private readonly static Queue<int> availableIds = new();
        private static int nextObjectId = 1;

        public bool IsRunning { get; private set; }
        private TcpListener listener;

        private static Dictionary<int, GameObjectServerData> gameObjects = [];

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
            gameObjects.Remove(id);

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
                    string serverMessage;

                    string[] objectDataArray = messageReceived.Split(';');

                    foreach (string objectData in objectDataArray)
                    {
                        if (string.IsNullOrWhiteSpace(objectData))
                        {
                            continue;
                        }

                        // The index 0 in the variable below should withhold the "method"
                        // This would mean that the message shall be formatted as "METHOD:DATA..."
                        string[] parts = objectData.Split(':');
                        ServerCommands command;

                        if (int.TryParse(parts.First(), out int parsedCommand))
                        {
                            command = (ServerCommands)parsedCommand;

                        }
                        else
                        {
                            continue;
                        }

                        //Console.WriteLine($"[CLIENT] Client: { clientId } sent: [{ messageReceived }]"); // (For debugging)
                        Console.WriteLine($"[CLIENT] Client: {clientId} sent command: {command} ");

                        switch (command)
                        {
                            case ServerCommands.FetchID: // For this command index 1 should be the game objects *temp id*

                                if (parts.Length != 3)
                                {
                                    break;
                                }

                                int newId = NewClientId();
                                serverMessage = $"{(int)DataSent.ID}:{newId}:{parts[1]}";

                                byte[] responseBytes = Encoding.UTF8.GetBytes(serverMessage);

                                if (clientInfo != null)
                                {
                                    ClientManager.SendMessage(serverMessage, clientInfo.Value);
                                    clientInfo.Value.connectedIDS.Add(newId);
                                }

                                Console.WriteLine($"[SERVER] Sent: [{serverMessage}] to client: [{clientId}]");
                                break;
                            case ServerCommands.ReleaseID:
                                if (parts.Length != 2)
                                {
                                    break;
                                }

                                ReleaseID(int.Parse(parts[1]));
                                break;
                            case ServerCommands.Echo:

                                serverMessage = string.Join(":", parts.Skip(1));

                                ClientManager.Echo(serverMessage, client);

                                // Set the gameData to the data sent to later send to the clients if needed
                                if (int.TryParse(parts[1], out int value))
                                {
                                    DataSent dataSent = (DataSent)value;
                                    int id;
                                    Vector2 tempPosition;

                                    switch (dataSent)
                                    {
                                        case DataSent.Move:
                                            id = int.Parse(parts[2]);
                                            tempPosition = new(float.Parse(parts[3]), float.Parse(parts[4]));

                                            if (gameObjects.ContainsKey(id))
                                            {
                                                gameObjects[id].position = tempPosition;
                                            }
                                            break;
                                        case DataSent.Mine:
                                            break;
                                        case DataSent.NewGameObject:

                                            GameObjectType type = (GameObjectType)int.Parse(parts[3]);
                                            id = int.Parse(parts[2]);
                                            tempPosition = new(float.Parse(parts[4]), float.Parse(parts[5]));

                                            if (!gameObjects.ContainsKey(id))
                                            {
                                                gameObjects.Add(id, new GameObjectServerData() { position = tempPosition, type = type });
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                Console.WriteLine($"[SERVER] Echo: [{serverMessage}] from client: [{clientId}]");
                                break;
                            case ServerCommands.FetchGameData:

                                if (clientInfo == null)
                                {
                                    break;
                                }

                                serverMessage = "Empty;";

                                foreach (int id in gameObjects.Keys)
                                {
                                    if (clientInfo.Value.connectedIDS.Contains(id))
                                    {
                                        continue;
                                    }

                                    serverMessage += $"{(int)DataSent.NewGameObject}:{id}:{(int)gameObjects[id].type}:{gameObjects[id].position.X}:{gameObjects[id].position.Y};";
                                }

                                if (clientInfo != null)
                                {
                                    ClientManager.SendMessage(serverMessage, clientInfo.Value);
                                }

                                Console.WriteLine($"[SERVER] Sent: [{serverMessage}] to client: [{clientId}]");
                                break;
                            default:
                                Console.WriteLine($"[ERROR] Unknown command: {command} sent from: [{clientId}] ");
                                break;
                        }
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

        public class GameObjectServerData()
        {
            public Vector2 position;
            public GameObjectType type;
        }
    }
}
