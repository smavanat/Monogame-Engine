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
    //A simple inventory I made for holding objects in order to add behaviour tree functionality
    public class Inventory: Component
    {
        public Component[] items = new Component[10];//This is where all the items are stored
        public int numFirewood = 0;//For GOAP test

        public Inventory() { }

        public override void Update(GameTime gameTime) {}
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

        public void Add(Component item, int index) {
            items[index] = item;
        }
    }
}
