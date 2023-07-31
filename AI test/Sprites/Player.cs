using AI_test.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.Sprites
{
    public class Player : Sprite
    {
        private int speed = 3;
        private Vector2 origin;
        private MouseState oldMouseState;

        public Player(Texture2D _texture, Vector2 _position):base(_texture, _position, 0, 5, false) 
        { origin = new Vector2(texture.Width / 2, texture.Height / 2); IsCollideable = true; }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            Vector2 dPos = (Position - Camera.ScreenToWorldSpace(mousePosition));
            Rotation = (float)Math.Atan2(dPos.Y, dPos.X);

            Velocity = new Vector2();
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Velocity.Y = -speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Velocity.Y = speed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Velocity.X = -speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Velocity.X = speed;

            if (IsColliding)
            {
                Position += Vector2.Zero;
            }
            else
            {
                Position += Velocity;
            }
            oldMouseState = mouseState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, this.Scale, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
