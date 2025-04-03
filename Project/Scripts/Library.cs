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
        public static Dictionary<int, GameObject> serverIDGameObjectPair = [];

        public static GameObject CreateLocalGameObject(GameObject gameObject)
        {
            if (ServerHandler.Connected)
            {
                ServerHandler.RequestIdFromServer(gameObject);
            }

            localGameObjects.Add(gameObject);

            _ = gameObject.Start();

            return gameObject;
        }

        public static GameObject CreateServerGameObject(GameObject gameObject, int iD)
        {
            serverIDGameObjectPair.Add(iD, gameObject);

            gameObject.ServerID = iD;
            gameObject.tcs.SetResult(true);

            serverGameObjects.Add(gameObject);

            _ = gameObject.Start();

            return gameObject;
        }
    }
}
