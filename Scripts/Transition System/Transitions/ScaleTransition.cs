using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class ScaleTransition : BaseTransition
    {
        private readonly Vector2 startingScale, targetScale;

        public ScaleTransition(Transform t, float duration, Vector2 target, TransitionType type, ExecuteAfterTransition execute = null)
        {
            // The parameter transform will be moved
            transform = t;

            // How long this transition will take
            this.duration = duration;

            startingScale = t.Scale;
            targetScale = target;

            // What transition type the transition will follow
            transitionType = type;

            // Add methods that will run after this transition
            Execute += execute;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            transform.SetScale(Vector2.Lerp(startingScale, targetScale, t));
        }

        public override void OnInstantTransition()
        {
            base.OnInstantTransition();

            transform.SetScale(targetScale);
        }

        public override void RunAfterTransition()
        {
            base.RunAfterTransition();

            transform.SetScale(targetScale);
        }
    }
}
