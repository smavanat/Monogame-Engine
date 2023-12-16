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
    public class SupplyPile : Sprite
    {
        public int numFirewood = 0;
        Vector2 origin;
        
        public SupplyPile(Texture2D _image, Vector2 position, float _rotation) : base(_image, position, _rotation) {
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public override void Update(GameTime gameTime){}

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1, SpriteEffects.None, 0f);
        }
    }
}
