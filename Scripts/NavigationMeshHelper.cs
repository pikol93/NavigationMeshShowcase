using System.Collections.Generic;
using Godot;
using Pikol93.NavigationMesh;
using SN = System.Numerics;

namespace Pikol93.NavigationMeshShowcase
{
    // The NavigationMesh library uses System.Numerics.Vector2 internally while Godot uses
    // Godot.Vector2. This class's purpose is to allow for using the library without
    // manually converting the data.
    public static class NavigationMeshHelper
    {
        public static NavMesh CreateNavMesh(Vector2[] bounds, Vector2[][] shapes, double agentSize)
        {
            SN.Vector2[][] shapesSN = new SN.Vector2[shapes.Length][];
            for (int i = 0; i < shapes.Length; i++)
            {
                shapesSN[i] = shapes[i].ToSNVector2Array();
            }

            return NavMeshFactory.Create(bounds.ToSNVector2Array(), shapesSN, agentSize);
        }
        
        public static Vector2 GodotFindNearestPoint(this NavMesh navMesh, Vector2 point)
        {
            return navMesh.FindNearestPoint(point.ToSNVector2()).ToGodotVector2();
        }

        public static List<Vector2> GodotFindPath(this NavMesh navMesh, Vector2 start, Vector2 end, bool limitToNavMesh = false)
        {
            return navMesh.FindPath(start.ToSNVector2(), end.ToSNVector2(), limitToNavMesh).ToGodotVector2List();
        }

        public static Vector2 ToGodotVector2(this SN.Vector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static SN.Vector2 ToSNVector2(this Vector2 vector)
        {
            return new SN.Vector2(vector.x, vector.y);
        }

        public static List<Vector2> ToGodotVector2List(this List<SN.Vector2> list)
        {
            List<Vector2> result = new List<Vector2>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i].ToGodotVector2());
            }

            return result;
        }

        public static Vector2[] ToGodotVector2Array(this SN.Vector2[] array)
        {
            Vector2[] result = new Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = new Vector2(array[i].X, array[i].Y);
            }

            return result;
        }

        public static SN.Vector2[] ToSNVector2Array(this Vector2[] array)
        {
            SN.Vector2[] result = new SN.Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = new SN.Vector2(array[i].x, array[i].y);
            }

            return result;
        }
    }
}