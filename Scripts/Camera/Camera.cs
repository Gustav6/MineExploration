using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        private Viewport viewport;
        private Vector2 center;

        private readonly float maximumZoom = 3;
        private readonly float minimumZoom = 0.05f;

        public float X { get { return center.X; } }
        public float Y { get { return center.Y; } }

        public GameObject target;

        private float zoom;
        public float Zoom
        {
            get { return zoom; }

            set
            {
                zoom = value;

                // Cap maximum and minimum zoom
                if (zoom < minimumZoom)
                {
                    zoom = minimumZoom;
                }
                else if (zoom > maximumZoom)
                {
                    zoom = maximumZoom;
                }
            }
        }

        private float rotation;
        public float Rotation
        {
            get { return rotation; }

            set
            {
                rotation = value;

                // Set rotation to 0 when camera has rotated 360°
                if (rotation > Math.PI * 2 || rotation < -Math.PI * 2)
                {
                    rotation = 0;
                }
            }
        }

        public Camera(Viewport viewport, GameObject follow = null)
        {
            this.viewport = viewport;
            target = follow;

            Zoom = 1;
            Rotation = 0;
        }

        public void Update()
        {
            if (target != null)
            {
                center = new Vector2(target.Position.X, target.Position.Y);
            }

            Transform =
                Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }

        public void ScreenShake(float duration, float intensity)
        {

        }
    }
}
