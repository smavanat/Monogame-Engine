using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Core;

namespace AI_test.Sprites
{
    public class Agent : Sprite
    {
        public Vector2 targetPosition;

        private Vector2 direction;
        private MouseState oldMouseState;
        private int speed = 50;

        public Agent(Texture2D _texture, Vector2 _position, float _rotation) : base(_texture, _position, _rotation, false)
        {
            targetPosition = _position;
        }

        public void agentUpdate(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                targetPosition = new Vector2(mouseState.X, mouseState.Y);

            if (Vector2.Distance(Position, targetPosition) > 1)
            {
                Position = SteeringBehaviours.Seek(Position, targetPosition, gameTime, speed);
            }
            else
            {
                direction = Vector2.Zero;
            }
        }
        public override void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            int width = texture.Width;
            int height = texture.Height;

            Rectangle sourceRectangle = new Rectangle(width, height, width, height); //Image within the texture we want to draw
            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height); //Where we want to draw the texture within the game

            spriteBatch.Draw(texture, Position, Color.White);
        }

        #region StateMachine
        void Move()
        {

        }
        #endregion
    }
}
