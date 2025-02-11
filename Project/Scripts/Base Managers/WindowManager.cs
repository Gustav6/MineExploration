using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public static class WindowManager
    {
        public static event EventHandler OnWindowSizeChange;

        public static int WindowWidth { get; private set; }
        public static int WindowHeight { get; private set; }

        public static readonly int defaultWindowWidth = 1920;
        public static readonly int defaultWindowHeight = 1080;

        public static int WindowScaling { get; private set; }

        public static void ChangeSize(int width, int height)
        {
            WindowHeight = height;
            WindowWidth = width;

            Game1.Graphics.PreferredBackBufferWidth = WindowWidth;
            Game1.Graphics.PreferredBackBufferHeight = WindowHeight;

            Game1.Graphics.ApplyChanges();

            WindowScaling = WindowWidth / defaultWindowWidth;

            OnWindowSizeChange?.Invoke(null, EventArgs.Empty);
        }

        public static void Fullscreen(bool status)
        {
            Game1.Graphics.IsFullScreen = status;

            Game1.Graphics.ApplyChanges();
        }
    }
}
