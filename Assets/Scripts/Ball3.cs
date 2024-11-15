using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NearestCoordinate
{
    public class Ball3
    {
        public class Node
        {
            public Vector2D Point { get; set; }
            public double Radius { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public Node(Vector2D point, double radius)
            {
                Point = point;
                Radius = radius;
                Left = null;
                Right = null;
            }
        }

        private Node root;
        public Node bestNode;

        public Ball3()
        {
            root = null;
        }

        public Ball3(int dimensions)
        {
            if (dimensions <= 0)
            {
                throw new ArgumentException("Number of dimensions must be greater than zero.");
            }
            root = null;
        }

        public async Task InsertAsync(Vector2D point)
        {
            root = await InsertRecAsync(root, point);
        }

        private async Task<Node> InsertRecAsync(Node node, Vector2D point)
        {
            if (node == null)
            {
                return new Node(point, 0);
            }

            double distance = Vector2D.Distance(node.Point, point);

            if (distance <= node.Radius)
            {
                node.Left = await InsertRecAsync(node.Left, point);
            } else
            {
                node.Right = await InsertRecAsync(node.Right, point);
                node.Radius = distance;
            }

            return node;
        }

        public async Task<Node> NearestAsync(Vector2D target)
        {
            return (await NearestRecAsync(root, target, root.Point, double.MaxValue));
        }

        private async Task<Node> NearestRecAsync(Node node, Vector2D target, Vector2D bestPoint, double bestDistance)
        {
            if (node == null)
            {
                return new Node(bestPoint, bestDistance);
            }

            double distance = Vector2D.Distance(node.Point, target);

            if (distance < bestDistance)
            {
                bestPoint = node.Point;
                bestDistance = distance;
            }

            Node goodSide = Vector2D.Distance(target, node.Point) <= bestDistance ? node.Left : node.Right;
            Node badSide = Vector2D.Distance(target, node.Point) > bestDistance ? node.Left : node.Right;

            bestPoint = (await NearestRecAsync(goodSide, target, bestPoint, bestDistance)).Point;
            
            if (Vector2D.Distance(bestPoint, target) > bestDistance)
            {
                var badSidePoint = (await NearestRecAsync(badSide, target, bestPoint, bestDistance)).Point;
                var goodSidePoint = (await NearestRecAsync(goodSide, target, bestPoint, bestDistance)).Point;
                node =
                    Vector2D.Distance(goodSidePoint, target) < Vector2D.Distance(badSidePoint, target)
                    ? goodSide : badSide;

                var badSideRightPoint = (await NearestRecAsync(node, target, bestPoint, bestDistance)).Point;
                var goodSideLeftPoint = (await NearestRecAsync(node, target, bestPoint, bestDistance)).Point;
                bestPoint =
                    Vector2D.Distance(goodSideLeftPoint, target) < Vector2D.Distance(badSideRightPoint, target)
                    ? goodSideLeftPoint : badSideRightPoint;
            }

            Debug.Log(
                $"-> Target: {target}\n" +
                $"-> Point: {node?.Point ?? Vector2D.zero}, {Vector2D.Distance(target, node?.Point ?? Vector2D.zero)}\n" +
                $"-> Point.Left: {node?.Left?.Point ?? Vector2D.zero}, {Vector2D.Distance(target, node?.Left?.Point ?? Vector2D.zero)}\n" +
                $"-> Point.Right: {node?.Right?.Point ?? Vector2D.zero}, {Vector2D.Distance(target, node?.Right?.Point ?? Vector2D.zero)}\n" +
                $"-> Nearest: {bestDistance}");

            return new Node(bestPoint, bestDistance);
        }

        public async IAsyncEnumerable<Vector2D> GetAllPointsAsync()
        {
            if (root == null) yield break;

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node.Point;

                if (node.Left != null) queue.Enqueue(node.Left);
                if (node.Right != null) queue.Enqueue(node.Right);

                // Await to simulate async operation
                await Task.Yield();
            }
        }
    }
}
