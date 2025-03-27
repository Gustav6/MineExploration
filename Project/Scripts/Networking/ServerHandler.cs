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
        public static event EventHandler OnServerConnect;
        public static bool ConnectedToServer { get; private set; } = false;

        /// <summary>
        /// Tries to connect to the specified TCP server within a given timeout.
        /// </summary>
        /// <param name="host">The IP or hostname of the server.</param>
        /// <param name="port">The TCP port of the server.</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <param name="client">An out parameter that returns the connected TcpClient if successful.</param>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        public static async Task<bool> TryConnect(string host, int port, int timeoutMs)
        {
            if (client != null)
            {
                CloseConnection();
            }

            client = new TcpClient();
            try
            {
                // Begin asynchronous connect
                IAsyncResult asyncResult = client.BeginConnect(host, port, null, null);

                // Wait for the connection or timeout
                bool success = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMs));

                ConnectedToServer = success;

                if (!ConnectedToServer)
                {
                    // Timed out
                    CloseConnection();
                    return false;
                }

                stream = client.GetStream();

                Task.Run(ReceiveMessages);

                // Complete the connection
                client.EndConnect(asyncResult);
                OnServerConnect?.Invoke(null, EventArgs.Empty);

                if (Library.playerInstance != null)
                {
                    await Library.FetchNewGameObjectID(Library.playerInstance);
                }

                return true;
            }
            catch (Exception)
            {
                CloseConnection();
                return false;
            }
        }

        public static void CloseConnection()
        {
            client.Close();
            client = null;
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
                //Debug.WriteLine($"Received: {receivedData}");
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

        public static async Task<int> RequestIdFromServer(GameObject gameObjectToAssign)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes("GET_ID" + ":" + (int)gameObjectToAssign.Type);

                // Create the response buffer
                byte[] responseBuffer = new byte[1024];

                // Start reading response before sending request
                Task<int> readTask = stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);

                // Send the request
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();

                // Wait for response to arrive
                int bytesRead = await readTask;
                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                // Parse response
                if (response.StartsWith("ID:"))
                {
                    string[] parts = response.Split(':');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int newId))
                    {
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Error requesting ID: { ex.Message }");
            }

            return -1; // Return -1 if failed
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

                ServerDataType recivedServerDataType = (ServerDataType)int.Parse(parts[0]);

                switch (recivedServerDataType)
                {
                    case ServerDataType.ID:


                        break;
                    case ServerDataType.GameObject:

                        int senderId = int.Parse(parts[0]);
                        int type = int.Parse(parts[1]);
                        float x = float.Parse(parts[2]);
                        float y = float.Parse(parts[3]);

                        Vector2 position = new(x, y);

                        if (Library.serverGameObjects.TryGetValue(senderId, out GameObject gameObject))
                        {
                            gameObject.SetPosition(position);
                            return;
                        }

                        switch ((GameObjectType)type)
                        {
                            case GameObjectType.Player:
                                Library.CreateServerGameObject(new Player(position), senderId);
                                break;
                            case GameObjectType.Enemy:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

public enum ServerDataType
{
    ID,
    GameObject
}
