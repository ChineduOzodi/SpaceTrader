using UnityEngine;
public struct Coord
{
    public int x;
    public int y;

    public static Coord zero = new Coord();
    //public Coord()
    //{
    //    x = 0;
    //    y = 0;
    //}
    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Coord( Vector2 vect)
    {
        x = (int) vect.x;
        y = (int) vect.y;
    }
    public Coord(Vector2d vect)
    {
        x = (int)vect.x;
        y = (int)vect.y;
    }

    /// <summary>
    /// Creates Coord from Vector
    /// </summary>
    /// <param name="vect"></param>
    /// <returns></returns>
    public static Coord Vector3ToCoord(Vector3 vect)
    {
        Coord coord = new Coord((int) vect.x, (int) vect.y);

        return coord;
    }

    public static Coord operator +(Coord coordA, Coord coordB)
    {
        Coord coord = new Coord(coordA.x + coordB.x, coordA.y + coordB.y);

        return coord;
    }

    /// <summary>
    /// Subtract the x and y values of the coord simultaneuously
    /// </summary>
    /// <param name="coordA"></param>
    /// <param name="coordB"></param>
    /// <returns></returns>
    public static Coord operator -(Coord coordA, Coord coordB)
    {
        Coord coord = new Coord(coordA.x - coordB.x, coordA.y - coordB.y);

        return coord;
    }
    /// <summary>
    /// Tests to see if the integers in the coords are equal
    /// </summary>
    /// <param name="coordA"></param>
    /// <param name="coordB"></param>
    /// <returns></returns>
    public static bool operator ==(Coord coordA, Coord coordB)
    {
        bool test = false;

        if (coordA.x == coordB.x && coordA.y == coordB.y)
            test = true;

        return test;
    }
    public static bool operator !=(Coord coordA, Coord coordB)
    {
        bool test = true;

        if (coordA.x == coordB.x && coordA.y == coordB.y)
            test = false;

        return test;
    }

    public override bool Equals(object obj)
    {
        return ((Coord)obj == this);
    }

    public override int GetHashCode()
    {
        return ("x" + x +"y" + y).GetHashCode();
    }

    public override string ToString()
    {
        return "x: " + x + " y: " + y;
    }
}