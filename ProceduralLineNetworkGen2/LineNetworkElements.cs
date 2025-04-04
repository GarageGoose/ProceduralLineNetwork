using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public struct Point
    {
        public Vector2 Location;
    }

    public struct Line
    {
        public uint PointKey1;
        public uint PointKey2;
    }
}