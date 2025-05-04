using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public GameObjectServerData ServerData { get; set; }

        public TaskCompletionSource<bool> tcs = new();
        private bool canRun = false;

        public virtual async Task Start()
        {
            if (Library.localGameObjects.Contains(this))
            {
                await tcs.Task;

                ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{(int)MessageType.NewGameObject}:{DataSerialized()}");
            }

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);

            canRun = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!canRun)
            {
                return;
            }
        }

        public virtual void RunOnDestroy() { }
        public override void Destroy() { }

        public string DataSerialized()
        {
            return JsonSerializer.Serialize(ServerData);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }
}
