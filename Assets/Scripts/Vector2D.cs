[System.Serializable]
public struct Vector2D
{
    public double x;
    public double y;

    public Vector2D(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2D zero => new Vector2D(0, 0);

    public static Vector2D one => new Vector2D(1, 1);

    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.x + b.x, a.y + b.y);
    }

    public static Vector2D operator -(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.x - b.x, a.y - b.y);
    }

    public static Vector2D operator *(Vector2D a, double d)
    {
        return new Vector2D(a.x * d, a.y * d);
    }

    public static Vector2D operator /(Vector2D a, double d)
    {
        return new Vector2D(a.x / d, a.y / d);
    }

    public double Magnitude()
    {
        return System.Math.Sqrt(x * x + y * y);
    }

    public static double Distance(Vector2D a, Vector2D b)
    {
        return (a - b).Magnitude();
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}

