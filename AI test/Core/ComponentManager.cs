using AI_test.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_test.Core
{
    public static class ComponentManager
    {
        public static List<Component> components = new List<Component>();
        public static List<Collider> colliders = new List<Collider>();
        public static List<Sprite> sprites = new List<Sprite>();

        public static void AddComponent(Component component)
        {
            components.Add(component);
        }

        public static void AddSprite(Sprite sprite) //Adds object to Sprite list
        {
            sprites.Add(sprite);
        }

        public static void AddCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        public static void RemoveComponent(Component component)//Removes object from GameObject list
        {
            components.Remove(component);
            component = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void RemoveSprite(Sprite sprite)
        {
            sprites.Remove(sprite);
            sprite = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void RemoveColiider(Collider collider)
        {
            colliders.Remove(collider);
            collider = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


    }
}