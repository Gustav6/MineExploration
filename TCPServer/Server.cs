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

namespace MineExploration
{
    public class Server
    {
        private readonly static Queue<int> availableIds = new();
        private static int nextObjectId = 1;

        private Dictionary<int, int> iDGameObjectPair = []; // Key: ID, Value: GameObject ID

        public bool IsRunning { get; private set; }
        private TcpListener listener;

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

        public static void ReleaseId(int id)
        {
            availableIds.Enqueue(id);
        }

        public void Start(int port, IPAddress address)
        {
            listener = new TcpListener(address, port);
            listener.Start();
            IsRunning = true;

            Console.WriteLine($"[SERVER] Server has started port on { port }, and using ip address { address }");

            Task.Run(AcceptClients);
        }

        private async Task AcceptClients()
        {
            while (IsRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                string clientId = Guid.NewGuid().ToString(); // or something else

                ClientManager.AddClient(clientId, client);

                Task.Run(() => HandleClient(client, clientId)); // Start handling this client in a new task
            }
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

        private async Task HandleClient(TcpClient client, string clientId)
        {
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

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[CLIENT] Client: { clientId } sent: [{ message }]");

                    // The first part of the variable below should always be the "method"
                    // This would mean that the message should be formatted as "METHOD:DATA"
                    string[] parts = message.Split(':');

                    if (parts.ElementAt(0).ToString() == "GET_ID")
                    {
                        int newId = NewClientId();
                        string response = $"ID:{ newId }";

                        iDGameObjectPair.TryAdd(newId, int.Parse(parts.ElementAt(1)));

                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);

                        ClientInfo? clientInfo = ClientManager.GetClient(clientId);
                        if (clientInfo != null)
                        {
                            ClientManager.SendMessage(clientInfo.Value, response);
                        }
                    }
                    else if (parts.ElementAt(0).ToString() == "BROADCAST")
                    {
                        // Send the message to all clients

                        int iD = int.Parse(parts.ElementAt(1));
                        string brodcastMessage = parts.ElementAt(1) + ":" + iDGameObjectPair[iD] + ":" + parts.ElementAt(2) + ":" + parts.ElementAt(3);

                        ClientManager.Broadcast(brodcastMessage, client);
                    }

                    // Echo the message back to the client
                    //byte[] response = Encoding.UTF8.GetBytes($"Server received: { receivedMessage }");
                    //await stream.WriteAsync(response, 0, response.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Found error with client { clientId }: { ex.Message }");
            }
            finally
            {
                ClientManager.RemoveClient(clientId);
            }
        }
    }
}
