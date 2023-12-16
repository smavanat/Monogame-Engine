using AI_test.Core;
using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.ToolsIIStorageIIPrefabs
{
    public class ChoppingBlock : Sprite
    {
        private Vector2 origin;
        public ChoppingBlock(Texture2D _texture, Vector2 _position, float _rotation) : base(_texture, _position, _rotation) {
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
        public override void Update(GameTime gameTime){}

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1, SpriteEffects.None, 0f);
        }
    }
}