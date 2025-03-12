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
        private Vector2 spawnPosition;

        #region IDamageable variables
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public bool CanTakeKnockBack => true;

        public Rectangle Hitbox { get; set; }

        public IDamageable.OnDeath RunOnDeath { get; set; }
        #endregion

        public Player(Vector2 position)
        {
            spawnPosition = position;

            Texture = TextureManager.Textures[TextureIdentifier.Player];
            SpriteLayer = TextureManager.SpriteLayers[SpriteLayerIdentifier.Player];
            Type = GameObjectType.Player;
        }

        public override void Start()
        {
            SetPosition(spawnPosition);

            movementSpeed = PlayerStats.BaseMovementSpeed;
            MaxHealth = PlayerStats.MaxHealth;
            Health = MaxHealth;

            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            MoveDirection = new Vector2(KeyboardInput.Horizontal(), KeyboardInput.Vertical());

            if (MoveDirection != Vector2.Zero)
            {
                ServerHandler.SendMessage(Id + ":" + (int)Type + ":" + Position.X + ":" + Position.Y);
            }

            base.Update(gameTime);
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
