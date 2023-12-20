using AI_test.AI_and_Behaviours;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AI_test.Core
{
    //Basic class that is used by all objects that update,
    public abstract class Component
    {
        public Component parent;
        public List<Component> children;
        public Component(Component _parent = null) 
        {EntityManager.AddGameObject(this); parent = _parent; children = new List<Component>();}

        //Adds a Child Object
        public void AddChild(Component Child) { Child.parent = this;  this.children.Add(Child);}

        public void AddChildren<T>(List<Component> children)
        {
            foreach(var child in children)
            {
                child.parent = this;
                children.Add(child);
            }
        } 

        //Returns a child object of a specified type (usually a component like a collider)
        public T GetComponent<T>() where T : Component
        {
            foreach (Object obj in children)
            {
                if (typeof(T).IsAssignableFrom(obj.GetType()))
                {
                    return (T)obj;
                }
            }
            //return children.OfType<T>().FirstOrDefault();
            return default;
        }

        public List<T> GetComponents<T>()
        {
            List<T> components = new List<T>();
            foreach (Object obj in children)
            {
                if (typeof(T).IsAssignableFrom(obj.GetType()))
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