using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public interface IDamageable
    {
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public bool CanTakeKnockBack { get; }

        public Rectangle Hitbox { get; set; }

        public delegate void OnDeath();
        public OnDeath RunOnDeath { get; set; }

        public void Damage(float damageAmount);
        public void Heal(float healAmount);
        public void ApplyKnockBack(float strength, Vector2 direction)
        {
            if (!CanTakeKnockBack)
            {
                return;
            }


        }
    }
}
