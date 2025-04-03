using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Tracks the connected lines on a point
    /// </summary>
    class TrackConnectedLinesOnPoint : ILineNetwork, ILineNetworkTracker
    {
        /// <summary>
        /// Point key on the Dictionary key and line key on the Hashset.
        /// </summary>
        public readonly Dictionary<uint, HashSet<uint>> LinesOnPoint = new();

        int SearchMin = 0;
        int SearchMax = 0;
        public void SetSearchRange(int MinOrSpecific, int Max = -1)
        {
            SearchMin = MinOrSpecific;
            SearchMax = Max;
        }

        HashSet<uint> ILineNetworkTracker.Search()
        {
            return new();
        }

        bool ILineNetwork.ThreadSafeOperation() { return true; }
        bool ILineNetworkTracker.ThreadSafeAccess() { return true; }

        ElementsDatabase DB = new(new());
        void ILineNetwork.InheritDatabase(ElementsDatabase DB) { this.DB = DB; }

        void ILineNetworkTracker.OnLineAddition(uint Key)
        {
            //Add the new line on LinesOnPoint at the line's point 1
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey2, new());
            LinesOnPoint[DB.Lines[Key].PointKey2].Add(Key);

            //Add the new line on LinesOnPoint at the line's point 2
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey1, new());
            LinesOnPoint[DB.Lines[Key].PointKey1].Add(Key);
        }

        private uint OldPoint1Key;
        private uint OldPoint2Key;
        void ILineNetworkTracker.OnLineModificationBefore(uint Key)
        {
            //Save the old point key as it will be used to compare the updated point key if its the same.
            OldPoint1Key = DB.Lines[Key].PointKey1;
            OldPoint2Key = DB.Lines[Key].PointKey2;
        }
        void ILineNetworkTracker.OnLineModificationAfter(uint Key)
        {
            //Check if the PointKey1 changed on the line and if so, update the change at LinesAtPoint
            if(OldPoint1Key != DB.Lines[Key].PointKey1)
            {
                LinesOnPoint[OldPoint1Key].Remove(Key);

                LinesOnPoint.TryAdd(DB.Lines[Key].PointKey1, new());
                LinesOnPoint[DB.Lines[Key].PointKey1].Add(Key);
            }

            //Check if the PointKey1 changed on the line and if so, update the change at LinesAtPoint
            if (OldPoint2Key != DB.Lines[Key].PointKey2)
            {
                LinesOnPoint[OldPoint2Key].Remove(Key);

                LinesOnPoint.TryAdd(DB.Lines[Key].PointKey2, new());
                LinesOnPoint[DB.Lines[Key].PointKey2].Add(Key);
            }
        }

        void ILineNetworkTracker.OnLineRemoval(uint Key)
        {
            //Remove current line on LinesOnPoint at the line's point 1
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey2, new());
            LinesOnPoint[DB.Lines[Key].PointKey2].Remove(Key);

            //Remove current line on LinesOnPoint at the line's point 2
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey1, new());
            LinesOnPoint[DB.Lines[Key].PointKey1].Remove(Key);
        }
    }
    class TrackPointPositionQuadTree : ILineNetwork, ILineNetworkTracker
    {
        
    }
    class TrackPointAngles : ILineNetwork, ILineNetworkTracker
    {
        Dictionary<uint, SortedList<float, uint>> AngleOfALine;
        Dictionary<uint, SortedList<float, uint>> AngleBetweenLines;

        SortedDictionary<uint, float> MaxAngles;
        SortedDictionary<uint, float> MinAngles;
        public TrackPointAngles(bool TrackMaxAngle, bool TrackMinAngle)
        {

        }
    }
    class TrackModificationChanges
    {
        public List<uint, Point?> Before = new(); 
    }
    class TrackAndModifyCustomIdentifier : ILineNetwork, ILineNetworkTracker, ILineNetworkModification
    {

    }
    class ModifierAddLinesOnPoint
    {
        public interface IAddLinesOnPoint
        {

        }
    }
    class ModifierSubdivideLines
    {

    }
    class ModifierMergePointsInCloseProximity
    {

    }
    class ModifierAddPointsOnLineIntersection
    {

    }
}
