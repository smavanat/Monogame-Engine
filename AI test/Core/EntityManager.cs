using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;


namespace AI_test.Core
{
    //Manages all the entities in the game.
    public static class EntityManager
    {
        public static List<Component> GameObjects = new List<Component>();
        public static List<Sprite> sprites = new List<Sprite>();//Differentiate for Drawing
        public static List<Collider> colliders = new List<Collider>(); //Differentiate for collisions
        public static Quadtree quadtree = new Quadtree(0, new Rectangle(200, 200, 2250, 1500));

        static AutoResetEvent autoEvent = new AutoResetEvent(false);
        static Timer timer = new System.Threading.Timer(PhysicsUpdate, null, 0, 20);

        public static void AddGameObject(Component component)//Adds object to GameObject list
        {
            GameObjects.Add(component);
        }

        public static void AddSprite(Sprite sprite) //Adds object to Sprite list
        { 
            sprites.Add(sprite);
        }

        public static void AddCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        public static void RemoveGameObject(Component component)//Removes object from GameObject list
        {
            GameObjects.Remove(component);
            component = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        //Updates all items in the game simultaneously for ease of use.
        public static void Update(GameTime gameTime)
        {
            foreach (Component component in GameObjects)
            {
                    component.Update(gameTime);
            }
        }

        public static void PhysicsUpdate(object state)
        {
            //This is the quadtree implementation code found here: http://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374
            quadtree.Clear();

            for (int i = 0; i < colliders.Count; i++)
            {
                quadtree.Insert(colliders[i]);
            }
            List<Collider> returnObjects = new List<Collider>();
            for (int i = 0; i < colliders.Count; i++)
            {
                returnObjects.Clear();
                returnObjects = quadtree.Retrieve(colliders[i].Rectangle);

                Rectangle rect = colliders[i].Rectangle;
                rect.X += (int)colliders[i].Parent.Velocity.X;
                rect.Y += (int)colliders[i].Parent.Velocity.Y;
                for (int x = 0; x < returnObjects.Count; x++)
                {
                    if (returnObjects[x] == colliders[i])
                        continue;
                    else
                    {
                        if ((colliders[i].Parent.Velocity.X > 0 && IsTouchingLeft(colliders[i], returnObjects[x])) ||
                        (colliders[i].Parent.Velocity.X < 0 && IsTouchingRight(colliders[i], returnObjects[x])))
                        {
                            //colliders[i].IsCollidingHorizontally = true;
                            colliders[i].IsColliding = true;
                            //Vector2 d = returnObjects[x].Parent.Position - colliders[i].Parent.Position;
                            //float mx = d.X;
                            //float my = d.Y;
                            //Vector2 v;
                            //if(Math.Abs(mx) > Math.Abs(my))
                            //    v = new Vector2(mx, 0);
                            //else 
                            //    v = new Vector2(0, my);
                            //v = d - v;
                            //v.Normalize();
                            //colliders[i].ParentDirection = v;
                            break;
                        }
                        else if ((colliders[i].Parent.Velocity.Y > 0 && IsTouchingTop(colliders[i], returnObjects[x])) ||
                            (colliders[i].Parent.Velocity.Y < 0 && IsTouchingBottom(colliders[i], returnObjects[x])))
                        {
                            //colliders[i].IsCollidingVertically = true;
                            colliders[i].IsColliding = true;
                            break;
                        }
                        //if (rect.Intersects(returnObjects[x].Rectangle))
                        //    colliders[i].IsColliding = true;
                        else
                        {
                            //colliders[i].IsCollidingHorizontally = false;
                            //colliders[i].IsCollidingVertically = false;
                            colliders[i].IsColliding = false;
                        }
                    }
                }
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)//General Draw function
        {
            foreach(Sprite sprite in sprites)
            {
                sprite.Draw(gameTime, spriteBatch);
            }
        }



        #region Collision
        //This checks if objects are colliding by seeing if bounding boxes overlap.
        static bool IsTouchingLeft(Collider sprite, Collider other)
        {
            return sprite.Rectangle.Right + sprite.Parent.Velocity.X > other.Rectangle.Left &&
              sprite.Rectangle.Left < other.Rectangle.Left &&
              sprite.Rectangle.Bottom > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Bottom;
        }

        static bool IsTouchingRight(Collider sprite, Collider other)
        {
            return sprite.Rectangle.Left + sprite.Parent.Velocity.X < other.Rectangle.Right &&
              sprite.Rectangle.Right > other.Rectangle.Right &&
              sprite.Rectangle.Bottom > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Bottom;
        }

        static bool IsTouchingTop(Collider sprite, Collider other)
        {
            return sprite.Rectangle.Bottom + sprite.Parent.Velocity.Y > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Top &&
              sprite.Rectangle.Right > other.Rectangle.Left &&
              sprite.Rectangle.Left < other.Rectangle.Right;
        }

        static bool IsTouchingBottom(Collider sprite, Collider other)
        {
            return sprite.Rectangle.Top + sprite.Parent.Velocity.Y < other.Rectangle.Bottom &&
              sprite.Rectangle.Bottom > other.Rectangle.Bottom &&
              sprite.Rectangle.Right > other.Rectangle.Left &&
              sprite.Rectangle.Left < other.Rectangle.Right;
        }

        #endregion

    }
    public static class ObjectSpawner
    {
        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static void Load(Game1 instance)
        {
            textures.Add("Nodes", instance.Content.Load<Texture2D>("Node"));
            textures.Add("Door", instance.Content.Load<Texture2D>("Door"));
            textures.Add("Player", instance.Content.Load<Texture2D>("Operator"));
        }
    }
}
