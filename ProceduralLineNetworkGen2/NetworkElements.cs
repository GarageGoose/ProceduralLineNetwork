using System;
using System.Numerics;

public class Element
{
    public class Point
    {
        public Vector2 Location;

        public string[]? ID;
        public float? MinAngle = MathF.PI * 2;
        public float? MaxAngle = MathF.PI * 2;
        public uint Key;

        //TKey   = Angle of the connected line
        //TValue = Key of the line
        public SortedList<float, uint> ConnectedLines = new();
        
        public Point(Vector2 Location, string[]? ID = null)
        {
            this.Location = Location;
            this.ID = ID;
        }

        public void AddLine(uint LineKey, float Angle)
        {
            ConnectedLines.Add(Angle, LineKey);
        }
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