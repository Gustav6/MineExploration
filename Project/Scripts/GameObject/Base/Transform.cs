using Microsoft.Xna.Framework;
using System;

namespace MineExploration
{
    public class Transform
    {
        public Vector2 Position { get; protected set; }
        public float Rotation { get; private set; } = 0;
        public Vector2 Scale { get; private set; } = Vector2.One;
        public bool IsDestroyed { get; private set; } = false;

        public event EventHandler OnPositionChanged;

        public virtual void SetPosition(Vector2 position)
        {
            Position = position;
            OnPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SetRotation(float rotation) => Rotation = rotation;
        public virtual void SetScale(Vector2 scale) => Scale = scale;

        public virtual void Rotate(float rotation) => Rotation += rotation;

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
