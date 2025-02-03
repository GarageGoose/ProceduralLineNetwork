using System.Numerics;
using System;

namespace GarageGoose.ProceduralLineNetwork
{
    partial class EquiangularLineNetwork
    {

    }
    partial class DynamicLineNetwork
    {
        //EquiangularPoints means lines will have predetermined angle based on the max line per point (ex, if MaxLinesPerPoint = 4, every line will be 90 deg apart)
        //MaxPointsPerLine will apply to every point on the line network. Cannot be changed once the line network is created (if equiangular points is true)
        public DynamicLineNetwork(int Seed)
        {

        }


        //These trackers significanly slow down adding new elements but is permanent when turned off.
        //Parameters relying on these parameters will obvoiusly stop working when turned off (it wont crash tho)
        public void StopTracking(bool MaxAnglePerPoint, bool MinAnglePerPoint, bool NumberOfLinesOnPoint, bool PointID)
        {

        }

        //
        public class ExpandOnLastLine
        {
            private int LineLength;
            private string PointID;
            private float LineRotationRelativeToLastLine;

            //Required
            public ExpandOnLastLine SetLineLength(int LineLength)
            {
                this.LineLength = LineLength;
                return this;
            }

            //Optional
            public ExpandOnLastLine SetPointID(string PointID)
            {
                this.PointID = PointID;
                return this;
            }

            //Required
            public ExpandOnLastLine SetLineRotationRelativeToLastLine(float LineRotationRelativeToLastLine)
            {
                this.LineRotationRelativeToLastLine = LineRotationRelativeToLastLine;
                return this;
            }


            public ExpandOnLastLine Build(int Iterations)
            {
                return this;
            }
        }
        public class ExpandOnPoint
        {
            //Initiation
            private int LineLength;
            private string? PointID;

            //Exception
            private string[] NewLinesOnID;
            private float MinAngleBetweenLines;
            private float MaxAngleBetweenLines;
            private int MaxAreaDensity;
            private int MinAreaDensity;
            private int MaxLinesAtPoint;
            private int MinLinesAtPoint;

            //Behavior
            private float SnapToPointRadius;
            private bool SnapToNearestCollision;
            private float AngularDistToAnchorLine;
            private float MinAngularDistToNearestLine;


            //Required
            public ExpandOnPoint SetLineLength(int LineLength)
            {
                this.LineLength = LineLength;
                return this;
            }

            //Optional
            public ExpandOnPoint SetPointID(string PointID)
            {
                this.PointID = PointID;
                return this;
            }

            //Optional
            public ExpandOnPoint SetNewLinesOnID(string[] NewLinesOnID)
            {
                this.NewLinesOnID = NewLinesOnID;
                return this;
            }

            //Optional
            public ExpandOnPoint SetAngleLimitBetweenLines(float Min, float Max)
            {
                MinAngleBetweenLines = Min;
                MaxAngleBetweenLines = Max;
                return this;
            }

            //Optional
            public ExpandOnPoint SetAreaDensityLimit(int Min, int Max)
            {
                MinAreaDensity = Min;
                MaxAreaDensity = Max;
                return this;
            }

            //Optional
            public ExpandOnPoint SetLineCapPerPoint(int Min, int Max)
            {
                MinLinesAtPoint = Min;
                MaxLinesAtPoint = Max;
                return this;
            }

            //Optional
            public ExpandOnPoint SetSnapToPointRadius(float SnapToPointRadius)
            {
                this.SnapToPointRadius = SnapToPointRadius;
                return this;
            }

            //Optional
            public ExpandOnPoint SetSnapToNearestCollision(bool SnapToNearestCollision)
            {
                this.SnapToNearestCollision = SnapToNearestCollision;
                return this;
            }

            //Optional
            public ExpandOnPoint SetAngularDistToAnchorLine(float AngularDistToAnchorLine)
            {
                this.AngularDistToAnchorLine = AngularDistToAnchorLine;
                return this;
            }
            
            //Optional
            public ExpandOnPoint SetMinAngularDistToNearestLine(float MinAngularDistToNearestLine)
            {
                this.MinAngularDistToNearestLine = MinAngularDistToNearestLine;
                return this;
            }
            public ExpandOnPoint Expand(int Iterations)
            {
                return this;
            }
        }
    }
}
