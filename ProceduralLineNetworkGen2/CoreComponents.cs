using GarageGoose.ProceduralLineNetwork.Component.Interface;

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

        public bool ThreadSafeOperation() { return true; }
        public bool ThreadSafeAccess() { return true; }

        ElementsDatabase DB;
        public void InheritDatabase(ElementsDatabase DB) { this.DB = DB; }

        public void OnLineAdded(uint Key)
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
        public void OnLineModificationBefore(uint Key)
        {
            //Save the old point key as it will be used to compare the updated point key if its the same.
            OldPoint1Key = DB.Lines[Key].PointKey1;
            OldPoint2Key = DB.Lines[Key].PointKey2;
        }
        public void OnLineModificationAfter(uint Key)
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

        public void OnLineRemoved(uint Key)
        {
            //Remove current line on LinesOnPoint at the line's point 1
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey2, new());
            LinesOnPoint[DB.Lines[Key].PointKey2].Remove(Key);

            //Remove current line on LinesOnPoint at the line's point 2
            LinesOnPoint.TryAdd(DB.Lines[Key].PointKey1, new());
            LinesOnPoint[DB.Lines[Key].PointKey1].Remove(Key);
        }
    }
}
