using System;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public class Point
    {
        public Vector2 Location;

        public Dictionary<uint, object> TrackerData;
    }

    public class Line
    {
        public uint PointKey1;
        public uint PointKey2;
        public Line(uint PointKey1, uint PointKey2)
        {
            this.PointKey1 = PointKey1;
            this.PointKey2 = PointKey2;
        }
    }
}