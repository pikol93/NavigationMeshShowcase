using Godot;
using System;
using System.Collections.Generic;

namespace Pikol93.NavigationMeshShowcase
{
    public class Agent : KinematicBody2D
    {
        private const float Speed = 300f;
        private static Random random = new Random();
        private static Color pathColor = new Color(1f, 0f, 0f, 0.3f);
        public static bool drawPath = false;
        public static float Size = 16f;

        private List<Vector2> path = new List<Vector2>();

        public override void _Ready()
        {
            SetSize(Size);
        }

        public override void _PhysicsProcess(float delta)
        {
            // This is added for displaying the path that the agent is following
            Update();

            if (path.Count > 0)
            {
                Vector2 difference = path[0] - GlobalPosition;
                float distanceLeft = difference.Length();

                if (distanceLeft < Speed * delta)
                {
                    // No need to MoveAndSlide(), the point is assumed to be on the NavMesh
                    GlobalPosition = path[0];
                    path.RemoveAt(0);
                    return;
                }

                // Normalize the difference vector
                Vector2 direction = difference / distanceLeft;
                MoveAndSlide(direction * Speed);
            }
            else
            {
                OnPathCompleted();
            }
        }

        public override void _Draw()
        {
            if (drawPath)
            {
                if (path.Count <= 0)
                    return;

                // Draws the line to the current path target
                DrawLine(Vector2.Zero, path[0] - Position, pathColor);

                // Draws the rest of the line
                for (int i = 0; i < path.Count - 1; i++)
                {

                    DrawLine(path[i] - Position, path[i + 1] - Position, pathColor);
                }
            }
        }

        private void SetSize(float size)
        {
            var collisionNode = GetNode<CollisionShape2D>("CollisionShape2D");
            var shape = collisionNode.Shape as CircleShape2D;
            shape.Radius = (float)size;
        }

        private void OnPathCompleted()
        {
            // Note that this is an example project focusing on the aspect of the
            // pathfinding showcase. The way a reference to the NavMesh object is 
            // obtained is left up to the user to decide.
            Scene parent = GetParent() as Scene;
            if (parent == null)
            {
                GD.PushError("Could not find parent of type Scene.");
                return;
            }

            // Find a random point in the scene to travel to
            Vector2 sceneLimit = GetViewportRect().Size;
            Vector2 targetPoint = new Vector2(
                (float)random.NextDouble() * sceneLimit.x,
                (float)random.NextDouble() * sceneLimit.y);

            path = parent.FindPath(GlobalPosition, targetPoint);
        }
    }
}