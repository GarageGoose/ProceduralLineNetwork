using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        //Handles storage of the line network
        private ElementsDatabase DB;

        //Handles modifiying and retirieving data from the line network
        private ElementsDatabaseHandler DBHandle;

        //Handles various computation needed to find points and to expand the network
        private NetworkCompute Compute;

        public LineNetwork()
        {
            DBHandle = new(ref DB);
            Compute = new(ref DB);
        }

        /// <summary>
        /// These trackers significanly slow down the line network overall. Rebuilding the entire network is needed to turn back those parameters on 
        /// Parameters relying on these parameters will obviously stop working when turned off (it wont throw an ArgumentException tho)
        /// </summary>
        public void DisableTracker(DisableTracker DT)
        {
            
        }

        /// <summary>
        /// Find eligible point keys based on the parameters
        /// More efficient to find many point keys at once due to how the finding algorithm work (see NetworkManager.cs)
        /// </summary>
        /// <param name="Limit">Limits the size of the array</param>
        /// <param name="Params">Set of rules for finding points</param>
        /// <param name="Multithread">Performs multithreading, increase performance if many parameters are in use</param>
        public HashSet<int> FindPointKeys(FindPointsParams Params, int Limit, bool Multithread)
        {
            return new();
        }

        /// <summary>
        /// Expand the line network based on the behavior
        /// </summary>
        /// <param name="Behavior">Set of rules for the behavior</param>
        /// <param name="Multithread">Performs multithreading, will increase performance if there are many points in HashSet or Array</param>
        /// <param name="PointKeysArray">Use multiple PointKeys in an array</param>
        /// <param name="PointKeysHashSet">Use multiple PointKeys in an hashset, prioritized if both isnt null</param>
        public void ExpandNetwork(ExpandOnPointBehavior Behavior, bool Multithread, int[]? PointKeysArray = null, HashSet<int>? PointKeysHashSet = null)
        {

        }

        /// <summary>
        /// Manually add a point to the line network.
        /// </summary>
        /// <param name="Key">Used to identify this specific point internally</param>
        /// <param name="ID">Used to identify this specific point (or a group of points) via custom IDs (good for categorizing points, etc.)</param>
        public void AddPoint(Vector2 Location, out uint Key, string[]? ID = null)
        {
            Key = 0;
        }

        /// <summary>
        /// Manually add a line to the line network.
        /// </summary>
        /// <param name="Point1Key">First  point the line connects to</param>
        /// <param name="Point2Key">Second point the line connects to</param>
        /// <param name="Key">Used to identify this specific line internally</param>
        public void AddLine(uint Point1Key, uint Point2Key, out uint Key)
        {
            Key = 0;
        }

        /// <summary>
        /// Manually delete a point, all connected lines going to it will also be deleted.
        /// PointKey is prioritized when both isnt null
        /// </summary>
        /// <param name="Index">Get point by index (sorted by oldest to newest)</param>
        /// <param name="PointKey">Get point by key</param>
        public void DeletePoint(int? Index = null, uint? PointKey = null)
        {

        }

        /// <summary>
        /// Manually delete a line.
        /// LineKey is prioritized when both isnt null
        /// </summary>
        /// <param name="Index">Get point by index (sorted by oldest to newest)</param>
        /// <param name="LineKey">Get point by key</param>
        public void DeleteLine(int? Index = null, uint? LineKey = null)
        {

        }

        /// <summary>
        /// Merge two line networks. Selected points on both network will be merged to a single one.
        /// </summary>
        /// <param name="MergingLN">Line network to be merged</param>
        /// <param name="MergeAtThisPointKey">Point to be merged at this line network</param>
        /// <param name="MergeAtTheMeringLNPointKey">Point to be merged on the merging line network</param>
        public void MergeLineNetworks(LineNetwork MergingLN, uint MergeAtThisPointKey, uint MergeAtTheMeringLNPointKey)
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
        /// Make the line go towards the location as much as possible given the earlier parameters
        /// </summary>
        /// <param name="Strength"></param>
        public ExpandOnPointBehavior SetLocationAttraction(Vector2[] Location, float[] Radius, float[] Strength)
        {
            if((Location.Length != Radius.Length) || (Strength.Length != Radius.Length))
            {
                throw new ArgumentException("Length of each array is not equal");
            }
            locationAttractions = new LocationAttraction[Location.Length];
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
    /// Trackers significanly slow down the line network.
    /// Needs to rebuild the entire network to turn on again.
    /// May also slow down merging two line networks together if the disabled tracker on the merging line is enabled on the merger line.
    /// Parameters relying on these parameters will obviously stop working when turned off. (it wont throw an ArgumentException tho)
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


