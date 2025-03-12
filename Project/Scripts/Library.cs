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

        public static List<GameObject> gameObjects = [];
        public static Dictionary<int, GameObject> serverGameObjects = [];

        public static async Task<GameObject> CreateLocalGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);

            await FetchNewGameObjectID(gameObject);

            gameObject.Start();

            return gameObject;
        }

        public static async Task<GameObject> CreateServerGameObject(GameObject gameObject, int id = -1)
        {
            if (id == -1)
            {
                if (await FetchNewGameObjectID(gameObject))
                {
                    serverGameObjects.Add(gameObject.Id, gameObject);

                    gameObject.Start();

                    return gameObject;
                }
            }
            else
            {
                serverGameObjects.Add(id, gameObject);

                gameObject.Start();

                return gameObject;
            }

            return null;
        }

        private static async Task<bool> FetchNewGameObjectID(GameObject gameObject)
        {
            int newId = await ServerHandler.RequestIdFromServer();

            if (newId == -1)
            {
                Console.WriteLine("Failed to get an ID from the server.");
                return false;
            }

            gameObject.Id = newId;

            return true;
        }
    }
}
