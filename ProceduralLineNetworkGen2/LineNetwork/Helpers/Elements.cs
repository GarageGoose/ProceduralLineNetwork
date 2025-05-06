namespace GarageGoose.ProceduralLineNetwork.Elements
{
    /// <summary>
    /// Marker interface for the elements of a line network.
    /// </summary>
    public interface ILineNetworkElement { }

    /// <summary>
    /// Represents a location in 2d space where a line could connect to it.
    /// </summary>
    public class Point : ILineNetworkElement
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;
    }

    /// <summary>
    /// Represents a connection between two points.
    /// </summary>
    public class Line : ILineNetworkElement
    {
        public Line(uint pointKey1, uint pointKey2)
        {
            this.PointKey1 = pointKey1;
            this.PointKey2 = pointKey2;
        }
        public uint PointKey1;
        public uint PointKey2;
    }

    /// <summary>
    /// An enum for identifiying the type of an element.
    /// </summary>
    public enum ElementType
    {
        Point, Line, Unknown
    }

    /// <summary>
    /// An enum for identifiying the specified end point of a line.
    /// </summary>
    public enum LineEndPoint
    {
        Point1, Point2
    }
}