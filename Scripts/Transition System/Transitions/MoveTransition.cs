using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class MoveTransition : BaseTransition
    {
        private readonly Vector2 startingPosition, targetPosition;

        private readonly bool targetInWorld;

        public MoveTransition(Transform t, float duration, Vector2 target, TransitionType type, bool targetInWorld = true, ExecuteAfterTransition execute = null)
        {
            // The parameter transform will be moved
            transform = t;

            // How long this transition will take
            this.duration = duration;

            startingPosition = t.Position;
            targetPosition = target;

            // What transition type the transition will follow
            transitionType = type;

            // Should position variables be local or in world
            this.targetInWorld = targetInWorld;

            // Add methods that will run after this transition
            Execute += execute;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (targetInWorld)
            {
                transform.SetPosition(Vector2.Lerp(startingPosition, targetPosition, t));
            }
            else
            {
                transform.SetPosition(Vector2.Lerp(startingPosition, startingPosition + targetPosition, t));
            }
        }

        public override void OnInstantTransition()
        {
            base.OnInstantTransition();

            if (targetInWorld)
            {
                transform.SetPosition(targetPosition);
            }
            else
            {
                transform.SetPosition(startingPosition + targetPosition);
            }
        }

        public override void RunAfterTransition()
        {
            base.RunAfterTransition();

            if (targetInWorld)
            {
                transform.SetPosition(targetPosition);
            }
            else
            {
                transform.SetPosition(startingPosition + targetPosition);
            }
        }
    }
}
