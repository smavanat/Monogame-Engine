//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;

//namespace AI_test.Core
//{
//    public class PathRequestManager
//    {
//        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
//        PathRequest currentPathRequest;

//        static PathRequestManager instance;
//        Pathfinding pathfinding;

//        bool isProcessingPath;

//        public PathRequestManager(Pathfinding _pathfinding)
//        {
//            instance = this;
//            pathfinding = _pathfinding;
//        }

//        public static void RequestPath(Node pathStart, Node pathEnd, Action<Node[], bool> callback)
//        {
//            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
//            instance.pathRequestQueue.Enqueue(newRequest);
//            instance.TryProcessNext();
//        }

//        void TryProcessNext()
//        {
//            if(!isProcessingPath && pathRequestQueue.Count > 0)
//            {
//                currentPathRequest = pathRequestQueue.Dequeue();
//                isProcessingPath = true;
//                pathfinding.FindAstarPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
//            }
//        }

//        public void FinishedProcessingPath(Node[] path, bool success)
//        {
//            currentPathRequest.callback(path, success);
//            isProcessingPath = false;
//            TryProcessNext();
//        }

//        struct PathRequest 
//        {
//            public Node pathStart;
//            public Node pathEnd;
//            public Action<Node[], bool> callback;

//            public PathRequest(Node _start, Node _end, Action<Node[], bool> _callback)
//            {
//                pathStart = _start;
//                pathEnd = _end;
//                callback = _callback;
//            }
//        }
//    }
//}

//Old FollowPath code:

//IEnumerator<float> FollowPath()
//{
//    Node currentWaypoint = path[0];
//    while (true)
//    {
//        if (Position == currentWaypoint.Position)
//        {
//            targetIndex++;
//            if (targetIndex >= path.Length)
//            {
//                yield break;
//            }
//            currentWaypoint = path[targetIndex];
//        }

//        Vector2 dir = currentWaypoint.Position - Position;
//        dir.Normalize();

//        Position += dir * speed;
//        yield return 1 / 60f;

//    }
//}

//public void OnPathFound(Node[] newPath, bool pathSuccessful)
//{
//    if (pathSuccessful)
//    {
//        path = newPath;
//        targetIndex = 0;
//        //FollowPath();
//        //StopCoroutine("FollowPath");
//        //StartCoroutine("FollowPath");
//    }
//}