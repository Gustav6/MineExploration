using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class ServerHandler
    {
        private static readonly int bufferSize = 1024;
        private static NetworkStream stream;
        private static TcpClient client;
        public static bool Connected { get; private set; }
        private static readonly Dictionary<string, GameObject> tempIdentificationToGameObject = [];

        /// <summary>
        /// Tries to connect to the specified TCP server within a given timeout.
        /// </summary>
        /// <param name="host">The IP or hostname of the server.</param>
        /// <param name="port">The TCP port of the server.</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <param name="client">An out parameter that returns the connected TcpClient if successful.</param>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        public static async Task<bool> TryToConnect(string host, int port, int timeoutMs)
        {
            client = new TcpClient();
            try
            {
                using var cts = new CancellationTokenSource(timeoutMs);

                await client.ConnectAsync(host, port).WaitAsync(cts.Token);

                stream = client.GetStream();
                _ = Task.Run(ReceiveMessages);

                Connected = true;

                SendMessage($"{(int)ServerCommands.FetchGameData}");

                return true;
            }
            catch
            {
                Disconnect();

                return false;
            }
        }

        public static void Disconnect()
        {
            client.Close();
            client = null;
            Connected = false;
        }

        private static async Task ReceiveMessages()
        {
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer);

                if (client == null)
                {
                    break;
                }

                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ProcessReceivedData(receivedData);
            }
        }

        /// <summary>
        /// [Message Length][Message Type][Payload] (Message structure)
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage(string message)
        {
            if (stream == null || stream == NetworkStream.Null)
            {
                return;
            }

            string temp = message + ";";

            byte[] bytes = Encoding.UTF8.GetBytes(temp);

            stream.Write(bytes);
        }

        public static void RequestIdentification(GameObject gameObjectToAssign)
        {
            string tempIdentification = Guid.NewGuid().ToString();
            tempIdentificationToGameObject.TryAdd(tempIdentification, gameObjectToAssign);
            SendMessage($"{(int)ServerCommands.FetchIdentification}:{tempIdentification}:{(int)gameObjectToAssign.ServerData.Type}");
        }

        private static void ProcessReceivedData(string data)
        {
            string[] objectDataArray = data.Split(';');

            foreach (string objectData in objectDataArray)
            {
                if (string.IsNullOrWhiteSpace(objectData))
                {
                    continue;
                }

                string[] parts = objectData.Split(':');

                MessageType receivedData;

                if (!int.TryParse(parts.First(), out int parsed))
                {
                    return;
                }

                receivedData = (MessageType)parsed;

                int sendersIdentification, type;
                float positionX, positionY;

                switch (receivedData)
                {
                    case MessageType.Identification:

                        if (!int.TryParse(parts[1], out int serversIdentification))
                        {
                            return;
                        }

                        string gameObjectsIdentifier = parts[2];

                        tempIdentificationToGameObject[gameObjectsIdentifier].ServerData.Identification = serversIdentification;
                        Library.IdentificationToGameObject.TryAdd(serversIdentification, tempIdentificationToGameObject[gameObjectsIdentifier]);
                        tempIdentificationToGameObject.Remove(gameObjectsIdentifier);

                        Library.IdentificationToGameObject[serversIdentification].tcs.SetResult(true); // Enables update to be called

                        break;
                    case MessageType.NewGameObject:

                        GameObjectServerData newGameObject = JsonSerializer.Deserialize<GameObjectServerData>(parts[1]);

                        switch (newGameObject.Type)
                        {
                            case GameObjectType.Player:
                                Library.CreateServerGameObject(new Player(newGameObject.Position), newGameObject.Identification);
                                break;
                            case GameObjectType.Enemy:
                                //Library.CreateServerGameObject(new Enemy(position), senderID);
                                break;
                            default:
                                break;
                        }

                        break;
                    case MessageType.Move:

                        if (!int.TryParse(parts[1], out sendersIdentification))
                        {
                            return;
                        }

                        if (!float.TryParse(parts[3], out positionX) || !float.TryParse(parts[4], out positionY))
                        {
                            return;
                        }

                        if (Library.IdentificationToGameObject.TryGetValue(sendersIdentification, out GameObject toBeMoved))
                        {
                            toBeMoved.SetPosition(new Vector2(positionX, positionY));
                        }

                        break;
                    case MessageType.DestroyGameObject:
                        if (!int.TryParse(parts[1], out sendersIdentification))
                        {
                            return;
                        }

                        if (Library.IdentificationToGameObject.TryGetValue(sendersIdentification, out GameObject toBeRemoved))
                        {
                            toBeRemoved.Destroy();
                        }
                        break;
                    case MessageType.Attack:

                        GameObject gameObject = Library.IdentificationToGameObject[int.Parse(parts[1])];

                        if (gameObject is IDamageable d)
                        {
                            d.Damage(float.Parse(parts[2])); // TODO: Add knockback and other effects
                        }

                        break;
                    case MessageType.Mine:
                        if (parts.Length != 5)
                        {
                            break;
                        }

                        Point Chunk = new(int.Parse(parts[1]), int.Parse(parts[2]));
                        Point tilePositionInChunk = new(int.Parse(parts[3]), int.Parse(parts[4]));

                        MapManager.SetTileInChunk(Chunk, tilePositionInChunk, null);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

public enum MessageType
{
    Identification,
    Move,
    Attack,
    NewGameObject,
    DestroyGameObject,
    Mine,
}

public enum ServerCommands
{
    FetchIdentification,
    ReleaseIdentification,
    Echo,
    FetchGameData
}
