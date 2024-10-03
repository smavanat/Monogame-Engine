using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AI_test.Sprites;

namespace AI_test.ToolsIIStorageIIPrefabs
{
    internal class Tree : Sprite
    {
        private Vector2 origin;
        public Tree(Texture2D _texture, Vector2 _position, float _rotation) : base(_texture, _position, _rotation)
        {
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1, SpriteEffects.None, 0f);
        }
    }
}
