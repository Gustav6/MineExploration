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
        private Vector2 center;

        private readonly float maximumZoom = 3;
        private readonly float minimumZoom = 0.05f;

        public Vector2 Position { get { return center; } }
        public Transform Target { get; private set; }

        private float zoom;
        public float Zoom
        {
            get { return zoom; }

            set
            {
                // Cap maximum and minimum zoom
                if (value <= minimumZoom)
                {
                    zoom = minimumZoom;
                }
                else if (value >= maximumZoom)
                {
                    zoom = maximumZoom;
                }
                else
                {
                    zoom = value;
                }

                UpdateCamera();
            }
        }

        private float rotation;
        public float Rotation
        {
            get { return rotation; }

            set
            {
                // Set rotation to 0 when camera has rotated 360°
                if (value >= Math.PI * 2 || value <= -Math.PI * 2)
                {
                    rotation = 0;
                }
                else
                {
                    rotation = value;
                }

                UpdateCamera();
            }
        }

        public Camera(Transform target = null)
        {
            Target = target;

            Zoom = 1;
            Rotation = 0;

            UpdateCamera();

            Target.OnPositionChanged += Target_OnPositionChanged;
            WindowManager.OnWindowSizeChange += WindowManager_OnWindowSizeChange;
        }

        private void WindowManager_OnWindowSizeChange(object sender, EventArgs e)
        {
            UpdateCamera();
        }

        private void Target_OnPositionChanged(object sender, EventArgs e)
        {
            if (sender is Transform t)
            {
                Target = t;
                UpdateCamera();
                MapManager.CheckForNewActiveChunks();
            }
        }

        private void UpdateCamera()
        {
            if (Target != null)
            {
                center = Target.Position;
            }

            Transform =
                Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2, 0));
        }

        public static Vector2 PositionInWorld(Vector2 position)
        {
            Matrix inverseTransform = Matrix.Invert(Library.MainCamera.Transform);
            return Vector2.Transform(new Vector2(position.X, position.Y), inverseTransform);
        }

        public void SetTarget(Transform newTarget)
        {
            Target = newTarget;
            UpdateCamera();
        }

        public void ScreenShake(float duration, float intensity)
        {
            Rotation = 0;
        }
    }
}
