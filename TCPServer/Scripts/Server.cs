using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace TCPServer
{
    public class Server
    {
        public bool Active { get; private set; }

        private readonly int bufferSize = 1024;
        private TcpListener listener;


        public Task Start(int port, IPAddress address)
        {
            listener = new TcpListener(address, port);
            listener.Start();
            Active = true;

            Log($"Started, port: [{port}], ip address: [{address}]", LogType.Server);

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

                Log($"Client: [{clientIdentification}] has connected. Client count: [{ClientManager.clients.Count}]", LogType.Server);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleClient(client);
                    }
                    catch (Exception exception)
                    {
                        Log($"Exception: ({exception.Message}), orignates from: [{client.Identification}]", LogType.Error);
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

            while (client.IsConnected)
            {
                int bytesRead = await stream.ReadAsync(buffer);

                if (bytesRead == 0)
                {
                    break; // Client disconnected
                }

                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);
                PayloadHandler.HandlePayload(data, client);

                /*
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
                }*/
            }
        }

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

            Console.WriteLine($"[{logType}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public enum LogType
    {
        Client,
        Server,
        Error
    }
}
