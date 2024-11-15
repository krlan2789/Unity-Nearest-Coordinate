namespace NearestCoordinate
{
    [System.Serializable]
    public class ClusterNode<T>
    {
        public T data;
        public Vector2D Point { get; set; }
    }
}
