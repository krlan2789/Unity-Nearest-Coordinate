using System.Collections.Generic;
using UnityEngine;

namespace NearestCoordinate
{
    public class Clusterer
    {
        public uint NumberOfClusters { get; private set; }
        public uint ClusteringIteration { get; private set; }

        private List<Vector2D>[] clusters;
        private Vector2D[] centroids;

        public Clusterer(uint numberOfClusters = 4, uint clusteringIteration = 16)
        {
            NumberOfClusters = numberOfClusters;
            ClusteringIteration = clusteringIteration;
        }

        /// <summary>
        /// Perform clustering (k-means or other algorithm)
        /// </summary>
        /// <param name="coordinates">Coordinates List</param>
        /// <param name="numberOfClusters">Number of clusters</param>
        /// <param name="clusteringIteration">Number iteration of clustering</param>
        public void PerformClustering(List<Vector2D> coordinates)
        {
            // Step 1: Initialize clusters and centroids
            clusters = new List<Vector2D>[NumberOfClusters];
            for (int i = 0; i < NumberOfClusters; i++)
            {
                clusters[i] = new List<Vector2D>();
            }

            // Initialize centroids
            centroids = new Vector2D[NumberOfClusters];
            for (int i = 0; i < NumberOfClusters; i++)
            {
                centroids[i] = coordinates[Random.Range(0, coordinates.Count - 1)];
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
                    double minDistance = Vector2D.Distance(coord, centroids[0]);
                    for (int i = 1; i < centroids.Length; i++)
                    {
                        double distance = Vector2D.Distance(coord, centroids[i]);
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
                            sum += coord;
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
                    sum += clusters[i][j];
                }
                centroids[i] = sum / clusters[i].Count;

                Debug.Log($"Clusters[{i}]: {string.Join(", ", clusters[i])}\ncentroids[{i}]: {centroids[i]}");
            }
        }

        public Vector2D FindNearest(Vector2D queryCoordinate)
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
            Vector2D nearestCoordinate = clusters[nearestClusterIndex][0];
            minDistance = Vector2D.Distance(queryCoordinate, nearestCoordinate);
            for (int i = 1; i < clusters[nearestClusterIndex].Count; i++)
            {
                double distance = Vector2D.Distance(queryCoordinate, clusters[nearestClusterIndex][i]);
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
