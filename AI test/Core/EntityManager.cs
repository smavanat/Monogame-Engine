using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AI_test.Core
{
    public static class EntityManager
    {
        public static List<Component> GameObjects = new List<Component>();
        public static List<Sprite> sprites = new List<Sprite>();
        public static Quadtree quadtree = new Quadtree(0, new Rectangle(200, 200, 2250, 1500));

        public static void AddGameObject(Component component)
        {
            GameObjects.Add(component);
        }

        public static void AddSprite(Sprite sprite) 
        { 
            sprites.Add(sprite);
        }

        public static void RemoveGameObject(Component component)
        {
            GameObjects.Remove(component);
            component = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void Update(GameTime gameTime)
        {
            quadtree.Clear();

            foreach (Component component in GameObjects)
            {
                if (!sprites.Contains(component))
                    component.Update(gameTime);
            }
            for (int i = 0; i < sprites.Count; i++)
            {
                quadtree.Insert(sprites[i]);
            }
            List<Sprite> returnObjects = new List<Sprite>();
            for (int i = 0; i < sprites.Count; i++)
            {
                returnObjects.Clear();
                returnObjects = quadtree.Retrieve(sprites[i].Rectangle);
                for (int x = 0; x < returnObjects.Count; x++)
                {
                    if (returnObjects[x] == sprites[i])
                        continue;
                    else
                    {
                        if ((sprites[i].Velocity.X > 0 && IsTouchingLeft(sprites[i], returnObjects[x]) && returnObjects[x].IsCollideable) ||
                        (sprites[i].Velocity.X < 0 & IsTouchingRight(sprites[i], returnObjects[x]) && returnObjects[x].IsCollideable))
                        {
                            sprites[i].IsColliding = true;
                            break;
                        }
                        else if ((sprites[i].Velocity.Y > 0 && IsTouchingTop(sprites[i], returnObjects[x]) && returnObjects[x].IsCollideable) ||
                            (sprites[i].Velocity.Y < 0 & IsTouchingBottom(sprites[i], returnObjects[x]) && returnObjects[x].IsCollideable))
                        {
                            sprites[i].IsColliding = true;
                            break;
                        }
                        else
                        {
                            sprites[i].IsColliding = false;
                        }
                    }
                }
            }
            foreach(Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach(Component component in GameObjects)
            {
                component.Draw(gameTime, spriteBatch);
            }
        }

        #region Collision

        static bool IsTouchingLeft(Sprite sprite, Sprite other)
        {
            return sprite.Rectangle.Right + sprite.Velocity.X > other.Rectangle.Left &&
              sprite.Rectangle.Left < other.Rectangle.Left &&
              sprite.Rectangle.Bottom > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Bottom;
        }

        static bool IsTouchingRight(Sprite sprite, Sprite other)
        {
            return sprite.Rectangle.Left + sprite.Velocity.X < other.Rectangle.Right &&
              sprite.Rectangle.Right > other.Rectangle.Right &&
              sprite.Rectangle.Bottom > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Bottom;
        }

        static bool IsTouchingTop(Sprite sprite, Sprite other)
        {
            return sprite.Rectangle.Bottom + sprite.Velocity.Y > other.Rectangle.Top &&
              sprite.Rectangle.Top < other.Rectangle.Top &&
              sprite.Rectangle.Right > other.Rectangle.Left &&
              sprite.Rectangle.Left < other.Rectangle.Right;
        }

        static bool IsTouchingBottom(Sprite sprite, Sprite other)
        {
            return sprite.Rectangle.Top + sprite.Velocity.Y < other.Rectangle.Bottom &&
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
