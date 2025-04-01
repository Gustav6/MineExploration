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

        public static List<GameObject> localGameObjects = [];
        public static Dictionary<int, GameObject> serverGameObjects = [];

        public static GameObject CreateLocalGameObject(GameObject gameObject)
        {
            localGameObjects.Add(gameObject);

            gameObject.Start();

            return gameObject;
        }

        public static async Task<GameObject> CreateServerGameObject(GameObject gameObject, int id = -1)
        {
            //if (id == -1)
            //{
            //    if (await FetchNewGameObjectID(gameObject))
            //    {
            //        serverGameObjects.Add(gameObject.ServerId, gameObject);

            //        gameObject.Start();

            //        return gameObject;
            //    }
            //}
            //else
            //{
            //    serverGameObjects.Add(id, gameObject);

            //    gameObject.Start();

            //    return gameObject;
            //}

            return null;
        }
    }
}
