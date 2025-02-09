using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace NearestCoordinate
{

    public class KD3
    {
        public class KdNode
        {
            public float2 Point;
            public KdNode Left;
            public KdNode Right;
        }

        private KdNode root;

        public void Build(float2[] points)
        {
            root = BuildRecursive(points, 0, points.Length - 1, 0);
        }

        private KdNode BuildRecursive(float2[] points, int start, int end, int depth)
        {
            if (start > end)
                return null;

            int axis = depth % 2;
            int median = (start + end) / 2;

            SortByAxis(points, start, end, axis);
            KdNode node = new KdNode { Point = points[median] };

            node.Left = BuildRecursive(points, start, median - 1, depth + 1);
            node.Right = BuildRecursive(points, median + 1, end, depth + 1);

            return node;
        }

        private void SortByAxis(float2[] points, int start, int end, int axis)
        {
            System.Array.Sort(points, start, end - start + 1, Comparer<float2>.Create((a, b) =>
                axis == 0 ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y)
            ));
        }

        public KdNode FindNearest(float2 target)
        {
            return FindNearestRecursive(root, target, 0);
        }

        private KdNode FindNearestRecursive(KdNode node, float2 target, int depth)
        {
            if (node == null)
                return null;

            int axis = depth % 2;
            KdNode nextNode = target[axis] < node.Point[axis] ? node.Left : node.Right;
            KdNode otherNode = target[axis] < node.Point[axis] ? node.Right : node.Left;

            KdNode best = FindNearestRecursive(nextNode, target, depth + 1) ?? node;
            if (math.distancesq(target, node.Point) < math.distancesq(target, best.Point))
                best = node;

            float distToPlane = target[axis] - node.Point[axis];
            if (distToPlane * distToPlane < math.distancesq(target, best.Point))
            {
                KdNode possibleBest = FindNearestRecursive(otherNode, target, depth + 1);
                if (possibleBest != null && math.distancesq(target, possibleBest.Point) < math.distancesq(target, best.Point))
                    best = possibleBest;
            }

            return best;
        }
    }

    public static class Float2Extension
    {
        public static Vector2D ToVector2D(this float2 current)
        {
            return new Vector2D(current.x, current.y);
        }

        public static float2 ToFloat2(this Vector2D current)
        {
            return new float2((float)current.x, (float)current.y);
        }
    }
}
