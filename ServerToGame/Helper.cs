using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ServerToGame
{
    public class Vec2(float x, float y)
    {
        public float X { get; set; } = x;
        public float Y { get; set; } = y;
    }

    public enum ObjectType
    {
        Player,
        Enemy,
    }
}
