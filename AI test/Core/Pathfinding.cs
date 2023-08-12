using AI_test.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_test.Core
{
    //Using Sebastian Lague's implentation of 2D A* with some edits
    //Pretty good explanation of the algorithm https://www.youtube.com/watch?v=-L-WgKMFuhE&t=1s&ab_channel=SebastianLague
    public class Pathfinding
    {
        Grid map;
        public Node[] returnedPath;
        public Pathfinding(Grid _map)
        {
            map = _map;
        }
        public void FindAstarPath(Node startNode, Node targetNode)
        {
            Node[] waypoints = new Node[0];
            bool pathSuccess = false;

            if (startNode.isWalkable && targetNode.isWalkable)
            {
                Heap<Node> openSet = new Heap<Node>(map.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in map.GetNeighbours(currentNode))
                    {
                        if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                        }
                    }
                }
                //foreach (Node n in closedSet)
                //{
                //    n.bitValue = 3;
                //}
            }
            if (pathSuccess)
                returnedPath = RetracePath(startNode, targetNode);
            //foreach (Node n in returnedPath)
            //{
            //    n.bitValue = 4;
            //}
        }

        Node[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Add(startNode);
            Node[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Node[] SimplifyPath(List<Node> path)
        {
            List<Node> waypoints = new List<Node>();
            Vector2 directionOld = Vector2.Zero;

            waypoints.Add(path[0]);

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].xPos - path[i].xPos, path[i - 1].yPos - path[i].yPos);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i-1]);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = (int)MathF.Abs(nodeA.xPos - nodeB.xPos);
            int dstY = (int)MathF.Abs(nodeA.yPos - nodeB.yPos);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }


    }
    public class Heap<T> where T : IHeapItem<T>
    {

        T[] items;
        int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }
        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }
        public void UpdateItem(T item)
        {
            SortUp(item);
        }
        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }
        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public static class SteeringBehaviours
    {
        public static Vector2 Seek(Vector2 position, Vector2 target, GameTime gameTime, float maxSpeed, int maxForce = 3)
        {
            Vector2 velocity = target - position;
            velocity.Normalize();
            velocity *= maxSpeed;
            Vector2 desiredVelocity = target - position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            Vector2 steering = desiredVelocity - velocity;
            steering = Truncate(steering, maxForce);

            velocity = Truncate(velocity + steering, (int)maxSpeed);
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return position;
        }
        public static Vector2 Truncate(Vector2 vector, int max)
        {
            float i = max / vector.Length();
            i = i < 1.0f ? i : 1.0f;
            vector *= i;
            return vector;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }

    }
}
