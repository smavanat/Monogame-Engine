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
using System.Xml.Linq;

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
        public Node(int x, int y, int id, Texture2D _image, Vector2 position, float _rotation) : base(_image, position, _rotation, false)
        {
            xPos = x;
            yPos = y;
            bitValue = id;
            if(bitValue == 1)
            {
                isWalkable = false;
                collider = new Collider(new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height), this);
            }
            else
                isWalkable = true;
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
            //else if(bitValue == 3)
            //    spriteBatch.Draw(texture, Position, null, Color.Orange, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            //else if (bitValue == 4)
            //    spriteBatch.Draw(texture, Position, null, Color.Green, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
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
        const string FILENAME = @"C:\Users\EdwinH\Documents\My Work\Data Structures and Algorithms\AI test\AI test\Level Generation\Levels\Test Level.xml";
        int gridSizeX;
        int gridSizeY;
        public Vector2 gridWorldSize;
        public Node[,] grid;
        public int nodeRadius;
        public int nodeDiameter;
        public Vector2 position;
        public Grid(Vector2 _gridWorldSize, int _nodeSize, Vector2 _position, Texture2D _sprite)
        {
            gridWorldSize = _gridWorldSize;
            nodeRadius = _nodeSize;
            position = _position;
            gridSizeX = (int)Math.Round(_gridWorldSize.X / nodeRadius);
            gridSizeY = (int)Math.Round(_gridWorldSize.Y / nodeRadius);
            grid = new Node[gridSizeX, gridSizeY];
            nodeDiameter = nodeRadius * 2;
            Vector2 worldBottomLeft = position - new Vector2(gridWorldSize.X / 2, 0) - new Vector2(0, gridWorldSize.Y / 2);

            grid = LoadToGrid(_sprite, worldBottomLeft);
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

        //Given Vector2 coordinates, gets the closest grid node for pathfinding;
        public Node NodeFromWorldPoint(Vector2 worldPosition)
        {
            //Divide by node diameter to get node. Subtract position to recenter to (0,0).
            //Adding the gridWorldSize is needed for mathematical shenanigans
            float percentX = (worldPosition.X - position.X + gridWorldSize.X/2 ) / nodeDiameter;
            float percentY = (worldPosition.Y - position.Y + gridWorldSize.Y/2 ) / nodeDiameter;

            //Ensures values stay within grid range and round down for more accuracy
            int x = (int)MathF.Floor(Math.Clamp(percentX, 0, gridSizeX - 1));
            int y = (int)MathF.Floor(Math.Clamp(percentY, 0, gridSizeY - 1));
            
            return grid[x, y];
        }

        public Node[,] LoadToGrid(Texture2D image, Vector2 _worldBottomLeft)
        {
            XDocument doc = XDocument.Load(FILENAME);
            XElement root = doc.Root;
            int sizeX = Int32.Parse(root.Attribute("sizeX").Value.ToString());
            int sizeY = Int32.Parse(root.Attribute("sizeY").Value.ToString());
            Node[,] tempGrid = new Node[sizeX, sizeY];
            List<string> gridTypes = new List<string>();
            foreach (XElement xElement in root.Elements())
            {
                gridTypes.Add(xElement.Value.ToString());
            }
            /*foreach (string s in gridTypes)
            {
                Debug.WriteLine(s);
            }*/
            for (int x = 0; x < sizeX; x++)
            {
                var s = gridTypes[x].Split(" ");
                for (int y = 0; y < sizeY; y++)
                {
                    Vector2 worldpoint = _worldBottomLeft + new Vector2(1, 0) * (x * nodeDiameter + nodeRadius) + new Vector2(0, 1) * (y * nodeDiameter + nodeRadius);
                    tempGrid[x, y] = new Node(x, y, Int32.Parse(s[y]), image, worldpoint, 0);
                    //Debug.WriteLine($"{worldpoint.X}, {worldpoint.Y}");
                }
            }
            return tempGrid;
        }

        float Clamp01(float value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        public void SpawnObjects()
        {
            foreach (Node n in grid)
            {
                if (n.bitValue == 3)
                {
                    Door d = new Door(ArtManager.textures["Door"], n.Position);
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
        public override void Update(GameTime gameTime){}
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Node node in grid)
            {
                node.Draw(gameTime, spriteBatch);
            }
        }
    }
}