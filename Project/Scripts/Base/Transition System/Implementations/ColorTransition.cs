using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class ColorTransition : BaseTransition
    {
        private readonly ISpriteRenderer spriteRenderer;

        private readonly Color startingColor, targetColor;

        public ColorTransition(ISpriteRenderer renderer, float duration, Color target, TransitionType type, ExecuteAfterTransition execute = null)
        {
            spriteRenderer = renderer;

            // How long this transition will take
            this.duration = duration;

            startingColor = renderer.Color;
            targetColor = target;

            // What transition type the transition will follow
            transitionType = type;

            // Add methods that will run after this transition
            Execute += execute;
        }

        public override void Start()
        {
            base.Start();

            spriteRenderer.Color = Color.Lerp(startingColor, targetColor, t);
        }

        public override void OnInstantTransition()
        {
            base.OnInstantTransition();

            spriteRenderer.Color = targetColor;
        }

        public override void RunAfterTransition()
        {
            base.RunAfterTransition();

            spriteRenderer.Color = targetColor;
        }
    }
}
