using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public class Point
    {
        public float x;
        public float y;
    }

    public class Line
    {
        public uint PointKey1;
        public uint PointKey2;
    }
}