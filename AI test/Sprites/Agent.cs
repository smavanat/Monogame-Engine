using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_test.Core;
using System.Collections;
using System.Diagnostics;

namespace AI_test.Sprites
{
    public class Agent : Sprite
    {
        public Vector2 targetPosition;

        public Node[] path;
        private float speed = 1f;
        int targetIndex;
        Pathfinding pathfinding;
        Node currentWaypoint;
        bool hasReachedTarget = false;
        GameTime _gameTime;
        Grid map;

        public Agent(Texture2D _texture, Vector2 _position, float _rotation, Node targetNode, Grid _map) : base(_texture, _position, _rotation, false)
        {
            map = _map;
            pathfinding = new Pathfinding(map);
            //targetPosition = targetNode.Position;
            //pathfinding.FindAstarPath(map.NodeFromWorldPoint(Position), targetNode);
            //path = pathfinding.returnedPath;
            //currentWaypoint = path[0];
            //PathRequestManager.RequestPath(map.NodeFromWorldPoint(Position), targetNode, OnPathFound);
        }

        public override void Update(GameTime gameTime) 
        {
            _gameTime = gameTime;
            Move();
        }

        public void GetNewTarget(Node targetNode)
        {
            targetPosition = targetNode.Position;
            pathfinding.FindAstarPath(map.NodeFromWorldPoint(Position), targetNode);
            path = pathfinding.returnedPath;
            currentWaypoint = path[0];
        }

        public void Move()
        {
            if ((Vector2.Distance(Position, currentWaypoint.Position) < 1f))
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    hasReachedTarget = true;
                    speed = 0;
                }
                else
                    currentWaypoint = path[targetIndex];
            }

            if (!(Vector2.Distance(Position, targetPosition) < 1f))
            {
                Position = SteeringBehaviours.Seek(Position, currentWaypoint.Position, _gameTime, 50);
            }
        }

        public override void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 0f);
            base.Draw(gametime, spriteBatch);
        }
    }
}