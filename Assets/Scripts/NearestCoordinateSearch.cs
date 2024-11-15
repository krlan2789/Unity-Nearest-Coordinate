using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace NearestCoordinate
{
    public class NearestCoordinateSearch : MonoBehaviour
    {
        public enum Algorythm
        {
            BALL_TREE = 0,
            CLUSTERER_KMEANS = 2,
        }

        public Algorythm algorythmUse = Algorythm.CLUSTERER_KMEANS;
        public Button searchBtn;
        public Text resultTxt;
        public InputField xInput, yInput;

        public int dataLength = 200;
        public Vector2D rangeCoordinate = new Vector2D(-50, 50);

        private Vector2D targetPoint;
        private readonly List<Vector2D> points = new List<Vector2D>();

        // Ball Tree
        private Ball3 ball3Algo;

        // Ball Tree
        private KMeansClustering<string> clustererAlgo;

        private void Start()
        {
            xInput.text = targetPoint.x.ToString();
            yInput.text = targetPoint.y.ToString();

            searchBtn.onClick.AddListener(Search);
            StartCoroutine(Initializing());
        }

        public IEnumerator Initializing()
        {
            yield return null;

            if (dataLength > 0)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    yield return null;
                    Vector2D point = new(Random.Range((float)rangeCoordinate.x, (float)rangeCoordinate.y), Random.Range((float)rangeCoordinate.x, (float)rangeCoordinate.y));
                    points.Add(point);
                }
            } else
            {
                points.Add(new(-8, -8)); // Store 1
                points.Add(new(-16, -9)); // Store 2
                points.Add(new(20, 40)); // Store 3
                points.Add(new(-17, 1)); // Store 4
                points.Add(new(13, -30)); // Store 5
                points.Add(new(40, 10)); // Store 6
                points.Add(new(42, -22)); // Store 7
                points.Add(new(-36, 29)); // Store 8
                points.Add(new(-33, -20)); // Store 9
            }

            switch (algorythmUse)
            {
                case Algorythm.BALL_TREE:
                    Ball3InitializeAsync();
                    break;
                default:
                    ClusterInitializeAsync();
                    break;
            }
        }

        public void Search()
        {
            targetPoint = new Vector2D(double.Parse(xInput.text), double.Parse(yInput.text));
            searchBtn.interactable = false;

            switch (algorythmUse)
            {
                case Algorythm.BALL_TREE:
                    Ball3Search();
                    break;
                default:
                    ClusterSearch();
                    break;
            }

            searchBtn.interactable = true;
        }

        #region Ball3
        public async void Ball3InitializeAsync()
        {
            ball3Algo = new Ball3();

            foreach (var point in points)
            {
                await ball3Algo.InsertAsync(new Vector2D(point.x, point.y));
            }
        }

        public async void Ball3Search()
        {
            Ball3.Node nearest = await ball3Algo.NearestAsync(targetPoint);
            resultTxt.text =
                $"-> Target: {targetPoint}\n" +
                $"-> Point: {nearest.Point}, {Vector2D.Distance(targetPoint, nearest.Point)}";

            Debug.Log($"Index[{points.IndexOf(nearest.Point)}]:\n{resultTxt.text}");
        }
        #endregion Ball3

        #region Cluster - K-Means
        public void ClusterInitializeAsync()
        {
            clustererAlgo = new KMeansClustering<string>(4, 32);

            var nodes = new List<ClusterNode<string>>();

            int a = 1;
            foreach (var point in points)
            {
                nodes.Add(new ClusterNode<string>()
                {
                    Point = point,
                    data = $"Store {a}"
                });
                a++;
            }

            clustererAlgo.PerformClustering(nodes);
        }

        public void ClusterSearch()
        {
            var nearest = clustererAlgo.FindNearest(targetPoint);
            resultTxt.text =
                $"-> Target: {targetPoint}\n" +
                $"-> Nearest: {nearest.data} {nearest.Point}\nDistance: {Vector2D.Distance(targetPoint, nearest.Point)}";

            Debug.Log($"Index[{points.IndexOf(nearest.Point)}]: {nearest.data}\n{resultTxt.text}");
        }
        #endregion Cluster - K-Means
    }
}
