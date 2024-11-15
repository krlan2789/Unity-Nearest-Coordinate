using System;
using UnityEngine;

namespace NearestCoordinate
{
    public class KD3
    {
        private class KDNode
        {
            public double[] Point { get; set; }
            public KDNode Left { get; set; }
            public KDNode Right { get; set; }

            public KDNode(double[] point)
            {
                Point = point;
                Left = null;
                Right = null;
            }
        }

        private KDNode root;
        private int k;

        public KD3(int dimensions)
        {
            k = dimensions;
            root = new KDNode(new double[k]);
        }

        public void Insert(double[] point)
        {
            root = InsertRec(root, point, 0);
        }

        public void Insert(double[][] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                root = InsertRec(root, points[i], 0);
            }
        }

        private KDNode InsertRec(KDNode node, double[] point, int depth)
        {
            if (node == null)
            {
                return new KDNode(point);
            }

            int cd = depth % k;
            //Debug.Log($"{depth} % {k} = {cd} | point.Length: {point.Length} | node.Point.Length: {node.Point?.Length}");

            if (point[cd] < node.Point[cd])
            {
                node.Left = InsertRec(node.Left, point, depth + 1);
            } else
            {
                node.Right = InsertRec(node.Right, point, depth + 1);
            }

            return node;
        }

        public double[] Nearest(double[] target)
        {
            return NearestRec(root, target, 0, new KDNode(null)).Point;
        }

        private KDNode NearestRec(KDNode node, double[] target, int depth, KDNode best)
        {
            if (node == null)
            {
                return best;
            }

            if (best == null || Distance(node.Point, target) < Distance(best.Point, target))
            {
                best = node;
            }

            int cd = depth % k;

            KDNode goodSide = target[cd] < node.Point[cd] ? node.Left : node.Right;
            KDNode badSide = target[cd] < node.Point[cd] ? node.Right : node.Left;

            best = NearestRec(goodSide, target, depth + 1, best);
            if (Math.Abs(node.Point[cd] - target[cd]) < Distance(best.Point, target))
            {
                best = NearestRec(badSide, target, depth + 1, best);
            }

            return best;
        }

        private double Distance(double[] a, double[] b)
        {
            double dist = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dist += Math.Pow(a[i] - b[i], 2);
            }
            return Math.Sqrt(dist);
        }
    }
}
