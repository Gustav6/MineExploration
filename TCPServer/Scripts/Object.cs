using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class Object(int id, ObjectType type, Vector2 position, Vector2 size)
    {
        public int Id { get => id; }
        public ObjectType Type { get => type; }
        public Vector2 Position { get; set; } = position;
        public Vector2 Size { get; set; } = size;
        public RectangleF BoundingBox => new(Position.X, Position.Y, Size.X, Size.Y);
    }

    /// <summary>
    /// XNA RectangleF struct to represent a rectangle with float coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public struct RectangleF(float x, float y, float w, float h)
    {
        public float X = x, Y = y, Width = w, Height = h;
        public readonly float Left => X;
        public readonly float Right => X + Width;
        public readonly float Top => Y;
        public readonly float Bottom => Y + Height;

        public bool Intersects(RectangleF other)
        {
            return !(Right <= other.Left || Left >= other.Right || Bottom <= other.Top || Top >= other.Bottom);
        }
    }

    public enum ObjectType
    {
        Player,
        Enemy,
    }
}
