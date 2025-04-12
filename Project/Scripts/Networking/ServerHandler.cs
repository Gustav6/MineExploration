using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class ServerHandler
    {
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
                using (var cts = new CancellationTokenSource(timeoutMs))
                {
                    // Begin asynchronous connection
                    await client.ConnectAsync(host, port).WaitAsync(cts.Token);

                    stream = client.GetStream();
                    _ = Task.Run(ReceiveMessages);

                    Connected = true;

                    SendMessage($"{(int)ServerCommands.FetchGameData}");
                }

                return true;
            }
            catch (Exception)
            {
                Disconnect();

                Connected = true;

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
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (client == null)
                {
                    break;
                }

                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ProcessReceivedData(receivedData);
            }
        }

        public static void SendMessage(string message)
        {
            if (stream == null || stream == NetworkStream.Null)
            {
                return;
            }

            string temp = message + ";";

            byte[] messageBytes = Encoding.UTF8.GetBytes(temp);

            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public static void RequestIdentification(GameObject gameObjectToAssign)
        {
            string tempIdentification = Guid.NewGuid().ToString();
            tempIdentificationToGameObject.TryAdd(tempIdentification, gameObjectToAssign);
            SendMessage($"{(int)ServerCommands.FetchIdentification}:{tempIdentification}:{(int)gameObjectToAssign.serverData.Type}");
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

                DataSent receivedData;

                if (int.TryParse(parts.First(), out int parsedReceivedData))
                {
                    receivedData = (DataSent)parsedReceivedData;
                }
                else
                {
                    continue;
                }

                Console.WriteLine(receivedData);

                int sendersIdentification, type;
                Vector2 position;

                switch (receivedData)
                {
                    case DataSent.Identification:
                        if (parts.Length != 3) // To ensure the right amount of parts was sent
                        {
                            break;
                        }

                        int serversIdentification = int.Parse(parts[1]);
                        string gameObjectsIdentifier = parts[2];

                        tempIdentificationToGameObject[gameObjectsIdentifier].serverData.identification = serversIdentification;
                        Library.IdentificationToGameObject.TryAdd(serversIdentification, tempIdentificationToGameObject[gameObjectsIdentifier]);

                        tempIdentificationToGameObject[gameObjectsIdentifier].tcs.SetResult(true); // Enables update to be called
                        tempIdentificationToGameObject.Remove(gameObjectsIdentifier);

                        break;
                    case DataSent.NewGameObject:
                        if (parts.Length != GameObjectClientData.newGameObjectDataLength)
                        {
                            break;
                        }

                        sendersIdentification = int.Parse(parts[1]);
                        type = int.Parse(parts[2]);
                        position = new(float.Parse(parts[3]), float.Parse(parts[4]));

                        switch ((GameObjectType)type)
                        {
                            case GameObjectType.Player:
                                Library.CreateServerGameObject(new Player(position), sendersIdentification);
                                break;
                            case GameObjectType.Enemy:
                                //Library.CreateServerGameObject(new Enemy(position), senderID);
                                break;
                            default:
                                break;
                        }

                        break;
                    case DataSent.Move:
                        if (parts.Length != GameObjectClientData.moveDataLength)
                        {
                            break;
                        }

                        sendersIdentification = int.Parse(parts[1]);

                        position = new(float.Parse(parts[2]), float.Parse(parts[3]));

                        if (Library.IdentificationToGameObject.TryGetValue(sendersIdentification, out GameObject toBeMoved))
                        {
                            toBeMoved.SetPosition(position);
                        }

                        break;
                    case DataSent.DestroyGameObject:
                        sendersIdentification = int.Parse(parts[1]);

                        if (Library.IdentificationToGameObject.TryGetValue(sendersIdentification, out GameObject toBeRemoved))
                        {
                            toBeRemoved.Destroy();
                        }
                        break;
                    case DataSent.Attack:
                        if (parts.Length != GameObjectClientData.attackDataLength)
                        {
                            break;
                        }

                        GameObject gameObject = Library.IdentificationToGameObject[int.Parse(parts[1])];

                        if (gameObject is IDamageable d)
                        {
                            d.Damage(float.Parse(parts[2])); // TODO: Add knockback and other effects
                        }

                        break;
                    case DataSent.Mine:
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

public enum DataSent
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
