using Godot;
using Pikol93.NavigationMesh;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pikol93.NavigationMeshShowcase
{
    public class Scene : Node2D
    {
        private static readonly Color[] PolygonColors = new Color[]
        {
            Godot.Colors.Yellow,
            Godot.Colors.Green,
            Godot.Colors.Aqua,
            Godot.Colors.Blue,
            Godot.Colors.Purple,
            Godot.Colors.Pink,
            Godot.Colors.Orange,
            Godot.Colors.Gold,
            Godot.Colors.DarkCyan,
            Godot.Colors.DarkOrange,
        };

#pragma warning disable 0649
        [Export]
        public Vector2[] Bounds { get; private set; }
        [Export]
        public double AgentSize { get; private set; }
        [Export]
        public bool VisiblePolygons { get; private set; }
        [Export]
        public int AmountOfAgents { get; private set; }
        [Export]
        public bool AgentDrawPath { get; private set; }
#pragma warning restore 0649

        private NavMesh navMesh;

        public override void _Ready()
        {
            Vector2[][] shapes = GetShapes();
            navMesh = NavigationMeshHelper.CreateNavMesh(Bounds, shapes, AgentSize);
            Update();

            SpawnAgents();
        }

        public override void _Draw()
        {
            if (navMesh != null)
            {
                if (VisiblePolygons)
                {
                    Random random = new Random(0);
                    foreach (Polygon polygon in navMesh.Polygons)
                    {
                        Vector2[] points = polygon.Vertices.Select(x => navMesh.Vertices[x])
                            .ToArray().ToGodotVector2Array();

                        Color[] color = new Color[] { PolygonColors[random.Next() % PolygonColors.Length] };
                        color[0].a = 0.7f;

                        DrawPolygon(points, color);
                    }
                }
            }
        }

        public List<Vector2> FindPath(Vector2 start, Vector2 end)
        {
            // This method assumes that every agent asking for path
            // is of the same size. If that's not the case then tweaking
            // this method to ask the correct NavMesh for path is not difficult.
            return navMesh.GodotFindPath(start, end, true);
        }

        private void SpawnAgents()
        {
            // Called at the startup of the scene.
            // Is responsible for adding agents to the scene.
            PackedScene agentScene = ResourceLoader.Load<PackedScene>("res://Scenes/Agent.tscn");
            Agent.drawPath = AgentDrawPath;
            Agent.Size = (float)AgentSize; 

            Random random = new Random();
            Vector2 screenSize = GetViewportRect().Size;
            for (int i = 0; i < AmountOfAgents; i++)
            {
                Vector2 randomPosition = new Vector2(
                    (float)random.NextDouble() * screenSize.x,
                    (float)random.NextDouble() * screenSize.y);

                // Put points only in the NavMesh
                Vector2 spawnPosition = navMesh.GodotFindNearestPoint(randomPosition);

                Agent agent = (Agent)agentScene.Instance();
                agent.Position = spawnPosition;
                AddChild(agent);
            }
        }

        private Vector2[][] GetShapes()
        {
            // Find all children that contain collision shapes
            List<CollisionShape2D> collisionShapes = new List<CollisionShape2D>();
            List<CollisionPolygon2D> polygonShapes = new List<CollisionPolygon2D>();
            foreach (Node child in GetChildren())
            {
                if (child is StaticBody2D body)
                {
                    foreach (Node bodyChild in body.GetChildren())
                    {
                        // Don't process not visible shapes
                        if (bodyChild is Node2D node2d && !node2d.Visible)
                            continue;

                        if (bodyChild is CollisionShape2D shape)
                            collisionShapes.Add(shape);
                        else if (bodyChild is CollisionPolygon2D polygon)
                            polygonShapes.Add(polygon);
                    }
                }
            }

            // Get vertices from each found CollisionShape2D and CollisionPolygon2D
            Vector2[][] result = new Vector2[collisionShapes.Count + polygonShapes.Count][];
            for (int i = 0; i < collisionShapes.Count; i++)
            {
                result[i] = GetVerticesFromCollisionShape(collisionShapes[i]);
            }
            for (int i = 0; i < polygonShapes.Count; i++)
            {
                result[collisionShapes.Count + i] = GetVerticesFromPolygonShape(polygonShapes[i]);
            }

            return result;
        }

        private Vector2[] GetVerticesFromCollisionShape(CollisionShape2D collisionShape)
        {
            if (collisionShape.Shape is RectangleShape2D rect)
            {
                // Return the vertices in a clockwise order
                return new Vector2[]
                {
                    collisionShape.GlobalPosition + new Vector2(-rect.Extents.x, -rect.Extents.y),
                    collisionShape.GlobalPosition + new Vector2(rect.Extents.x, -rect.Extents.y),
                    collisionShape.GlobalPosition + new Vector2(rect.Extents.x, rect.Extents.y),
                    collisionShape.GlobalPosition + new Vector2(-rect.Extents.x, rect.Extents.y),
                };
            }
            else if (collisionShape.Shape is ConvexPolygonShape2D convex)
            {
                // convex.Points already contains the list of vertices
                // but they need to be offset to correctly reflect changes in the editor
                Vector2[] result = convex.Points;
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] += collisionShape.GlobalPosition;
                }

                return result;
            }

            throw new NotImplementedException("Tried to get vertices from a not implemented shape type.");
        }

        private Vector2[] GetVerticesFromPolygonShape(CollisionPolygon2D collisionPolygon)
        {
            // Keep in mind that the vertices need to be put in a clockwise order!
            Vector2[] polygon = collisionPolygon.Polygon;
            for (int i = 0; i < polygon.Length; i++)
            {
                polygon[i] += collisionPolygon.GlobalPosition;
            }

            return polygon;
        }
    }
}
