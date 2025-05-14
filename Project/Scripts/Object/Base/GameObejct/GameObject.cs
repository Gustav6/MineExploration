using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ServerToGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MineExploration
{
    public abstract class GameObject : Transform, ISpriteRenderer
    {
        #region Sprite renderer variables
        public Texture2D Texture { get; protected set; }
        public Vector2 Origin { get; private set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Source { get; private set; }
        public SpriteEffects SpriteEffects { get; private set; }
        public float SpriteLayer { get; protected set; } = TextureManager.SpriteLayers[SpriteLayerIdentifier.Default];
        #endregion

        public ObjectType Type { get; protected set; }
        public int ObjectIdentification { get; set; }

        public TaskCompletionSource<bool> serverSync = new();

        public async Task AwaitSeverSync()
        {
            await serverSync.Task;

            Start();
        }

        public virtual void Start()
        {
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }
}
