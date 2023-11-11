using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Core;
using System.Diagnostics;

namespace AI_test.Sprites
{
    public class Door : Sprite
    {
        private Vector2 origin;
        private Collider trigger;
        public Door(Texture2D _texture, Vector2 _position) : base(_texture, _position, 0, false)
        { origin = new Vector2(texture.Width / 2, texture.Height / 2); 
            //IsCollideable = true; 
            trigger = new Collider(new Rectangle((int)_position.X, (int)_position.Y, texture.Width, texture.Height), this);
            trigger.CollisionEnter2D += Effect;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);  
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }

        //This is for  a feature that should be implemented sometime soon - player interactivity.
        //But this will take a while.
        public void Effect()
        {
            Debug.WriteLine("This works");
        }
    }
}
