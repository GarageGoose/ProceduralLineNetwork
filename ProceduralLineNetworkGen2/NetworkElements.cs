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

        //TKey   = Angle of the connected line
        //TValue = Key of the line
        public SortedList<float, int> ConnectedLines = new();
        
        public Point(Vector2 Location, string[]? ID = null)
        {
            this.Location = Location;
            this.ID = ID;
        }

        public void AddLine(int LineKey, float Angle)
        {
            ConnectedLines.Add(Angle, LineKey);
        }
    }

    public class Line
    {
        public int PointKey1;
        public int PointKey2;
        public Line(int PointKey1, int PointKey2)
        {
            this.PointKey1 = PointKey1;
            this.PointKey2 = PointKey2;
        }
    }
}