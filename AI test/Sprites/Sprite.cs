using AI_test.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.Sprites
{
    public class Sprite : Component
    {
        protected Texture2D texture;

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool IsCollideable { get; set; }
        public int Scale { get; set; }
        public Vector2 Velocity;
        public bool IsColliding;

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, texture.Width * Scale, texture.Height * Scale); }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch){}

        public Sprite(Texture2D _texture, Vector2 _position, float _rotation, int _scale, bool _isColliding)
        {
            this.texture = _texture;
            Position = _position;
            Rotation = _rotation;
            Scale = _scale;
            IsColliding = _isColliding;
            EntityManager.AddSprite(this);
        }

        public override void Update(GameTime gameTime){}
    }
}
