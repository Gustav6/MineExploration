using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class BaseTroop : Moveable, IDamageable
    {
        #region Damagable interface variables
        private float health;
        public float Health
        {
            get => health;
            set
            {
                if (value > MaxHealth)
                {
                    health = MaxHealth;
                }
                else if (value <= 0)
                {
                    RunOnDeath.Invoke();
                }
                else
                {
                    health = value;
                }
            }
        }
        public float MaxHealth { get; set; }
        public IDamageable.OnDeath RunOnDeath { get; set; }
        public bool CanTakeKnockBack { get => true; }
        public Rectangle Hitbox { get; set; }
        #endregion

        public BaseTroop()
        {
            Texture = TextureManager.Textures[TextureIdentifier.Troop];
            SpriteLayer = TextureManager.SpriteLayers[SpriteLayerIdentifier.Troop];
        }
        public override void Start()
        {
            RunOnDeath += Destroy;

            Hitbox = new Rectangle(Position.ToPoint(), Texture.Bounds.Size);

            base.Start();
        }

        public virtual void Damage(float damageAmount)
        {
            Health -= damageAmount;
        }

        public virtual void Heal(float healAmount)
        {
            Health += healAmount;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Hitbox.Location.ToVector2(), Source, Color.Green * 0.5f, Rotation, Origin, Scale, SpriteEffects.None, TextureManager.SpriteLayers[SpriteLayerIdentifier.Hitbox]);

            base.Draw(spriteBatch);
        }
    }
}
