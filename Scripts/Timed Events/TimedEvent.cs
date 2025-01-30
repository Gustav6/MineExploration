using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public struct TimedEvent(float duration, TimedEvent.TimerCallbackDelegate callBack)
    {
        public delegate void TimerCallbackDelegate();
        private readonly TimerCallbackDelegate TimerCallback => callBack;

        private float timer = duration;

        public void Update(GameTime gameTime)
        {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0)
            {
                TimerCallback?.Invoke();
                TimedEventSystem.timers.Remove(this);
            }
        }
    }
}
