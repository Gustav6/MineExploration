using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class GameObjectServerData
    {
        public int Identification { get; set; }
        public GameObjectType Type { get; set; }
        public Vector2 Position { get; set; }
    }

    public enum GameObjectType
    {
        Player,
        Enemy,
    }
}
