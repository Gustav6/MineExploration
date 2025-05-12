using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public abstract class BaseTransition
    {
        public delegate void ExecuteAfterTransition();
        public ExecuteAfterTransition Execute { get; protected set; }

        protected Transform transform;

        protected TransitionType transitionType;

        private protected float curveInterval;
        private protected Vector2 curveOffset, curveAmplitude;

        public bool Loop { get; set; }
        public bool IsRemoved { get; protected set; }

        protected float timer, duration, t;

        public virtual void Start()
        {
            if (Loop)
            {
                duration = 1;
            }
            else
            {
                if (duration <= 0)
                {
                    OnInstantTransition();
                    return;
                }
            }

            Execute += RunAfterTransition;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (transform == null)
            {
                IsRemoved = true;
                return;
            }

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!Loop && timer >= duration)
            {
                IsRemoved = true;
            }

            t = transitionType switch
            {
                TransitionType.SmoothStart2 => TransitionSystem.SmoothStart2(timer / duration),
                TransitionType.SmoothStart3 => TransitionSystem.SmoothStart3(timer / duration),
                TransitionType.SmoothStart4 => TransitionSystem.SmoothStart4(timer / duration),
                TransitionType.SmoothStop2 => TransitionSystem.SmoothStop2(timer / duration),
                TransitionType.SmoothStop3 => TransitionSystem.SmoothStop3(timer / duration),
                TransitionType.SmoothStop4 => TransitionSystem.SmoothStop4(timer / duration),
                _ => 0,
            };
        }

        public virtual void OnInstantTransition()
        {
            if (transform == null)
            {
                return;
            }

            IsRemoved = true;
        }
        public virtual void RunAfterTransition()
        {
            if (transform == null)
            {
                return;
            }
        }
    }
}
