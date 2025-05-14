using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerToGame;

namespace TCPServer
{
    public class Object(int id, ObjectType type, Vec2 startingPosition, Vec2 size)
    {
        public int Identification { get => id; }
        public string? clientsIdentification;
        public ObjectType Type { get => type; }

        public Vec2 Position { get; set; } = startingPosition;
        public Vec2 Velocity { get; set; } = new Vec2(0, 0);

        public Vec2 Size { get; set; } = size;
        public RectangleF BoundingBox => new(Position.X, Position.Y, Size.X, Size.Y);

        public bool IsDirty { get; private set; } = true;

        public void Move(Vec2 moveAmount)
        {
            Position.X += moveAmount.X;
            Position.Y += moveAmount.Y;
            IsDirty = true;
        }

        public void Update(float deltaTime)
        {
            if (Velocity != Vec2.Zero)
            {
                Move(new Vec2(Velocity.X * deltaTime, Velocity.Y * deltaTime));
            }
        }

        public void ClearDirtyFlag()
        {
            IsDirty = false;
        }
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
}
