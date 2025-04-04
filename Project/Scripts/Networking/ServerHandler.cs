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
        private static readonly Dictionary<string, GameObject> tempLocalIDPair = [];

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

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public static void RequestIdFromServer(GameObject gameObjectToAssign)
        {
            string tempID = Guid.NewGuid().ToString();
            tempLocalIDPair.TryAdd(tempID, gameObjectToAssign);
            SendMessage($"{ (int)ServerCommands.FetchID }:{ tempID }");
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

                DataSent receivedData = (DataSent)int.Parse(parts[0]);


                int senderID, type;
                Vector2 position;

                switch (receivedData)
                {
                    case DataSent.ID:
                        if (parts.Length != 3) // To ensure the right amount of parts was sent
                        {
                            break;
                        }

                        string gameObjectsIdentifier = parts[2];
                        int serverID = int.Parse(parts[1]);

                        tempLocalIDPair[gameObjectsIdentifier].gameObjectData.ID = serverID;
                        Library.serverIDGameObjectPair.TryAdd(serverID, tempLocalIDPair[gameObjectsIdentifier]);

                        tempLocalIDPair[gameObjectsIdentifier].tcs.SetResult(true); // Enables update to be called
                        tempLocalIDPair.Remove(gameObjectsIdentifier);

                        break;
                    case DataSent.NewGameObject:
                        if (parts.Length != GameObjectData.newGameObjectDataLength)
                        {
                            break;
                        }

                        senderID = int.Parse(parts[1]);
                        type = int.Parse(parts[2]);

                        position = new(float.Parse(parts[3]), float.Parse(parts[4]));


                        switch ((GameObjectType)type)
                        {
                            case GameObjectType.Player:
                                Library.CreateServerGameObject(new Player(position), senderID);
                                break;
                            case GameObjectType.Enemy:
                                //Library.CreateServerGameObject(new Enemy(position), senderID);
                                break;
                            default:
                                break;
                        }

                        break;
                    case DataSent.Move:
                        if (parts.Length != GameObjectData.moveDataLength)
                        {
                            break;
                        }

                        senderID = int.Parse(parts[1]);

                        position = new(float.Parse(parts[2]), float.Parse(parts[3]));

                        if (Library.serverIDGameObjectPair.TryGetValue(senderID, out GameObject toBeMoved))
                        {
                            toBeMoved.SetPosition(position);
                        }
                        break;
                    case DataSent.DestroyGameObject:
                        senderID = int.Parse(parts[1]);

                        if (Library.serverIDGameObjectPair.TryGetValue(senderID, out GameObject toBeRemoved))
                        {
                            toBeRemoved.Destroy();
                        }
                        break;
                    case DataSent.Attack:
                        if (parts.Length != GameObjectData.attackDataLength)
                        {
                            break;
                        }

                        GameObject gameObject = Library.serverIDGameObjectPair[int.Parse(parts[1])];

                        if (gameObject is IDamageable d)
                        {
                            d.Damage(float.Parse(parts[2])); // TODO: Add knockback and other effects
                        }

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
    ID,
    Move,
    Attack,
    NewGameObject,
    DestroyGameObject,
}

public enum ServerCommands
{
    FetchID,
    ReleaseID,
    Echo,
}
