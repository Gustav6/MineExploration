using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class Data
    {
        // * ALL VARIABLES NEED TO BE PROPERTIES * \\

        public float BaseMovementSpeed { get; set; } = PlayerStats.BaseMovmentSpeed;
        public float MaxHealth { get; set; } = PlayerStats.MaxHealth;

        // Index 1 in array = x axes, index 2 in array = y axes
        public float[] SavedPosition { get; set; } = new float[2];
    }
}
