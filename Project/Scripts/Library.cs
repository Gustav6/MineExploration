using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerToGame;

namespace MineExploration
{
    public static class Library
    {
        public static Camera MainCamera { get; set; }

        public static Player playerInstance;

        public static List<GameObject> localGameObjects = []; // Handles local update calls
        public static Dictionary<int, GameObject> gameObjects = []; // Handles everything that the above does not

        public static Dictionary<string, GameObject> clientsIdentificationToGameObject = [];

        public static GameObject CreateLocalGameObject(GameObject g)
        {
            localGameObjects.Add(g);

            if (ServerManager.Connected)
            {
                string tempIdentification = Guid.NewGuid().ToString();

                NetworkMessage spawnRequest = new()
                {
                    Payload = new ObjectSpawnRequest()
                    {
                        Position = new Vec2(g.Position.X, g.Position.Y),
                        Size = new Vec2(g.Texture.Width, g.Texture.Height),
                        Type = g.Type,
                        TempIdentification = tempIdentification
                    },
                    Type = MessageType.ObjectSpawnRequest
                };

                ServerManager.SendMessage(spawnRequest);

                clientsIdentificationToGameObject.Add(tempIdentification, g);

                _ = g.AwaitSeverSync();
            }

            return g;
        }
    }
}
