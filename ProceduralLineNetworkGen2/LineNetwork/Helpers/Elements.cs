using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Elements
{

    public interface ILineNetworkElement { }

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

    public enum ElementType
    {
        Point, Line, Unknown
    }

    public enum LineAtPoint
    {
        Point1, Point2
    }
}