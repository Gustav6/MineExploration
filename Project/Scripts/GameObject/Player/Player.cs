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

        private Vector2 spawnPoint;

        public Player(Vector2 position)
        {
            spawnPoint = position;

            Texture = TextureManager.Textures[TextureIdentifier.Player];
            SpriteLayer = TextureManager.SpriteLayers[SpriteLayerIdentifier.Player];
            gameObjectData.Type = GameObjectType.Player;
        }

        public override async Task Start()
        {
            await base.Start();

            movementSpeed = PlayerStats.BaseMovementSpeed;
            MaxHealth = PlayerStats.MaxHealth;
            Health = MaxHealth;

            SetPosition(spawnPoint);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MoveDirection = new Vector2(KeyboardInput.Horizontal(), KeyboardInput.Vertical());
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
