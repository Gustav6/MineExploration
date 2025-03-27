using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public int ServerId { get; set; } = -1;
        public GameObjectType Type { get; protected set; }

        GameObjectData gameObjectData;

        public bool IsDestroyed { get; private set; }

        public virtual void Start()
        {
            gameObjectData = new GameObjectData
            {
                serverId = ServerId,
                type = Type,
                positionX = (int)Position.X,
                positionY = (int)Position.Y
            };

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);

            ServerHandler.OnServerConnect += ServerHandler_OnServerConnect;

            if (ServerHandler.ConnectedToServer)
            {
                Task.Run(() => SendToServerOnConnect());
            }
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void RunOnDestroy() { }
        public void Destroy()
        {
            IsDestroyed = true;
        }

        private void ServerHandler_OnServerConnect(object sender, EventArgs e)
        {
            Task.Run(() => SendToServerOnConnect());
        }

        public virtual async Task SendToServerOnConnect() { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }

    public struct GameObjectData
    {
        public int serverId;
        public GameObjectType type;
        public int positionX, positionY;
    }
}

public enum GameObjectType
{
    Player,
    Enemy,
}
