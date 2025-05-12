using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class TimedEventSystem
    {
        public static List<TimedEvent> timers = [];

        public static void UpdateTimers(GameTime gameTime)
        {
            foreach (TimedEvent timer in timers)
            {
                timer.Update(gameTime);
            }
        }

        public static void StartTimer(float duration, TimedEvent.TimerCallbackDelegate callback) => timers.Add(new TimedEvent(duration, callback));
    }
}
