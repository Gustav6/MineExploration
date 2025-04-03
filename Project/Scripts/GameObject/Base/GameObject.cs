using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public int ServerID { get; set; } = -1;
        public GameObjectType Type { get; protected set; }
        public bool IsDestroyed { get; private set; }

        public TaskCompletionSource<bool> tcs = new();
        private bool canRun = false;

        public virtual async Task Start()
        {
            await tcs.Task;

            ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{(int)DataSent.GameObject}:{ServerID}:{(int)Type}:{Position.X}:{Position.Y}");

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
        public void Destroy()
        {
            IsDestroyed = true;

            //if (ServerHandler.Connected)
            //{
            //    ServerHandler.SendMessage($"{ (int)ServerCommands.ReleaseID }:{ ServerID }");
            //}
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }
}

public enum GameObjectType
{
    Player,
    Enemy,
}
