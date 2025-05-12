using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class PayloadHandler
    {

        public static void HandlePayload(byte[] data, Client client)
        {
            Message message = Message.FromBytes(data);

            switch (message.Type)
            {
                case MessageType.CreateGameObject:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                case MessageType.DestroyGameObject:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                case MessageType.UpdateGameObject:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                case MessageType.FetchGameData:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                case MessageType.ClientDisconnect:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                case MessageType.ClientConnect:
                    //if (!message.Payload is ...)
                    //{
                    //    return;
                    //}

                    break;
                default:
                    break;
            }

            Server.Log($"Received: {message}, from: [{client.Identification}]", LogType.Client);
        }
    }
}
