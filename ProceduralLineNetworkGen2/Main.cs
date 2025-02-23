using System.Diagnostics;
using System.Numerics;
using GarageGoose.ProceduralLineNetwork.Module.Interface;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        //Contains primary lines and points data
        private ElementsDatabase DB = new();

        //Used for tracking various stuff on the line network and retrieving elements with specific traits.
        private TrackerHandler Tracker;

        //Used for custom behavior when expanding the line network.
        private BehaviorHandler Behavior;

        

        public LineNetwork()
        {
            Tracker  = new(DB);
            Behavior = new(DB);
        }

        public HashSet<uint> FindPointsViaTrackers(bool Multithread)
        {
            return new();
        }

        public void ExpandLineNetwork(HashSet<uint> PointKeys)
        {
            
        }
    }

    /// <summary>
    /// Parameters for the behavior when expanding a LineNetwork
    /// Every parameters is optional except for SetLineLength
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
        public LocationAttraction[] locationAttractions;
        public class LocationAttraction
        {
            public Vector2 Location;
            public float Radius;
            public float Strength;
        }

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
        /// It will still follow other behavior as much as possible so dont use other behavior parameters for the best effect.
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
        /// Sets specific angular distance to the anchored line
        /// Chosen at random if SpecificLineIndex is untouched.
        /// Overrides SetMinAngularDistToNearestLine, LocationAttraction, and SetDesiredAngle
        /// </summary>
        /// <param name="SpecificLineIndex">Get connected line index infos at FEATURE NOT IMPLEMENTED</param>
        public ExpandOnPointBehavior SetAngularDistToAnchorLine(float AngularDistToAnchorLine, int SpecificLineIndex = -1)
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
        /// Make the line go towards the location as much as possible given the earlier parameters.
        /// Each item with the same index will represent a single location attraction. 
        /// </summary>
        /// <param name="Strength"></param>
        public ExpandOnPointBehavior SetLocationAttraction(Vector2[] Location, float[] Radius, float[] Strength)
        {
            locationAttractions = new LocationAttraction[Location.Length];
            int[] LengthPerArray = {Location.Length,  Radius.Length, Strength.Length};
            Array.Sort(LengthPerArray);
            int MinArrayLengthOfAllParams = LengthPerArray[2];
            for(int i = 0; i < Location.Length; i++)
            {
                locationAttractions[i].Location = Location[i];
                locationAttractions[i].Radius = Radius[i];
                locationAttractions[i].Strength = Strength[i];
            }
            return this;
        }
    }

    /// <summary>
    /// Parameters for finding relevant points, used in LineNetwork.FindPointKeys
    /// Everything is optional
    /// </summary>
    public class FindPointsParams
    {
        public string[]? PointThatHasAnySpecifiedID;
        public string[]? PointThatHasAllSpecifiedID;

        public float? MinAngleBetweenLines;
        public float? MaxAngleBetweenLines;

        public int? MaxAreaDensity;
        public int? MinAreaDensity;

        public int? MaxLinesAtPoint;
        public int? MinLinesAtPoint;

        public Vector2? BiasOnAreaLocation;
        public float? BiasOnAreaLocationStrength;

        public int? PointsAddedAfterIndex;
        public int? PointsAddedBeforeIndex;

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
        /// Only include points that has the required min and max angular angles between lines
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

    /// <summary>
    /// These trackers will significanly slow down the line network.
    /// The option to disabe the trackers is provided here but it needs to rebuild the entire network to turn on the disabled trackers again.
    /// Parameters relying on these parameters will obviously stop working when turned off. (it wont throw an ArgumentException tho)
    /// May also slow down merging two line networks together if the disabled tracker on the merging line is enabled on the merger line.
    /// </summary>
    public class DisableTracker
    {
        /// <summary>
        /// SpawnOnPointsThatHasSpecifiedAngle will be disabled
        /// </summary>
        public bool MaxAngleAtPoint = false;

        /// <summary>
        /// SpawnOnPointsThatHasSpecifiedAngle will be disabled
        /// </summary>
        public bool MinAngleAtPoint = false;

        /// <summary>
        /// SpawnOnPointsThatOnlyHasSpecifiedAmountOfLines will be disabled.
        /// </summary>
        public bool LineCountAtPoint = false;

        /// <summary>
        /// NewLinesOnID and SetPointsOnID will be disabled.
        /// </summary>
        public bool PointID = false;

        /// <summary>
        /// PointAddAtIndexRange will be disabled.
        /// </summary>
        public bool PointAdditionOrder = false;
    }
}


