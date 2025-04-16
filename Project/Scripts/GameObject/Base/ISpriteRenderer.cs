using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineExploration
{
    public interface ISpriteRenderer
    {
        public Texture2D Texture { get; }
        public Vector2 Origin { get;}
        public Color Color { get; set; }
        public Rectangle Source { get; }
        public SpriteEffects SpriteEffects { get; }
        public float SpriteLayer { get; }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
