using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class UIGameObject : Transform, ISpriteRenderer
    {
        #region Sprite renderer variables
        public Texture2D Texture { get; protected set; }
        public Vector2 Origin { get; private set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Source { get; private set; }
        public SpriteEffects SpriteEffects { get; private set; }
        public float SpriteLayer { get; protected set; } = TextureManager.SpriteLayers[SpriteLayerIdentifier.UI];
        #endregion

        public virtual void Update(GameTime gameTime)
        {

        }

        public override void Destroy()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }
}
