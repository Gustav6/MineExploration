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

        public GameObjectData serverData;
        public bool IsDestroyed { get; private set; }

        public TaskCompletionSource<bool> tcs = new();
        private bool canRun = false;

        public virtual async Task Start()
        {
            await tcs.Task;

            serverData = new(this);

            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);

            canRun = true;

            ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{serverData.NewGameObjectData}");
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

            if (Library.localGameObjects.Contains(this))
            {
                ServerHandler.SendMessage($"{(int)ServerCommands.ReleaseID}:{serverData.ID}");
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }

    public struct GameObjectData(GameObject gameObject)
    {
        public int ID;
        public GameObjectType Type;

        public const int moveDataLength = 4;
        public readonly string MoveData
        {
            get
            {
                return $"{(int)DataSent.Move}:{ID}:{gameObject.Position.X}:{gameObject.Position.Y}";
            }
        }

        public const int newGameObjectDataLength = 5;
        public readonly string NewGameObjectData
        {
            get
            {
                return $"{(int)DataSent.NewGameObject}:{ID}:{(int)Type}:{gameObject.Position.X}:{gameObject.Position.Y}";
            }

        }

        public const int attackDataLength = 3;
        public static string AttackData(int iDForAffected, float damageAmount)
        {
            return $"{(int)DataSent.Attack}:{iDForAffected}:{damageAmount}";
        }

        public const int destroyDataLength = 3;
        public static string DestroyData(int iDForAffected)
        {
            return $"{(int)DataSent.DestroyGameObject}:{iDForAffected}";
        }
    }
}

public enum GameObjectType
{
    Player,
    Enemy,
}
