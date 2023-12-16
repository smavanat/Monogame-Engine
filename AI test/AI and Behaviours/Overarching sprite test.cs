using AI_test.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.AI_and_Behaviours
{
    internal class Overarching_sprite_test : Sprite
    {
        private Vector2 origin;
        public Overarching_sprite_test(Texture2D _texture, Vector2 _position) : base(_texture, _position, 0)
        {
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            AddChild(new ChopFirewoodAction());
            AddChild(new DropOffFirewoodAction());
            AddChild(new Woodcutter(this));
            AddChild(new GOAPAgent(this));
        }

        public override void Update(GameTime gameTime){}

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
