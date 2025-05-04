using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class Library
    {
        public static Camera MainCamera { get; set; }

        public static Player playerInstance;

        public static List<GameObject> localGameObjects = [], serverGameObjects = [];
        public static Dictionary<int, GameObject> IdentificationToGameObject = [];

        public static GameObject CreateLocalGameObject(GameObject gameObject)
        {
            if (ServerHandler.Connected)
            {
                ServerHandler.RequestIdentification(gameObject);
            }
            else
            {
                Task.Delay(1000).ContinueWith(_ =>
                {
                    if (ServerHandler.Connected)
                    {
                        ServerHandler.RequestIdentification(gameObject);
                    }
                });
            }

            localGameObjects.Add(gameObject);

            _ = gameObject.Start();

            return gameObject;
        }

        public static GameObject CreateServerGameObject(GameObject gameObject, int identification)
        {
            if (IdentificationToGameObject.ContainsKey(identification))
            {
                return null;
            }

            IdentificationToGameObject.TryAdd(identification, gameObject);

            gameObject.ServerData.Identification = identification;

            serverGameObjects.Add(gameObject);

            _ = gameObject.Start();

            return gameObject;
        }
    }
}
