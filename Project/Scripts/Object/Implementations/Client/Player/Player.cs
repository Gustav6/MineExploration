using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class Player : Moveable, IDamageable
    {
        #region IDamageable variables
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public bool CanTakeKnockBack => true;

        public Rectangle Hitbox { get; set; }

        public IDamageable.OnDeath RunOnDeath { get; set; }
        #endregion

        public Player(Vector2 position)
        {
            Position = position;

            Texture = TextureManager.Textures[TextureIdentifier.Player];
            SpriteLayer = TextureManager.SpriteLayers[SpriteLayerIdentifier.Player];

            ServerData = new() { Position = position, Type = GameObjectType.Player};
        }

        public override async Task Start()
        {
            await base.Start();

            UnlockMovement();
            movementSpeed = PlayerStats.BaseMovementSpeed;
            MaxHealth = PlayerStats.MaxHealth;
            Health = MaxHealth;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MoveDirection = new Vector2(KeyboardInput.Horizontal(), KeyboardInput.Vertical());

            if (MouseInput.HasBeenPressed(MouseKeys.Left))
            {
                // Code below will set the tile that the mouse clicks on to null

                //Point Chunk = Vector2.Floor(Camera.PositionInWorld(MouseInput.CurrentState.Position.ToVector2()) / MapManager.tileSize / MapManager.chunkSize).ToPoint();
                //Vector2 tilePosition = Camera.PositionInWorld(MouseInput.CurrentState.Position.ToVector2()) / MapManager.tileSize;
                //Point tilePositionInChunk = (tilePosition - Chunk.ToVector2() * MapManager.chunkSize).ToPoint();

                //MapManager.SetTileInChunk(Chunk, tilePositionInChunk, null);

                //ServerHandler.SendMessage($"{(int)ServerCommands.Echo}:{(int)DataSent.Mine}:{Chunk.X}:{Chunk.Y}:{tilePositionInChunk.X}:{tilePositionInChunk.Y}");
            }
        }

        #region IDamageable related methods
        public void Damage(float damageAmount)
        {
            Health -= damageAmount;
        }

        public void Heal(float healAmount)
        {
            Health += healAmount;
        }

        public void ApplyKnockBack(float strength, Vector2 direction)
        {

        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
