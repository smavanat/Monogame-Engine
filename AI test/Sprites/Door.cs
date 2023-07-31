using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.Sprites
{
    public class Door : Sprite
    {
        private Vector2 origin;
        public Door(Texture2D _texture, Vector2 _position) : base(_texture, _position, 0, 3, false)
        { origin = new Vector2(texture.Width / 2, texture.Height / 2); IsCollideable = true; }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, this.Scale, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
