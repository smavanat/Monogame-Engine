using AI_test.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Sprites;

namespace AI_test.Core
{
    //https://badecho.com/index.php/2023/01/14/fast-simple-quadtree/
    //https://gist.github.com/MechanicalFerret/a871b49a607292d32d7a

    public class Quadtree
    {
        private const int MAX_OBJECTS = 32;
        private const int MAX_LEVELS = 5;

        private int level;
        private List<Sprite> collidableObjects;
        private Rectangle bounds;
        private Quadtree[] nodes;

        public int Count { get { return collidableObjects.Count; } }

        public Quadtree(int _level, Rectangle _bounds)
        {
            level = _level;
            collidableObjects = new List<Sprite>();
            bounds = _bounds;
            nodes = new Quadtree[4];
        }

        public void Clear()
        {
            collidableObjects.Clear();
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        private void Split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;
            nodes[0] = new Quadtree(level+1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level+1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level+1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level+1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(Rectangle _Rect)
        {
            int index = -1;
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            bool topQuadrant = (_Rect.Y < horizontalMidpoint &&  _Rect.Y + _Rect.Height < verticalMidpoint);
            bool bottomQuadrant = (_Rect.Y > horizontalMidpoint);

            if(_Rect.X < verticalMidpoint && _Rect.X + _Rect.Width < verticalMidpoint)
            {
                if(topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            else if (_Rect.X > verticalMidpoint)
            {
                if (topQuadrant)
                    index = 0;
                if(bottomQuadrant)
                    index = 3;
            }
            return index;
        }

        public void Insert(Sprite _body)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(_body.Rectangle);
                if (index != -1)
                {
                    nodes[index].Insert(_body);
                    return;
                }
            }

            collidableObjects.Add(_body);
            if(collidableObjects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }
                List<Sprite> save = new List<Sprite>();
                foreach(Sprite sprite in collidableObjects.ToList())
                { 
                    int index = GetIndex(sprite.Rectangle);
                    if(index != -1)
                        nodes[index].Insert(sprite);
                    else 
                        save.Add(sprite);
                }
                collidableObjects = save;
            }
        }
        private void Remove(Sprite _body)
        {
            if(collidableObjects == null && collidableObjects.Contains(_body))
            {
                collidableObjects.Remove(_body);
            }
        }

        public void Delete(Sprite _body)
        {
            bool objectRemoved = false;
            if(collidableObjects != null && collidableObjects.Contains(_body))
            {
                Remove(_body);
                objectRemoved = true;
            }

            if (nodes[0] != null && !objectRemoved)
            {
                nodes[0].Delete(_body);
                nodes[1].Delete(_body);
                nodes[2].Delete(_body);
                nodes[3].Delete(_body);
            }
            if (nodes[0] != null)
            {
                if (nodes[0].Count == 0 && nodes[1].Count == 0 && nodes[2].Count == 0 && nodes[3].Count == 0)
                {
                    nodes[0] = null;
                    nodes[1] = null;
                    nodes[2] = null;
                    nodes[3] = null;
                }
            }
        }

        public List<Sprite>Retrieve(Rectangle _rect)
        {
            int index = GetIndex(_rect);
            List<Sprite> returnedObjects = new List<Sprite>(collidableObjects);

            //If we have subnodes
            if (nodes[0] != null)
            {
                //If _rect fits into a sub node
                if(index != -1)
                {
                    returnedObjects.AddRange(nodes[index].Retrieve(_rect));
                }
                //If _rect does not fit into a subnode, check it against all subnodes
                else
                {
                    for(int i = 0; i < nodes.Length; i++)
                    {
                        returnedObjects.AddRange(nodes[i].Retrieve(_rect));
                    }
                }
            }
            return returnedObjects; 
        }
    }
}
