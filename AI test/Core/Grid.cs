using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using static AI_test.Core.Pathfinding;
using AI_test.Sprites;
using System.Xml.Linq;
using System.Threading.Tasks;
using AI_test.ToolsIIStorageIIPrefabs;
using System.Diagnostics;

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
        public Node(int x, int y, int id, Texture2D _image, Vector2 position, float _rotation) : base(_image, position, _rotation)
        {
            xPos = x;
            yPos = y;
            bitValue = id;//What type of node it is. The appropriate sprite corresponding to this value can then be drawn.
            if(bitValue == 1)
            {
                isWalkable = false;
                //Nodes only get a collider if they are an obstacle
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
                //Walls
                spriteBatch.Draw(texture, Position, null, Color.Black, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            else if (bitValue == 3)
                //Unplanted soil
                spriteBatch.Draw(texture, Position, null, Color.Orange, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            else if (bitValue == 4)
                //Planted soil
                spriteBatch.Draw(texture, Position, null, Color.Green, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            else if (bitValue == 5)
                spriteBatch.Draw(texture, Position, null, Color.Green, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            else
                //Just regular floor.
                spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            base.Draw(gameTime, spriteBatch);
        }
    }
    //A grid of nodes - 2D array. Also stores its world position.
    public class Grid : Component
    {
        //This is the file where the current demo map is held.
        const string FILENAME = @"C:\Users\EdwinH\Documents\My Work\Data Structures and Algorithms\AI test\AI test\Level Generation\Levels\Test Level.xml";
        //Dimensions of the grid
        int gridSizeX;
        int gridSizeY;
        public Vector2 gridWorldSize;
        //Actual grid
        public Node[,] grid;
        //Dimensions of the nodes.
        public int nodeRadius;
        public int nodeDiameter;
        //World position of the grid. Has to be included for drawing purposes.
        public Vector2 position;
        //Constructor.
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
            SpawnObjects();
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

        //Loads the bitvalues of the grid from an xml file.
        public Node[,] LoadToGrid(Texture2D image, Vector2 _worldBottomLeft)
        {
            XDocument doc = XDocument.Load(FILENAME);
            XElement root = doc.Root;
            //Gets dimensions of the grid.
            int sizeX = Int32.Parse(root.Attribute("sizeX").Value.ToString());
            int sizeY = Int32.Parse(root.Attribute("sizeY").Value.ToString());
            Node[,] tempGrid = new Node[sizeX, sizeY];
            List<string> gridTypes = new List<string>();
            foreach (XElement xElement in root.Elements())
            {
                gridTypes.Add(xElement.Value.ToString());
            }
            //Do y then x first otherwise there will be a transpose of the initial grid
            for (int y = 0; y < sizeX; y++)
            {
                var s = gridTypes[y].Split(" ");
                for (int x = 0; x < sizeY; x++)
                {
                    Vector2 worldpoint = _worldBottomLeft + new Vector2(1, 0) * (x * nodeDiameter + nodeRadius) + new Vector2(0, 1) * (y * nodeDiameter + nodeRadius);
                    tempGrid[x, y] = new Node(x, y, Int32.Parse(s[x]), image, worldpoint, 0);
                }
            }
            return tempGrid;
        }

        //Clamps a value between 0 and 1.
        float Clamp01(float value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        //Spawns objects (such as tables, doors, etc.) on top of nodes 
        public void SpawnObjects()
        {
            foreach (Node n in grid)
            {
                if (n.bitValue == 2)
                {
                    SupplyPile sp = new SupplyPile(ArtManager.textures["SupplyPile"], n.Position, n.Rotation);
                }
                if (n.bitValue == 6)
                {
                    ChoppingBlock cb = new ChoppingBlock(ArtManager.textures["ChoppingBlock"], n.Position, n.Rotation);
                }
            }
        }

        //My attempts at placing in time delays. I don't think they were successful 
        public async void ChangeNodeAfterTime(int nodeX, int nodeY, int newBitVal)
        {
            await Task.Delay(10000);
            grid[nodeY, nodeX].bitValue = newBitVal;
        }
        public void ChangeNodeAfterTime(Vector2 worldPoint, int newBitVal)
        {
            //Node n = NodeFromWorldPoint(worldPoint);
            //await Task.Delay(10000);
            //Thread.Sleep(10000);
            //grid[n.yPos, n.xPos].bitValue = newBitVal;
            var t = new Thread(() => ChangeNode(worldPoint, newBitVal));//I don't think this will work
            t.Start();
            t.Join();
        }

        void ChangeNode(Vector2 worldPoint, int newBitVal)
        {
            Node n = NodeFromWorldPoint(worldPoint);
            Thread.Sleep(10000);//Dumb idea
            grid[n.yPos, n.xPos].bitValue = newBitVal;
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