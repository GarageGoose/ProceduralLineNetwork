namespace GarageGoose.ProceduralLineNetwork.Elements
{
    /// <summary>
    /// Marker interface for the elements of a line network.
    /// </summary>
    public interface ILineNetworkElement { }

    /// <summary>
    /// Represents a location in 2d space where a line could connect to it.
    /// </summary>
    public struct Point : ILineNetworkElement
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public readonly float x;
        public readonly float y;
    }

    /// <summary>
    /// Represents a connection between two points.
    /// </summary>
    public struct Line : ILineNetworkElement
    {
        public Line(uint pointKey1, uint pointKey2)
        {
            PointKey1 = pointKey1;
            PointKey2 = pointKey2;
        }
        public readonly uint PointKey1;
        public readonly uint PointKey2;
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