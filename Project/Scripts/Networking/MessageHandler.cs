using ServerToGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MineExploration
{
    public static class MessageHandler
    {
        public static void ProcessMessage(NetworkMessage message)
        {
            switch (message.Type)
            {
                case MessageType.ObjectSpawnRequest:
                    if (message.Payload is not ObjectSpawnRequest req)
                    {
                        return;
                    }

                    GameObject serverObject = null;
                    switch (req.Type)
                    {
                        case ObjectType.Player:
                            serverObject = new Player(new Vector2(req.Position.X, req.Position.Y));
                            break;
                        case ObjectType.Enemy:
                            //serverObject = new Enemy(new Vector2(req.Position.X, req.Position.Y));
                            break;
                        default:
                            break;
                    }

                    if (serverObject == null)
                    {
                        return;
                    }

                    serverObject.ObjectIdentification = req.ObjectIdentification;
                    serverObject.serverSync.SetResult(true);

                    Library.gameObjects.Add(serverObject.ObjectIdentification, serverObject);

                    break;
                case MessageType.DestroyObject:
                    if (message.Payload is not DestroyObject des)
                    {
                        return;
                    }

                    Library.gameObjects[des.ObjectIdentification].RunOnDestroy();

                    Library.localGameObjects.Remove(Library.gameObjects[des.ObjectIdentification]);
                    Library.gameObjects.Remove(des.ObjectIdentification);

                    break;
                case MessageType.UpdateObject:
                    if (message.Payload is not ObjectUpdate upd)
                    {
                        return;
                    }

                    if (upd.ClientIdentification != null)
                    {
                        GameObject localObject = Library.clientsIdentificationToGameObject[upd.ClientIdentification];

                        localObject.ObjectIdentification = upd.ObjectIdentification;
                        localObject.serverSync.SetResult(true);

                        Library.gameObjects.Add(localObject.ObjectIdentification, localObject);

                        Library.clientsIdentificationToGameObject.Remove(upd.ClientIdentification);
                    }

                    Library.gameObjects[upd.ObjectIdentification].SetPosition(new Vector2(upd.Position.X, upd.Position.Y));

                    break;
                default:
                    break;
            }
        }
    }
}