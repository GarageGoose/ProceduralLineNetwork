using System.Numerics;
using System;

namespace GarageGoose.ProceduralLineNetwork
{
    //EquiangularPoints means lines will have predetermined angle based on the max line per point (ex, if MaxLinesPerPoint = 4, every line will be 90 deg apart)
    //Very limited in functionality but is significanltly more performant
    partial class EquiangularLineNetwork
    {

    }

    //Lines can have any angles regardless of amount on points
    //Worse in performance but is significanltly more customizable
    partial class DynamicLineNetwork
    {
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
            private string[]? PointID;

            //Exception
            private string[] NewLinesOnPointThatHasAnySpecifiedID;
            private string[] NewLinesOnPointThatHasAllSpecifiedID;
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
            //Sets the length of the line
            public ExpandOnPoint SetLineLength(int LineLength)
            {
                this.LineLength = LineLength;
                return this;
            }

            //Optional
            //Set custom point identifier(s). Used on SetNewLinesOnID.
            public ExpandOnPoint SetPointID(string[] PointID)
            {
                this.PointID = PointID;
                return this;
            }

            //Optional
            //Spawn new lines on point that has any or all specified ID(s)
            public ExpandOnPoint NewLinesOnID(string[] NewLinesOnID, bool MustIncludeAll)
            {
                if (MustIncludeAll)
                {
                    NewLinesOnPointThatHasAllSpecifiedID = NewLinesOnID;
                    return this;
                }
                NewLinesOnPointThatHasAnySpecifiedID = NewLinesOnID;
                return this;
            }

            //Optional
            //Spawn only on points that has the required min and max angles between lines
            public ExpandOnPoint SpawnOnPointsThatHasSpecifiedAngle(float Min, float Max)
            {
                MinAngleBetweenLines = Min;
                MaxAngleBetweenLines = Max;
                return this;
            }

            //Optional
            //Spawn only on points that falls within the specified area density
            public ExpandOnPoint AreaDensityLimit(int Min, int Max)
            {
                MinAreaDensity = Min;
                MaxAreaDensity = Max;
                return this;
            }

            //Optional
            //Spawn only on points that has the required min and max amount of lines
            public ExpandOnPoint SpawnOnPointsThatOnlyHasSpecifiedAmountOfLines(int Min, int Max)
            {
                MinLinesAtPoint = Min;
                MaxLinesAtPoint = Max;
                return this;
            }

            //Optional
            //The end of the line will connect to the nearest point that is within the radius as oppose to adding another point near that point
            public ExpandOnPoint SetSnapToPointRadius(float SnapToPointRadius)
            {
                this.SnapToPointRadius = SnapToPointRadius;
                return this;
            }

            //Optional
            //Line will stop at collision and make an intersection between this line and the colliding line
            //Overrides SetSnapToPointRadius
            public ExpandOnPoint SetSnapToNearestCollision(bool SnapToNearestCollision)
            {
                this.SnapToNearestCollision = SnapToNearestCollision;
                return this;
            }

            //Optional
            //Sets specific angular distance to the anchored line (chosen at random but depends on when SetAngleLimitBetweenLines is actice)
            //Overrides SetMinAngularDistToNearestLine
            public ExpandOnPoint SetAngularDistToAnchorLine(float AngularDistToAnchorLine)
            {
                this.AngularDistToAnchorLine = AngularDistToAnchorLine;
                return this;
            }

            //Optional
            //Prevents line from spawning too close in angular distance to the nearest line
            public ExpandOnPoint SetMinAngularDistToNearestLine(float MinAngularDistToNearestLine)
            {
                this.MinAngularDistToNearestLine = MinAngularDistToNearestLine;
                return this;
            }

            //Initiates expansion
            public ExpandOnPoint Expand(int Iterations)
            {
                return this;
            }
        }
    }

    public class MergeLineNetworks
    {

    }

}


