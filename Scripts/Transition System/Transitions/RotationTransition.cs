using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class RotationTransition : BaseTransition
    {
        private readonly float startingRotation, targetRotation;

        public RotationTransition(Transform t, float duration, float target, TransitionType type, ExecuteAfterTransition execute = null)
        {
            // The parameter transform will be rotated
            transform = t;

            // How long this transition will take
            this.duration = duration;

            startingRotation = t.Rotation;
            targetRotation = target;

            // What transition type the transition will follow
            transitionType = type;

            // Add methods that will run after this transition
            Execute += execute;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            transform.SetRotation(float.Lerp(startingRotation, targetRotation, t));
        }

        public override void OnInstantTransition()
        {
            base.OnInstantTransition();

            transform.SetRotation(targetRotation);
        }

        public override void RunAfterTransition()
        {
            base.RunAfterTransition();

            transform.SetRotation(targetRotation);
        }
    }
}
