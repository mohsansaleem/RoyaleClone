using System;

public class Vector2D
{
    public float x;
    public float y;

    public Vector2D():this(0.0f,0.0f)
    {

    }

    public Vector2D(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2D(int x, int y)
    {
        this.x = (float)x;
        this.y = (float)y;
    }

    public static Vector2D operator +(Vector2D c1, Vector2D c2)
    {
        return new Vector2D(c1.x + c2.x, c1.y + c2.y);
    }

    public static Vector2D operator -(Vector2D c1, Vector2D c2)
    {
        return new Vector2D(c1.x - c2.x, c1.y - c2.y);
    }

    public static bool operator ==(Vector2D c1, Vector2D c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Vector2D c1, Vector2D c2)
    {
        return !(c1 == c2);
    }

    public static float Distance(Vector2D c1, Vector2D c2)
    {
        return (c1 - c2).magnitude;
    }


    public int xi
    {
        get
        {
            return (int)x;
        }
    }

    public int yi
    {
        get
        {
            return (int)y;
        }
    }

    public float magnitude
    {
        get
        {
            return (float)Math.Sqrt(Math.Pow((double)x, 2) + Math.Pow((double)y, 2));
        }
    }

    public override string ToString()
    {
        return string.Format("[Vector2D: xi={0}, yi={1}]", xi, yi);
    }
}
