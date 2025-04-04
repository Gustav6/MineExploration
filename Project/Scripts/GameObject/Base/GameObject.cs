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

        public GameObjectData gameObjectData = new();
        public bool IsDestroyed { get; private set; }

        public TaskCompletionSource<bool> tcs = new();
        private bool canRun = false;

        public virtual async Task Start()
        {
            await tcs.Task;

            ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{gameObjectData.NewGameObjectData}");

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

    public struct GameObjectData
    {
        public int ID;
        public GameObjectType Type;
        public Vector2 Position;

        public readonly string MoveData
        {
            get
            {
                return $"{(int)DataSent.Move}:{ID}:{Position.X}:{Position.Y}";
            }
        }
        public const int moveDataLength = 4;

        public readonly string NewGameObjectData
        {
            get
            {
                return $"{(int)DataSent.NewGameObject}:{ID}:{(int)Type}:{Position.X}:{Position.Y}";
            }

        }
        public const int newGameObjectDataLength = 5;

        public static string AttackData(int iDForAffected, float damageAmount)
        {
            return $"{(int)DataSent.Attack}:{iDForAffected}:{damageAmount}";
        }
        public const int attackDataLength = 3;
    }
}

public enum GameObjectType
{
    Player,
    Enemy,
}
