using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AI_test.Core.Pathfinding;
using System.Diagnostics;
using AI_test.Sprites;
using System.Runtime.CompilerServices;

namespace AI_test.Core
{
    public class Node : Sprite, IHeapItem<Node>
    {
        public int xPos, yPos;
        public int gCost, hCost;
        public bool isWalkable;
        public Node parent;
        public int bitValue;
        public Collider collider;
        int heapIndex;
        public Node(int x, int y, int id, Texture2D _image, bool walkable, Vector2 position, float _rotation) : base(_image, position, _rotation, false)
        {
            xPos = x;
            yPos = y;
            isWalkable = walkable;
            //IsCollideable = !walkable;
            bitValue = id;
            if (isWalkable)
                bitValue = 0;
            else
            {
                bitValue = 1;
                collider = new Collider(new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height), this);
                //IsCollideable = true;
            }
        }
        //For aStar pathfinding
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (bitValue == 1)
            {
                spriteBatch.Draw(texture, Position, null, Color.Black, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            }
            base.Draw(gameTime, spriteBatch);
        }
    }
    //A grid of nodes - 2D array. Also stores its world position.
    public class Grid : Component
    {
        int gridSizeX;
        int gridSizeY;
        public int gridWorldSize;
        public Node[,] grid;
        public int nodeRadius;
        int nodeDiameter;
        public Vector2 position;
        public Grid(int _gridWorldSizeX, int _gridWorldSizeY, int _nodeSize, Vector2 _position, Texture2D _sprite)
        {
            gridWorldSize = _gridWorldSizeX * _gridWorldSizeY;
            nodeRadius = _nodeSize;
            position = _position;
            gridSizeX = _gridWorldSizeX / nodeRadius;
            gridSizeY = _gridWorldSizeY / nodeRadius;
            grid = new Node[gridSizeX, gridSizeY];
            nodeDiameter = nodeRadius * 2;
            Vector2 worldBottomLeft = position - new Vector2(_gridWorldSizeX / 2, 0) - new Vector2(0, _gridWorldSizeY / 2);


            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector2 worldpoint = worldBottomLeft + new Vector2(1, 0) * (x * nodeDiameter + nodeRadius) + new Vector2(0, 1) * (y * nodeDiameter + nodeRadius);
                    if(y == x)
                        grid[x, y] = new Node(x, y, 1, _sprite, false, worldpoint, 0f);
                    else
                        grid[x, y] = new Node(x, y, 1, _sprite, true, worldpoint, 0f);
                }
            }
            //grid = WriteToXML.GetGridFromXML(grid);
            //SpawnObjects();
        }
        //Given a node, get the 8 neighbours surrounding it.
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.xPos + x;
                    int checkY = node.yPos + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
        //Gets the north, south, east and west neighbours of a node in a grid.
        public List<Node> GetCardinalNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            int[] dRow = { 0, 1, 0, -1 };
            int[] dCol = { -1, 0, 1, 0 };
            for (int i = 0; i < 4; i++)
            {
                int adjx = node.xPos + dRow[i];
                int adjy = node.yPos + dCol[i];
                if (adjx >= 0 && adjx < gridSizeX && adjy >= 0 && adjy < gridSizeY)
                    neighbours.Add(grid[adjx, adjy]);
            }
            return neighbours;
        }

        public void SpawnObjects()
        {
            foreach (Node n in grid)
            {
                if (n.bitValue == 3)
                {
                    Door d = new Door(ObjectSpawner.textures["Door"], n.Position);
                }
            }
        }
        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }
        public override void Update(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Node node in grid)
            {
                node.Draw(gameTime, spriteBatch);
            }
        }
    }
}