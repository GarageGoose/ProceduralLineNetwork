using System.Numerics;
using System;

namespace GarageGoose.ProceduralLineNetwork
{
    partial class LineNetwork
    {
        public LineNetwork(int Seed)
        {

        }

        /// <summary>
        /// These trackers significanly slow down adding new elements but is permanent when turned off.
        /// Parameters relying on these parameters will obvoiusly stop working when turned off (it wont crash tho)
        /// </summary>
        public void StopTracking(bool MaxAnglePerPoint, bool MinAnglePerPoint, bool NumberOfLinesOnPoint, bool PointID)
        {

        }

        /// <summary>
        /// Find eligible point keys based on the parameters
        /// More efficient to find many point keys at one due to how the finding algorithm work (see NetworkManager.cs)
        /// </summary>
        /// <param name="Limit">Limits the size of the array</param>
        /// <param name="Params">Set of rules for finding points</param>
        /// <returns></returns>
        public int[] FindPointKeys(FindPointsParams Params, int Limit)
        {
            return new int[0];
        }

        /// <summary>
        /// Expand the line network based on the behavior
        /// </summary>
        /// <param name="Behavior">Set of rules for the behavior</param>
        /// <param name="PointKeysArray">Use multiple points at once</param>
        /// <param name="PointKey">Use a single point</param>
        public void ExpandNetwork(ExpandOnPointBehavior Behavior, int[]? PointKeysArray = null, int? PointKey = null)
        {

        }
    }

    public class MergeLineNetworks
    {

    }

    /// <summary>
    /// Parameters for the behavior when expanding a LineNetwork
    /// </summary>
    public class ExpandOnPointBehavior
    {
        //Initiation
        public int LineLength;
        public string[] PointID;

        //Behavior
        public float DesiredAngle;
        public float SnapToPointRadius;
        public bool SnapToNearestCollision;
        public float AngularDistToAnchorLine;
        public float MinAngularDistToNearestLine;
        public Vector2 LocationAttraction;
        public float LocationAttractionStrength;

        /// <summary>
        /// Required, sets line length
        /// </summary>
        public ExpandOnPointBehavior SetLineLength(int LineLength)
        {
            this.LineLength = LineLength;
            return this;
        }

        /// <summary>
        /// Set custom point identifier(s) on the newly created point (does not change current point). Used on NewLinesOnID.
        /// </summary>
        public ExpandOnPointBehavior SetPointID(string[] PointID)
        {
            this.PointID = PointID;
            return this;
        }

        /// <summary>
        /// Make the line face a certain direction.
        /// It will still follow other behavior as much as possible, dont use other behavior parameters for best effect.
        /// </summary>
        public ExpandOnPointBehavior SetDesiredAngle(float DesiredAngleRadians)
        {
            DesiredAngle = DesiredAngleRadians;
            return this;
        }

        /// <summary>
        /// The end of the line will connect to the nearest point that is within the radius as oppose to adding another point near that point
        /// </summary>
        public ExpandOnPointBehavior SetSnapToPointRadius(float SnapToPointRadius)
        {
            this.SnapToPointRadius = SnapToPointRadius;
            return this;
        }

        /// <summary>
        /// Line will stop at collision and make an intersection between this line and the colliding line
        /// Overrides SetSnapToPointRadius only when both conditions occur
        /// </summary>
        /// <param name="SnapToNearestCollision"></param>
        public ExpandOnPointBehavior SetSnapToNearestCollision(bool SnapToNearestCollision)
        {
            this.SnapToNearestCollision = SnapToNearestCollision;
            return this;
        }

        /// <summary>
        /// Sets specific angular distance to the anchored line (chosen at random but depends on when SetAngleLimitBetweenLines is actice)
        /// Overrides SetMinAngularDistToNearestLine, LocationAttraction, and SetDesiredAngle
        /// </summary>
        public ExpandOnPointBehavior SetAngularDistToAnchorLine(float AngularDistToAnchorLine)
        {
            this.AngularDistToAnchorLine = AngularDistToAnchorLine;
            return this;
        }

        /// <summary>
        /// Prevents line from spawning too close in angular distance to the nearest line
        /// </summary>
        public ExpandOnPointBehavior SetMinAngularDistToNearestLine(float MinAngularDistToNearestLine)
        {
            this.MinAngularDistToNearestLine = MinAngularDistToNearestLine;
            return this;
        }

        /// <summary>
        /// Make the line go towards the location as much as possible given the earlier parameters
        /// </summary>
        public ExpandOnPointBehavior SetLocationAttraction(Vector2 Location, float LocationAttractionStrength)
        {
            LocationAttraction = Location;
            this.LocationAttractionStrength = LocationAttractionStrength;
            return this;
        }
    }

    /// <summary>
    /// Parameters for finding relevant points, used in LineNetwork.FindPointKeys
    /// </summary>
    public class FindPointsParams
    {
        public string[] PointThatHasAnySpecifiedID;
        public string[] PointThatHasAllSpecifiedID;

        public float MinAngleBetweenLines;
        public float MaxAngleBetweenLines;

        public int MaxAreaDensity;
        public int MinAreaDensity;

        public int MaxLinesAtPoint;
        public int MinLinesAtPoint;

        public Vector2 BiasOnAreaLocation;
        public float BiasOnAreaLocationStrength;

        public int PointsAddedAfterIndex;
        public int PointsAddedBeforeIndex;

        /// <summary>
        /// Only include points that is within range in order of when the point is created
        /// </summary>
        public FindPointsParams PointAddedAtIndexRange(int Min, int Max)
        {
            PointsAddedAfterIndex = Min;
            PointsAddedBeforeIndex = Max;
            return this;
        }

        /// <summary>
        /// Only include points that has any or all specified ID(s)
        /// </summary>
        /// <param name="MustIncludeAll">Finds only points that include all the listed IDs as oppose to just having one of the required IDs</param>
        public FindPointsParams NewLinesOnID(string[] NewLinesOnID, bool MustIncludeAll)
        {
            if (MustIncludeAll)
            {
                PointThatHasAllSpecifiedID = NewLinesOnID;
                return this;
            }
            PointThatHasAnySpecifiedID = NewLinesOnID;
            return this;
        }


        /// <summary>
        /// Only include points that has the required min and max angles between lines
        /// </summary>
        public FindPointsParams SpawnOnPointsThatHasSpecifiedAngle(float Min, float Max)
        {
            MinAngleBetweenLines = Min;
            MaxAngleBetweenLines = Max;
            return this;
        }

        /// <summary>
        /// Only include points that falls within the specified area density
        /// </summary>
        public FindPointsParams AreaDensityLimit(int Min, int Max)
        {
            MinAreaDensity = Min;
            MaxAreaDensity = Max;
            return this;
        }

        /// <summary>
        /// Choose more eligible points specifically inside an area
        /// </summary>
        /// <param name="Strength">Works like a ratio between point inside and outside the area, takes value from 0 to 1</param>
        public FindPointsParams LocationBias(Vector2 Location, float Strength = 0.5f)
        {
            BiasOnAreaLocation = Location;
            BiasOnAreaLocationStrength = Strength;
            return this;
        }

        /// <summary>
        /// Only include points that has the required min and max amount of lines
        /// </summary>
        public FindPointsParams SpawnOnPointsThatOnlyHasSpecifiedAmountOfLines(int Min, int Max)
        {
            MinLinesAtPoint = Min;
            MaxLinesAtPoint = Max;
            return this;
        }


    }
}


