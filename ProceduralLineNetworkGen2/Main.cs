using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using GarageGoose.ProceduralLineNetwork.Elements;

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
            if (DT.MaxAngleAtPoint)    DB.MaxAngularDistanceAtPoint = null;
            if (DT.MinAngleAtPoint)    DB.MinAngularDistanceAtPoint = null;
            if (DT.LineCountAtPoint)   DB.LineCountAtPoint = null;
            if (DT.PointID)            DB.PointID = null;
            if (DT.PointAdditionOrder) DB.PointKeysList = null;
        }

        /// <summary>
        /// Find eligible point keys based on the parameters
        /// More efficient to find many point keys at once due to how the finding algorithm work (see NetworkManager.cs)
        /// </summary>
        /// <param name="Limit">Limits the size of the array</param>
        /// <param name="Params">Set of rules for finding points</param>
        /// <param name="Multithread">Performs multithreading, beneficial if many parameters are in use</param>
        public HashSet<uint> FindPointKeys(FindPointsParams Params, bool Multithread = false)
        {
            ConcurrentBag<HashSet<uint>> PointKeysFromParams = new();

            if(Params.MaxLinesAtPoint != null || Params.MinLinesAtPoint != null)
            {
                //Set to upper and lower bounds if null
                int Max = Params.MaxLinesAtPoint ?? 100000;
                int Min = Params.MinLinesAtPoint ?? 0;
                if (Multithread) Parallel.Invoke(() => PointKeysFromParams.Add(DBHandle.GetAllLineCountAtPointInRange(Max, Min)));
                else PointKeysFromParams.Add(DBHandle.GetAllLineCountAtPointInRange(Max, Min));
            }

            if((Params.PointsAddedAfterIndex != null || Params.PointsAddedBeforeIndex != null) && DB.LineCountAtPoint != null)
            {
                //Set to upper and lower bounds if null
                int Max = Params.PointsAddedBeforeIndex ?? DB.LineCountAtPoint.Count;
                int Min = Params.MinLinesAtPoint ?? 0;
                if (Multithread) Parallel.Invoke(() => PointKeysFromParams.Add(DBHandle.GetPointsInOrderOfAdditionRange(Min, Max)));
                else PointKeysFromParams.Add(DBHandle.GetPointsInOrderOfAdditionRange(Min, Max));
            }

            void PointAngleLimit()
            {
                //Set to upper and lower bounds if null
                //float Max = M
            }

            HashSet<uint>[] SortedPointKeysFromParams = PointKeysFromParams.ToArray();
            Array.Sort(SortedPointKeysFromParams);
            if (PointKeysFromParams.Count == 0) return new();
            if (PointKeysFromParams.Count == 1) return PointKeysFromParams[0];

            //After finding eligible points per parameters, the list is sorted from params with the least eligible keys on it to the most.
            //Then intersect each hashsets with least to the most uints.
            //Sorting improves performance because most of the ineligible points is eliminated already when intersecting from smallest to largest. 
            PointKeysFromParams.Sort();
            HashSet<uint> PointKeysFromAllParams = PointKeysFromParams[0];
            for(int i = 1; i < PointKeysFromParams.Count; i++)
            {
                PointKeysFromAllParams.Intersect(PointKeysFromParams[i]);
            }
            return PointKeysFromAllParams;
        }

        /// <summary>
        /// Expand the line network based on the behavior
        /// </summary>
        /// <param name="Behavior">Set of rules for the behavior</param>
        /// <param name="Multithread">Performs multithreading, will increase performance if there are many points in HashSet or Array</param>
        public void ExpandNetwork(HashSet<uint> PointKeys, ExpandOnPointBehavior Behavior, bool Multithread = false)
        {

        }

        /// <summary>
        /// Manually add a point to the line network.
        /// </summary>
        /// <param name="Key">Used to identify this specific point internally</param>
        /// <param name="ID">Used to identify this specific point (or a group of points) via custom IDs (good for categorizing points, etc.)</param>
        public void AddPoint(Vector2 Location, out uint Key, string[]? ID = null)
        {
            Key = DB.NewUniqueElementKey();
            Elements.Point Point = new(Location, ID);
            DB.Points.Add(Key, Point);
        }

        /// <summary>
        /// Manually add a line to the line network.
        /// </summary>
        /// <param name="Point1Key">First  point the line connects to</param>
        /// <param name="Point2Key">Second point the line connects to</param>
        /// <param name="Key">Used to identify this specific line internally</param>
        public void AddLine(uint Point1Key, uint Point2Key, out uint Key)
        {
            Key = DB.NewUniqueElementKey();
            Line Line = new(Point1Key, Point2Key);
        }

        /// <summary>
        /// Manually delete a point, all connected lines going to it will also be deleted.
        /// </summary>
        public void DeletePoint(uint PointKey)
        {
            DBHandle.DeletePoint(PointKey);
        }

        /// <summary>
        /// Manually delete a line.
        /// </summary>
        public void DeleteLine(uint LineKey)
        {
            DBHandle.DeleteLine(LineKey);
        }

        /// <summary>
        /// Identifies element from key
        /// </summary>
        public string IdentifyElement(uint Key)
        {
            if(DB.Points.ContainsKey(Key))
            {
                return "Point";
            }
            if (DB.Lines.ContainsKey(Key))
            {
                return "Line";
            }
            return "Null";
        }

        /// <summary>
        /// Get a point object reference using Key
        /// </summary>
        public Point GetPoint(uint Key)
        {
            if (DB.Points.ContainsKey(Key))
            {
                return DB.Points[Key];
            }
            return new(Vector2.Zero);
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


