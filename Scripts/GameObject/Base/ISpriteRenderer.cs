using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

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

        public void SetColor(Color color) => Color = color;

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
