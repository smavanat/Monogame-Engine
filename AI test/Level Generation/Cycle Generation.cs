using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AI_test.Level_Generation
{
    public class CycleGenerator
    {
        Random random = new Random();
        int numIntermediateNodes;
        AdjacencyList cycleGraph;
        string[] RoomTypes = {"Economic", "Social", "Housing", "Government" };
        public CycleGenerator(int _graphSize) 
        {
            cycleGraph = new AdjacencyList(_graphSize);
            Dictionary<int, string> nodes = new Dictionary<int, string>();
            for(int i = 0; i < _graphSize; i++)
            {
                var roomType = RoomTypes[random.Next(0, RoomTypes.Length)];
                nodes.Add(i, roomType);
            }
            nodes = EnsureCoreNodesPresent(nodes);
            foreach(KeyValuePair<int, string> pair in nodes)
            {
                Debug.WriteLine($"{pair.Key}, {pair.Value}");
                if(pair.Key < _graphSize - 1)
                    cycleGraph.AddEdgeAtEnd(pair.Key, pair.Key + 1, pair.Value);
                else
                    cycleGraph.AddEdgeAtEnd(pair.Key, 0, pair.Value);
            }
            AddBranches(_graphSize);
            cycleGraph.LogAdjacencyList();
        }
        public Dictionary<int, string> EnsureCoreNodesPresent(Dictionary<int, string> nodes)
        {
            cycleGraph.LogAdjacencyList();
            int i = 0;
            do
            {
                if (!nodes.ContainsValue("Government"))
                {
                    var Duplicate = nodes.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                    foreach (var item in Duplicate)
                    {
                        var keys = item.Aggregate("", (s, v) => s + ", " + v);
                        var message = "The following keys have the value " + item.Key + ":" + keys;
                        Debug.WriteLine(message);
                    }
                    var test = Duplicate.First().ElementAt(0);
                    nodes[test.Key] = "Government";
                    Debug.WriteLine(test.Value);
                }
                else if (!nodes.ContainsValue("Social"))
                {
                    var Duplicate = nodes.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                    foreach (var item in Duplicate)
                    {
                        var keys = item.Aggregate("", (s, v) => s + ", " + v);
                        var message = "The following keys have the value " + item.Key + ":" + keys;
                        Debug.WriteLine(message);
                    }
                    var test = Duplicate.First().ElementAt(0);
                    nodes[test.Key] = "Social";
                    Debug.WriteLine(test.Value);
                }
                else if (!nodes.ContainsValue("Economic"))
                {
                    var Duplicate = nodes.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                    foreach (var item in Duplicate)
                    {
                        var keys = item.Aggregate("", (s, v) => s + ", " + v);
                        var message = "The following keys have the value " + item.Key + ":" + keys;
                        Debug.WriteLine(message);
                    }
                    var test = Duplicate.First().ElementAt(0);
                    nodes[test.Key] = "Economic";
                    Debug.WriteLine(test.Value);
                }

                foreach (KeyValuePair<int, string> node in nodes)
                    Debug.WriteLine(node.Key + " " + node.Value);
                i++;
            } while (i < 3);
            return nodes;
        }
        public void AddBranches(int _graphSize)
        {
            int numBranches = random.Next(0, 4);
            Debug.WriteLine(numBranches);
            if (numBranches > 0)
            {
                for (int i = 0; i < numBranches; i++)
                {
                    var roomType = RoomTypes[random.Next(0, RoomTypes.Length)];
                    var roomNumber = random.Next(_graphSize);
                    cycleGraph.AddEdgeAtEnd(roomNumber, _graphSize + i, roomType);
                }
            }
        }
        public void UseGrammars(Dictionary<int, string> list, AdjacencyList adjacencyList)
        {
            bool govDuplicateExists = false;
            var q = list.GroupBy(x => x.Value).Where(x => x.Count() > 1 );
            foreach( var x in q)
            {
                foreach( var y in x)
                {
                    if (y.Value == "Government")
                    {
                        govDuplicateExists = true;
                    }
                }
            }
            foreach (KeyValuePair<int, string> node in list)
            {
                if(node.Value == "Government")
                {
                    if(!govDuplicateExists)
                        list[node.Key] = "Central Administration and Space Station";
                    else 
                    {
                        
                    }
                }
                else
                {

                }
            }
        }
        public string Grammars(LinkedList<Tuple<int, string>> vertices, string nodeType)
        {
            List<string> nodes = new List<string>();
            List<string> possibleNodes = new List<string>();
            bool Social = false, Government = false, Economic = false, Housing = false;
            foreach(Tuple<int, string> T in vertices)
            {
                nodes.Add(T.Item2);
                switch (T.Item2)
                {
                    case "Government":
                        Government = true;
                    break;
                    case "Social":
                        Social = true;
                        break;
                    case "Economic":
                        Economic = true;
                        break;
                    case "Housing":
                        Housing = true;
                        break;
                }
            }
            if (nodeType == "Government")
            {
                if (Economic)
                    possibleNodes.Add("Admin Centre");
                if (Economic && Housing)
                    possibleNodes.Add("Police Station");
                if (Economic && Social)
                    possibleNodes.Add("Courthouse");
                if (Housing && Social)
                    possibleNodes.Add("Police Station");
            }
            else if (nodeType == "Economic")
            {
                if (Government)
                    possibleNodes.Add("Space Port");
                if (Housing || Social)
                    possibleNodes.Add("Shopping Area");
                if (Government && Housing)
                    possibleNodes.Add("Military Depot");
                if (Government && Social)
                    possibleNodes.Add("Offices");
            }
            else if (nodeType == "Housing")
            {
                if (Government)
                {
                    possibleNodes.Add("High-End Apartments");
                    possibleNodes.Add("Militia Barracks");
                }
                if (Social)
                    possibleNodes.Add("Hotel");
                if (Housing)
                    possibleNodes.Add("Worker Quaters");
                if (Economic)
                    possibleNodes.Add("Worker Barracks"); 
            }
            else if (nodeType == "Social")
            {
                if (Housing)
                    possibleNodes.Add("Tavern and Bar or Cinema");
                if (Government)
                    possibleNodes.Add("State-Owned Recreation");
                if (Economic)
                    possibleNodes.Add("Resort");
            }   
            return possibleNodes[random.Next(0, possibleNodes.Count)];
        }
    }
    public class AdjacencyList
    {
        public LinkedList<Tuple<int,string>>[] adjacencyList;
        public LinkedList<Tuple<int, string>> nodes = new LinkedList<Tuple<int, string>>();

        public AdjacencyList(int vertices)
        {
            adjacencyList = new LinkedList<Tuple<int, string>>[vertices];

            for(int i = 0; i < adjacencyList.Length; i++)
            {
                adjacencyList[i] = new LinkedList<Tuple<int, string>>();
            }
            
        }

        //Appends a new Edge to the linked list
        public void AddEdgeAtEnd(int startVertex, int endVertex, string type)
        {
            adjacencyList[startVertex].AddLast(new Tuple<int, string>(endVertex, type));
        }
        //Adds new edge at front of linked list
        public void AddEdgeAtStart(int startVertex, int endVertex, string type)
        {
            adjacencyList[startVertex].AddFirst(new Tuple<int, string>(endVertex, type));
        }
        //Returns number of vertives. Does not change for an object
        public int GetNumberOfVerticies()
        {
            return adjacencyList.Length;
        }
        //Returns a copy of the linked list of outward edges from a vertex
        public LinkedList<Tuple<int, string>> this[int index]
        {
            get
            {
                LinkedList<Tuple<int, string>> edgeList = new LinkedList<Tuple<int, string>>(adjacencyList[index]);
                return edgeList;
            }
        }
        //Prints the adjacency list
        public void LogAdjacencyList()
        {
            int i = 0;
            foreach(LinkedList<Tuple<int, string>> list in adjacencyList)
            {
                Debug.Write($"adjacencyList[{i}] -> ");
                foreach(Tuple<int, string> edge in list)
                {
                    Debug.Write($"{edge.Item1}: {edge.Item2}, ");
                }
                i++;
                Debug.WriteLine("");
            }
        }
        // Removes the first occurence of an edge and returns true
        // if there was any change in the collection, else false
        public bool removeEdge(int startVertex, int endVertex, string type)
        {
            Tuple<int, string> edge = new Tuple<int, string>(endVertex, type);

            return adjacencyList[startVertex].Remove(edge);
        }
    }
}
