using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NearestCoordinate
{
    public class KMeansClustering<T>
    {
        public uint NumberOfClusters { get; private set; }
        public uint ClusteringIteration { get; private set; }

        private List<ClusterNode<T>>[] clusters;
        private Vector2D[] centroids;

        public KMeansClustering(uint numberOfClusters = 4, uint clusteringIteration = 16)
        {
            NumberOfClusters = numberOfClusters;
            ClusteringIteration = clusteringIteration;
        }

        /// <summary>
        /// Perform clustering (k-means algorithm)
        /// </summary>
        /// <param name="coordinates">Coordinates List</param>
        public void PerformClustering(List<ClusterNode<T>> coordinates)
        {
            clusters = null;
            centroids = null;

            // Step 1: Initialize clusters and centroids
            clusters = new List<ClusterNode<T>>[NumberOfClusters];
            for (int i = 0; i < NumberOfClusters; i++)
            {
                clusters[i] = new List<ClusterNode<T>>();
            }

            // Initialize centroids
            centroids = new Vector2D[NumberOfClusters];
            for (int i = 0; i < NumberOfClusters; i++)
            {
                centroids[i] = coordinates[Random.Range(0, coordinates.Count - 1)].Point;
            }

            // Perform k-means clustering
            for (int iteration = 0; iteration < ClusteringIteration; iteration++)
            {
                // Clear clusters
                for (int i = 0; i < NumberOfClusters; i++)
                {
                    clusters[i].Clear();
                }

                // Assign coordinates to the nearest centroid
                foreach (var coord in coordinates)
                {
                    int nearestClusterIndex = 0;
                    double minDistance = Vector2D.Distance(coord.Point, centroids[0]);
                    for (int i = 1; i < centroids.Length; i++)
                    {
                        double distance = Vector2D.Distance(coord.Point, centroids[i]);
                        if (distance < minDistance)
                        {
                            nearestClusterIndex = i;
                            minDistance = distance;
                        }
                    }
                    clusters[nearestClusterIndex].Add(coord);
                }

                // Recalculate centroids
                for (int i = 0; i < NumberOfClusters; i++)
                {
                    if (clusters[i].Count > 0)
                    {
                        Vector2D sum = Vector2D.zero;
                        foreach (var coord in clusters[i])
                        {
                            sum += coord.Point;
                        }
                        centroids[i] = sum / clusters[i].Count;
                    }
                }
            }

            CalculateCentroids();
        }

        /// <summary>
        /// Calculate centroids
        /// </summary>
        public void CalculateCentroids()
        {
            centroids = new Vector2D[NumberOfClusters];
            for (int i = 0; i < NumberOfClusters; i++)
            {
                Vector2D sum = Vector2D.zero;
                for (int j = 0; j < clusters[i].Count; j++)
                {
                    sum += clusters[i][j].Point;
                }
                centroids[i] = sum / clusters[i].Count;

                Debug.Log($"Clusters[{i}]: {string.Join(", ", clusters[i].Select(x => x.Point).ToList())}\ncentroids[{i}]: {centroids[i]}");
            }
        }

        public ClusterNode<T> FindNearest(Vector2D queryCoordinate)
        {
            // Step 3: Find nearest cluster
            int nearestClusterIndex = 0;
            double minDistance = Vector2D.Distance(queryCoordinate, centroids[0]);
            for (int i = 1; i < centroids.Length; i++)
            {
                double distance = Vector2D.Distance(queryCoordinate, centroids[i]);
                if (distance < minDistance)
                {
                    nearestClusterIndex = i;
                    minDistance = distance;
                }

                Debug.Log($"centroids[{i}]: {centroids[i]}\nDistance with {queryCoordinate}: {distance}\nNearest Distance: {minDistance}");
            }

            // Step 4: Search within the nearest cluster
            var nearestCoordinate = clusters[nearestClusterIndex][0];
            minDistance = Vector2D.Distance(queryCoordinate, nearestCoordinate.Point);
            for (int i = 1; i < clusters[nearestClusterIndex].Count; i++)
            {
                double distance = Vector2D.Distance(queryCoordinate, clusters[nearestClusterIndex][i].Point);
                if (distance < minDistance)
                {
                    nearestCoordinate = clusters[nearestClusterIndex][i];
                    minDistance = distance;
                }
            }

            return nearestCoordinate;
        }
    }
}
