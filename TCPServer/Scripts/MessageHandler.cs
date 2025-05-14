using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerToGame;

namespace TCPServer
{
    public class MessageQueue
    {
        public static readonly ConcurrentQueue<(Client client, NetworkMessage message)> queue = [];
        public static int Count => queue.Count;

        public static void Enqueue(Client client, NetworkMessage message)
        {
            queue.Enqueue((client, message));
        }

        public static bool TryDequeue(out (Client client, NetworkMessage message) result)
        {
            return queue.TryDequeue(out result);
        }
    }

    public class MessageHandler
    {
        public static void ProcessMessages()
        {
            while (MessageQueue.queue.TryDequeue(out var item))
            {
                Client client = item.client;
                NetworkMessage message = item.message;

                Server.Log($"Received: [{message.Type}], from: [{client.Identification}]", LogType.Server);

                switch (message.Type)
                {
                    case MessageType.ObjectSpawnRequest:
                        if (message.Payload is ObjectSpawnRequest spawnRequest)
                        {
                            HandleSpawnRequest(spawnRequest, client);
                        }
                        break;
                    case MessageType.DestroyObject:
                        if (message.Payload is DestroyObject destroyRequest)
                        {
                            HandleDestroyRequest(destroyRequest);
                        }
                        break;
                    case MessageType.MoveGameObject:
                        if (message.Payload is ObjectMoveRequest moveRequest)
                        {
                            HandleMoveRequest(moveRequest);
                        }
                        break;
                    case MessageType.FetchGameData:
                        //if (!message.Payload is ...)
                        //{
                        //    return;
                        //}

                        break;
                    case MessageType.ClientDisconnect:
                        break;
                    case MessageType.ClientConnect:
                        break;
                    default:
                        break;
                }
            }
        }

        private static void HandleSpawnRequest(ObjectSpawnRequest request, Client client)
        {
            Object obj = new(ObjectManager.GetObjectIdentification(), request.Type, request.Position, request.Size)
            {
                clientsIdentification = request.TempIdentification
            };

            ObjectManager.Add(obj);

            client.attachedIdentifications.Add(obj.Identification);
        }

        private static void HandleDestroyRequest(DestroyObject request)
        {
            ObjectManager.Destroy(request.ObjectIdentification);
        }

        private static void HandleMoveRequest(ObjectMoveRequest request)
        {
            Object? obj = ObjectManager.Get(request.ObjectIdentification);

            if (obj != null)
            {
                obj.Velocity = new Vec2(request.Direction.X * request.speed, request.Direction.Y * request.speed);
            }
        }
    }
}
