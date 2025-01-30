using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class Library
    {
        public static Camera Camera { get; set; }

        public static List<GameObject> gameObjects = [];
        public static void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            gameObject.Start();
        }
    }
}
