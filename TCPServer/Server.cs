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
        #region Identification handling variables
        private readonly static Queue<int> availableIdentifications = new();
        private static int nextAvailableIdentification = 1;
        #endregion

        public const int bufferSize = 1024;

        public bool Active { get; private set; }
        private TcpListener listener;

        private static Dictionary<int, GameObjectServerData> gameObjects = [];

        #region Identification handling
        public static int NewGameObjectIdentification()
        {
            if (availableIdentifications.Count > 0)
            {
                return availableIdentifications.Dequeue();
            }
            else
            {
                return nextAvailableIdentification++; // Assign a new ID
            }
        }

        public static void ReleaseIdentification(int identification)
        {
            gameObjects.Remove(identification);

            availableIdentifications.Enqueue(identification);

            ConsoleServerMessage($"Released identification: [{identification}]");
        }
        #endregion

        #region Start and stop functions
        public Task Start(int port, IPAddress address)
        {
            listener = new TcpListener(address, port);
            listener.Start();
            Active = true;

            ConsoleServerMessage($"Server has started port on [{port}], using ip address: [{address}]");

            Task.Run(AcceptClients);
            return Task.CompletedTask;
        }

        public void Stop()
        {
            Active = false;
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
            while (Active)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                
                _ = Task.Run(() => HandleClient(client)); // Start handling this client in a new task
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            string clientIdentification = Guid.NewGuid().ToString(); // Give a client an unique id
            ClientManager.AddClient(clientIdentification, client);

            ConsoleServerMessage($"Client: [{clientIdentification}] has connected. Client count: [{ClientManager.clients.Count}]");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[bufferSize];

            try
            {
                while (Active)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) // Client disconnected
                    {
                        break;
                    }

                    ClientInfo? clientInfo = ClientManager.GetClient(clientIdentification);

                    string messageReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string serverMessage = string.Empty;

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

                        ConsoleClientMessage($"Client: [{clientIdentification}] sent command: [{command}]");

                        switch (command)
                        {
                            case ServerCommands.FetchIdentification: // For this command index 1 should be the game objects *temp identification*

                                if (parts.Length != 3 || clientInfo == null)
                                {
                                    break;
                                }

                                int temp = NewGameObjectIdentification();
                                serverMessage = $"{(int)DataSent.Identification}:{temp}:{parts[1]}";

                                byte[] responseBytes = Encoding.UTF8.GetBytes(serverMessage);

                                ClientManager.SendMessage(serverMessage, clientInfo.Value);
                                clientInfo.Value.attachedIdentifications.Add(temp);

                                ConsoleServerMessage($"Sent: [{serverMessage}] to client: [{clientIdentification}]");
                                break;
                            case ServerCommands.ReleaseIdentification:
                                if (parts.Length != 2)
                                {
                                    break;
                                }

                                ReleaseIdentification(int.Parse(parts[1]));
                                break;
                            case ServerCommands.Echo:

                                serverMessage = string.Join(":", parts.Skip(1));

                                ClientManager.Echo(serverMessage, client);

                                ConsoleServerMessage($"Echoed: [{serverMessage}] sent from client: [{clientIdentification}]");

                                if (int.TryParse(parts[1], out int value))
                                {
                                    DataSent dataSent = (DataSent)value;
                                    int gameObjectsIdentification;
                                    Vector2 tempPosition;

                                    switch (dataSent)
                                    {
                                        case DataSent.Move:
                                            gameObjectsIdentification = int.Parse(parts[2]);
                                            tempPosition = new(float.Parse(parts[3]), float.Parse(parts[4]));

                                            if (gameObjects.ContainsKey(gameObjectsIdentification))
                                            {
                                                gameObjects[gameObjectsIdentification].position = tempPosition;
                                            }
                                            break;
                                        case DataSent.Mine:
                                            break;
                                        case DataSent.NewGameObject:

                                            GameObjectType type = (GameObjectType)int.Parse(parts[3]);
                                            gameObjectsIdentification = int.Parse(parts[2]);
                                            tempPosition = new(float.Parse(parts[4]), float.Parse(parts[5]));

                                            if (!gameObjects.ContainsKey(gameObjectsIdentification))
                                            {
                                                gameObjects.Add(gameObjectsIdentification, new GameObjectServerData() { position = tempPosition, type = type });
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case ServerCommands.FetchGameData:
                                if (clientInfo == null)
                                {
                                    break;
                                }

                                foreach (int key in gameObjects.Keys)
                                {
                                    if (clientInfo.Value.attachedIdentifications.Contains(key))
                                    {
                                        continue;
                                    }

                                    serverMessage += $"{(int)DataSent.NewGameObject}:{key}:{(int)gameObjects[key].type}:{gameObjects[key].position.X}:{gameObjects[key].position.Y};";
                                }

                                ClientManager.SendMessage(serverMessage, clientInfo.Value);

                                ConsoleServerMessage($"Sent: [{serverMessage}] to client: [{clientIdentification}]");
                                break;
                            default:
                                ConsoleErrorMessage($"Unknown command: {command} sent from: [{clientIdentification}]");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleErrorMessage($"Client: [{clientIdentification}], {ex.Message}");
            }
            finally
            {
                ClientManager.RemoveClient(clientIdentification);
            }
        }

        public class GameObjectServerData()
        {
            public Vector2 position;
            public GameObjectType type;
        }

        public static void ConsoleServerMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[SERVER] " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void ConsoleClientMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[CLIENT] " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void ConsoleErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
