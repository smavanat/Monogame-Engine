using AI_test.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.Core
{
    //This is a camera class that follows the player.
    public static class Camera
    {
        public static Matrix Transform { get; private set; }
        public static float zoom = 4f;//Camera Zoom
        public static float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } // Negative zoom will flip image
        }

        public static void Follow(Sprite target)
        {
            //To follow a target (Most likely the player)
            var position = Matrix.CreateTranslation(
              -target.Position.X - (target.Rectangle.Width / 2),
              -target.Position.Y - (target.Rectangle.Height / 2),
              0);

            //Camera position relative to the game screen
            var offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);
            var scale = Matrix.CreateScale( Zoom, Zoom, 1 );

            Transform = position * scale * offset;
        }
        //Sets the Draw method to work with the Camera.
        public static  Vector2 ScreenToWorldSpace(in Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(Transform);
            return Vector2.Transform(point, invertedMatrix);
        }
    }
}
