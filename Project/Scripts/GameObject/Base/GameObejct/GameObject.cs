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

        public GameObjectClientData serverData;

        public TaskCompletionSource<bool> tcs = new();
        private bool canRun = false;

        public GameObject()
        {
            serverData = new(this);
        }

        public virtual async Task Start()
        {
            if (Library.localGameObjects.Contains(this))
            {
                await tcs.Task;

                ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{serverData.DataForNewGameObject}");
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
        public override void Destroy()
        {
            if (Library.localGameObjects.Contains(this))
            {
                ServerHandler.SendMessage($"{(int)ServerCommands.ReleaseIdentification}:{serverData.identification}");
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects, SpriteLayer);
        }
    }

    public struct GameObjectClientData(GameObject gameObject)
    {
        public int identification;
        public GameObjectType Type;

        public const int moveDataLength = 4;
        public readonly string DataForMove
        {
            get
            {
                return $"{(int)DataSent.Move}:{identification}:{gameObject.Position.X}:{gameObject.Position.Y}";
            }
        }

        public const int newGameObjectDataLength = 5;
        public readonly string DataForNewGameObject
        {
            get
            {
                return $"{(int)DataSent.NewGameObject}:{identification}:{(int)Type}:{gameObject.Position.X}:{gameObject.Position.Y}";
            }

        }

        public const int attackDataLength = 3;
        public static string DataForAttack(int identificationForAffected, float damageAmount)
        {
            return $"{(int)DataSent.Attack}:{identificationForAffected}:{damageAmount}";
        }

        public const int destroyDataLength = 3;
        public static string DataForDestroy(int identificationForAffected)
        {
            return $"{(int)DataSent.DestroyGameObject}:{identificationForAffected}";
        }
    }
}

public enum GameObjectType
{
    Player,
    Enemy,
}
