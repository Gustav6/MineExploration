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
using System.Diagnostics;
using System.Text.Json;

namespace MineExploration
{
    public class Server
    {
        public bool Active { get; private set; }

        private readonly int bufferSize = 1024;
        private TcpListener listener;

        private static readonly Dictionary<int, GameObjectServerData> gameObjects = [];

        private readonly static Queue<int> availableIdentifications = new();
        private static int nextAvailableIdentification = 1;


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
            listener.Stop();
            Active = false;

            foreach (var client in ClientManager.clients.Values)
            {
                client.TcpClient.Close();
            }

            ClientManager.clients.Clear();
        }

        private async Task AcceptClients()
        {
            while (Active)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                string clientIdentification = Guid.NewGuid().ToString(); // Give a client a unique id
                Client client = ClientManager.AddClient(clientIdentification, tcpClient);

                ConsoleServerMessage($"Client: [{clientIdentification}] has connected. Client count: [{ClientManager.clients.Count}]");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleClient(client);
                    }
                    catch (Exception ex)
                    {
                        ConsoleErrorMessage($"Client: [{client.Identification}], {ex.Message}");
                    }
                    finally
                    {
                        ClientManager.RemoveClient(clientIdentification);
                    }
                });
            }
        }

        private async Task HandleClient(Client client)
        {
            NetworkStream stream = client.TcpClient.GetStream();
            byte[] buffer = new byte[bufferSize];

            // [Message Length][Message Type][Payload] (Message structure)

            while (Active)
            {
                int bytesRead = await stream.ReadAsync(buffer);

                if (bytesRead == 0) // Client disconnected
                {
                    break;
                }

                string messageReceived = Encoding.UTF8.GetString(buffer);
                string serverResponse = string.Empty;

                string[] dataReceived = messageReceived.Split(';');

                foreach (string data in dataReceived)
                {
                    if (string.IsNullOrWhiteSpace(data))
                    {
                        continue;
                    }

                    string[] dataParts = data.Split(':');

                    if (!int.TryParse(dataParts.First(), out int parsedCommand))
                    {
                        continue;
                    }

                    ServerCommands command = (ServerCommands)parsedCommand;

                    ConsoleClientMessage($"Client: [{client.Identification}] sent command: [{command}]");

                    switch (command)
                    {
                        case ServerCommands.FetchIdentification: // For this command index 1 should be the game objects *temp identification*

                            int temp = NewGameObjectIdentification();
                            serverResponse = $"{(int)MessageType.Identification}:{temp}:{dataParts[1]}";

                            byte[] responseBytes = Encoding.UTF8.GetBytes(serverResponse);

                            ClientManager.SendMessage(serverResponse, client);
                            client.attachedIdentifications.Add(temp);

                            ConsoleServerMessage($"Sent: [{serverResponse}] to client: [{client.Identification}]");

                            break;
                        case ServerCommands.ReleaseIdentification:

                            ReleaseIdentification(int.Parse(dataParts[1]));
                            gameObjects.Remove(int.Parse(dataParts[1]));

                            break;
                        case ServerCommands.Echo:

                            serverResponse = string.Join(":", dataParts.Skip(1));

                            ClientManager.Echo(serverResponse, client.TcpClient);

                            ConsoleServerMessage($"Echoed: [{serverResponse}] sent from client: [{client.Identification}]");

                            if (int.TryParse(dataParts[1], out int parsed))
                            {
                                MessageType dataSent = (MessageType)parsed;
                                GameObjectServerData gameObjectData = JsonSerializer.Deserialize<GameObjectServerData>(string.Join(":", dataParts.Skip(2)));

                                if (gameObjectData != null)
                                {
                                    gameObjects[gameObjectData.Identification] = gameObjectData;
                                }
                            }

                            break;
                        case ServerCommands.FetchGameData:

                            foreach (int serverIdentification in gameObjects.Keys)
                            {
                                if (client.attachedIdentifications.Contains(serverIdentification))
                                {
                                    continue;
                                }

                                serverResponse += $"{(int)MessageType.NewGameObject}:{gameObjects[serverIdentification]};";
                            }

                            ClientManager.SendMessage(serverResponse, client);

                            ConsoleServerMessage($"Sent: [{serverResponse}] to client: [{client.Identification}]");
                            break;
                        default:
                            ConsoleErrorMessage($"Unknown command: [{command}] sent from: [{client.Identification}]");
                            break;
                    }
                }
            }
        }

        public static int NewGameObjectIdentification()
        {
            if (availableIdentifications.Count > 0)
            {
                return availableIdentifications.Dequeue();
            }

            return nextAvailableIdentification++; // Assign a new ID
        }

        public static void ReleaseIdentification(int identification)
        {
            gameObjects.Remove(identification);

            availableIdentifications.Enqueue(identification);

            ConsoleServerMessage($"Released identification: [{identification}]");
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
