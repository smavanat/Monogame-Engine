using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AI_test.Core
{
    public abstract class Entity
    {
        Vector2 Position;
        public Entity parent;
        public List<Object> children;

        Entity(Vector2 _position, Entity _parent = null) { 
            Position = _position;
            //EntityManager.AddGameObject(this); 
            parent = _parent; 
            children = new List<Object>(); }

        //Adds a Child Object
        public void AddChild(Object Child) { children.Add(Child); }

        public void AddChildren<T>(List<T> children)
        {
            foreach (var child in children)
            {
                children.Add(child);
            }
        }

        //Returns a child object of a specified type (usually a component like a collider)
        public T GetComponent<T>()
        {
            foreach (Object obj in children)
            {
                if (obj.GetType() == typeof(T))
                {
                    return (T)obj;
                }
            }
            return default;
        }

        public List<T> GetComponents<T>()
        {
            List<T> components = new List<T>();
            foreach (Object obj in children)
            {
                if (obj.GetType() == typeof(T))
                {
                    components.Add((T)obj);
                }
            }
            return components;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
    }   
}