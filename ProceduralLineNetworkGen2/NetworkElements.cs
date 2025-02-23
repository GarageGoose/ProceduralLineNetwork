using System;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public class Point
    {
        public Vector2 Location;
    }

    public class Line
    {
        public uint PointKey1;
        public uint PointKey2;
    }
}