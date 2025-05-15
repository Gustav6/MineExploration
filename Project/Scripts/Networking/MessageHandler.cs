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
                case MessageType.AssignServerIdentification:
                    if (message.Payload is not AssignServerIdentification ser)
                    {
                        return;
                    }

                    if (Library.localIdentificationToGameObject.TryGetValue(ser.LocalIdentification, out GameObject l))
                    {
                        l.ObjectIdentification = ser.ObjectIdentification;
                        l.serverSync.SetResult(true);

                        Library.gameObjects.Add(l.ObjectIdentification, l);

                        Library.localIdentificationToGameObject.Remove(ser.LocalIdentification);
                    }

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

                    // Check if the object already exists
                    if (Library.gameObjects.TryGetValue(upd.ObjectIdentification, out GameObject g))
                    {
                        // Update gameobjects
                        g.SetPosition(new Vector2(upd.Position.X, upd.Position.Y));
                    }
                    else
                    {
                        GameObject serverObject = null;

                        switch (upd.Type)
                        {
                            case ObjectType.Player:
                                serverObject = new Player(new Vector2(upd.Position.X, upd.Position.Y));
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

                        serverObject.ObjectIdentification = upd.ObjectIdentification;
                        serverObject.serverSync.SetResult(true);

                        Library.gameObjects.Add(serverObject.ObjectIdentification, serverObject);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}