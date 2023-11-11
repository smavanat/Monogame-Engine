using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI_test.Core
{
    //This is a general rectangle collider class that handles collisions.
    public class Collider : Component
    {
        //public bool IsCollidingHorizontally = false;
        //public bool IsCollidingVertically = false;
        public bool IsColliding = false;
        public delegate void Test();
        public event Test CollisionEnter2D;
        public Sprite Parent;
        //public Vector2 ParentDirection = Vector2.Zero;

        public Collider(Rectangle _collider, Sprite parent)
        {
            Parent = parent;
            EntityManager.AddCollider(this);
        }

        public override void Update(GameTime gameTime)
        {
            if(IsColliding)
                OnCollisionEnter2D();
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Parent.Position.X, (int)Parent.Position.Y, Parent.texture.Width, Parent.texture.Height); }
        }

        //public void test()
        //{
        //    OnCollisionEnter2D();
        //}

        protected virtual void OnCollisionEnter2D()
        {
            CollisionEnter2D?.Invoke();//Using c# events
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {}
    }
}
