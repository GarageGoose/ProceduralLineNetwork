using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public interface ILineAngleTracker
    {
        IReadOnlyDictionary<uint, float> fromPoint1 { get; }
        IReadOnlyDictionary<uint, float> fromPoint2 { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrackLineAngles : LineNetworkObserver, ILineAngleTracker
    {
        public bool ThreadSafeDataAccess { get; } = true;

        //Access datas of lines and point to track
        private ElementsDatabase database;

        //Separated the internal and public dictionary to avoid unwanted changes.
        private readonly Dictionary<uint, float> internalLineAngleFromPoint1 = new();
        private readonly Dictionary<uint, float> internalLineAngleFromPoint2 = new();

        /// <summary>
        /// Check line angle from the perspective of the first connected point.
        /// Key is line key and float is angle in radian.
        /// </summary>
        public IReadOnlyDictionary<uint, float> fromPoint1 { get; }

        /// <summary>
        /// Check line angle from the perspective of the second connected point.
        /// Key is line key and float is angle in radian.
        /// </summary>
        public IReadOnlyDictionary<uint, float> fromPoint2 { get; }

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates()
        {
            return [ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnPointModification];
        }

        public TrackLineAngles(ElementsDatabase database) : base(0, true)
        {
            this.database = database;
            fromPoint1 = internalLineAngleFromPoint1;
            fromPoint2 = internalLineAngleFromPoint2;
        }

        protected override void PointModified(uint key, Point before, Point after)
        {
            foreach(uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                float Point1Angle = CalcAngle(database.LinePoint1(lineKey), database.LinePoint2(lineKey));
                internalLineAngleFromPoint1[lineKey] = Point1Angle;

                //Invert the angle from Point1Angle to avoid expensive computation
                internalLineAngleFromPoint2[lineKey] = InvertAngle(Point1Angle);
            }
        }

        protected override void LineAdded(uint key, Line newLine)
        {
            float Point1Angle = CalcAngle(database.LinePoint1(key), database.LinePoint2(key));
            internalLineAngleFromPoint1.Add(key, Point1Angle);

            //Invert the angle from Point1Angle to avoid expensive computation
            internalLineAngleFromPoint2.Add(key, InvertAngle(Point1Angle));
        }

        protected override void LineModified(uint key, Line before, Line after)
        {
            if (before.PointKey1 != after.PointKey1 || before.PointKey2 != after.PointKey2)
            {
                //Store the angle from the perspective of point 1 to save performance by just inverting this angle for point 2 (as oppose to recalculating it) just incase both end on the line is changed.
                float angleFromPoint1New = CalcAngle(database.LinePoint1(key), database.LinePoint2(key));
                internalLineAngleFromPoint1[key] = (float)angleFromPoint1New;

                //Invert the angle to avoid expensive computation
                internalLineAngleFromPoint2[key] = InvertAngle((float)angleFromPoint1New);
            }
        }

        protected override void LineRemoved(uint Key, Line oldPoint)
        {
            internalLineAngleFromPoint1.Remove(Key);
            internalLineAngleFromPoint2.Remove(Key);
        }

        protected override void LineClear()
        {
            internalLineAngleFromPoint1.Clear();
            internalLineAngleFromPoint2.Clear();
        }

        //Refence: https://stackoverflow.com/questions/2676719/calculating-the-angle-between-a-line-and-the-x-axis
        private float CalcAngle(Point one, Point two)
        {
            float diffX = two.x - one.x;
            float diffY = two.y - one.y;
            return MathF.Atan2(diffY, diffX);
        }

        //Subtract pi if the angle is >pi else add pi to keep the angle from surpassing <0 and >2pi.
        private float InvertAngle(float Angle) => (Angle >= MathF.PI) ? Angle - MathF.PI : Angle + MathF.PI;
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrackOrderOfLinesOnPoint : LineNetworkObserver
    {
        readonly TrackLineAngles lineAngle;
        readonly ElementsDatabase database;

        //Dict key: point key.
        //SortedList key: line angle from the perspective of the current point.
        //SortedList value: line key.
        private readonly Dictionary<uint, List<uint>> OrderedLinesOnPoint = new();

        private readonly Dictionary<uint, Dictionary<uint, uint>> internalNextLineToThis = new();
        private readonly Dictionary<uint, Dictionary<uint, uint>> internalLastLineToThis = new();

        public TrackOrderOfLinesOnPoint(TrackLineAngles lineAngle, ElementsDatabase database) : base(1, true)
        {
            this.lineAngle = lineAngle;
            this.database = database;
        }

        public IReadOnlyList<uint> GetOrderOfLinesOnPoint(uint pointKey) => OrderedLinesOnPoint[pointKey];
        public IReadOnlyDictionary<uint, uint> NextLineOfALineOnAPointDict(uint pointKey) => internalNextLineToThis[pointKey];
        public uint NextLineOfALine(uint pointKey, uint lineKey) => internalNextLineToThis[pointKey][lineKey];
        public IReadOnlyDictionary<uint, uint> LastLineOfALineOnAPointDict(uint pointKey) => internalLastLineToThis[pointKey];
        public uint LastLineOfALine(uint pointKey, uint lineKey) => internalLastLineToThis[pointKey][lineKey];

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() =>
            [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        //When the point is moved
        protected override void PointModified(uint key, Point before, Point after)
        {
            if(!OrderedLinesOnPoint.ContainsKey(key)) { return; }
            OrderedLinesOnPoint[key].Clear();
            foreach(uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                InsertLine(key, lineKey);
            }
        }
        protected override void LineAdded(uint key, Line line)
        {
            InsertLine(line.PointKey1, key);
            InsertLine(line.PointKey2, key);
        }

        //When the line changed its connected point(s).
        protected override void LineModified(uint key, Line before, Line after)
        {
            if (before.PointKey1 != after.PointKey1 || before.PointKey2 != after.PointKey2)
            {
                DeleteLine(before.PointKey1, key);
                InsertLine(after.PointKey1, key);

                DeleteLine(before.PointKey2, key);
                InsertLine(after.PointKey2, key);
            }
        }
        protected override void LineRemoved(uint key, Line line)
        {
            DeleteLine(line.PointKey1, key);
            DeleteLine(line.PointKey2, key);
        }
        protected override void LineClear()
        {
            OrderedLinesOnPoint.Clear();
            internalLastLineToThis.Clear();
            internalNextLineToThis.Clear();
        }

        //Custom algorithm instead of sort() is used to go from O(n log n) to O(n) (at worst case scenario!)
        private void InsertLine(uint pointKey, uint lineKeyToInsert)
        {
            OrderedLinesOnPoint.TryAdd(pointKey, new());
            List<uint> listOfLineKeys = OrderedLinesOnPoint[pointKey];
            if (listOfLineKeys.Count == 0)
            {
                listOfLineKeys.Add(lineKeyToInsert);
                UpdateDictElementAdded(pointKey, lineKeyToInsert, 0, LineListPos.Only);
            }
            float angleBeforeCurrAngle = 0;
            float currAngle = GetLineAngleUnsafe(lineKeyToInsert, pointKey);
            float angleAfterCurrAngle;

            for(int i = 0; i < listOfLineKeys.Count; i++)
            {
                angleAfterCurrAngle = GetLineAngleUnsafe(listOfLineKeys[i], pointKey);

                //Check if currAngle is between angleAfterCurrAngle and angleBeforeCurrAngle
                if (currAngle < angleAfterCurrAngle && currAngle >= angleBeforeCurrAngle)
                {
                    listOfLineKeys.Insert(i, lineKeyToInsert);
                    UpdateDictElementAdded(pointKey, lineKeyToInsert, i, (i == 0) ? LineListPos.First : LineListPos.InBetween);
                    return;
                }

                angleBeforeCurrAngle = angleAfterCurrAngle;
            }

            listOfLineKeys.Add(lineKeyToInsert);
            UpdateDictElementAdded(pointKey, lineKeyToInsert, listOfLineKeys.Count - 1, LineListPos.Last);
        }
        private void DeleteLine(uint pointKey, uint lineKey)
        {
            int indexOfLine = 0;
            while (indexOfLine < OrderedLinesOnPoint[pointKey].Count && OrderedLinesOnPoint[pointKey][indexOfLine] == lineKey) { indexOfLine++; } 

            OrderedLinesOnPoint[pointKey].RemoveAt(indexOfLine);

            if(OrderedLinesOnPoint[pointKey].Count == 0) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.Only); }
            else if(indexOfLine == OrderedLinesOnPoint[pointKey].Count - 1) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.Last); }
            else if(indexOfLine == 0) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.First); }
            else { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.InBetween); }

            if( OrderedLinesOnPoint[pointKey].Count == 0)
            {
                OrderedLinesOnPoint[pointKey].Clear();
            }
        }

        private void UpdateDictElementAdded(uint pointKey, uint lineKey, int index, LineListPos listPos)
        {
            if (listPos == LineListPos.Only)
            {
                internalNextLineToThis.TryAdd(pointKey, new());
                internalNextLineToThis[pointKey].Add(lineKey, lineKey);

                internalLastLineToThis.TryAdd(pointKey, new());
                internalLastLineToThis[pointKey].Add(lineKey, lineKey);
                return;
            }

            List<uint> lineKeys = OrderedLinesOnPoint[pointKey];

            switch (listPos)
            {
                case LineListPos.First:
                    //point the current line to the next line (1 because it is first)
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalNextLineToThis[pointKey].Add(lineKey, lineKeys[1]);

                    //point the pointer of the line before the current line to the current line.
                    internalNextLineToThis[pointKey][lineKeys[lineKeys.Count - 1]] = lineKey;

                    //Point the current line to the line before it.
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalLastLineToThis[pointKey].Add(lineKey, lineKeys[lineKeys.Count - 1]);

                    //Point the first line to the last line since it is before it
                    internalLastLineToThis[pointKey][lineKeys[lineKeys.Count - 1]] = lineKey;
                    break;

                case LineListPos.InBetween:
                    //Add the new line to the dictionary with the pointer pointing to the line next to this.
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalNextLineToThis[pointKey].Add(lineKey, lineKeys[index + 1]);

                    //point the pointer of the line before the current line to the current line.
                    internalNextLineToThis[pointKey][lineKeys[index - 1]] = lineKey;

                    //Add the new line to the dictionary with the pointer pointing to the line next to this.
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalLastLineToThis[pointKey].Add(lineKey, lineKeys[index + 1]);

                    //point the pointer of the line after the current line to the current line.
                    internalLastLineToThis[pointKey][lineKeys[index + 1]] = lineKey;
                    break;

                case LineListPos.Last:
                    //Since the current line is the last line, point to the first line since it is next to it.
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalNextLineToThis[pointKey].Add(lineKey, lineKeys[0]);

                    //point the pointer of the line before the current line to the current line.
                    internalNextLineToThis[pointKey][lineKeys[lineKeys.Count - 2]] = lineKey;

                    //Point the current line to the line before it.
                    internalNextLineToThis.TryAdd(pointKey, new());
                    internalLastLineToThis[pointKey].Add(lineKey, lineKeys[lineKeys.Count - 2]);

                    //Point the first line to the last line since it is before it
                    internalLastLineToThis[pointKey][lineKeys[0]] = lineKey;
                    break;
            }
        }

        private void UpdateDictElementRemoved(uint pointKey, uint lineKey, int index, LineListPos listPos)
        {
            internalNextLineToThis.Remove(lineKey);
            if (internalNextLineToThis[pointKey].Count == 0) { internalNextLineToThis.Remove(pointKey); }
            internalLastLineToThis.Remove(lineKey);
            if (internalLastLineToThis[pointKey].Count == 0) { internalLastLineToThis.Remove(pointKey); }

            if (listPos == LineListPos.Only) { return; }

            List<uint> lineKeys = OrderedLinesOnPoint[pointKey];

            switch (listPos)
            {
                case LineListPos.First:
                    //point the last line to the new first line
                    internalNextLineToThis[pointKey][lineKeys[lineKeys.Count - 1]] = lineKeys[0];

                    //point the new first line to the last line
                    internalLastLineToThis[pointKey][lineKeys[0]] = lineKeys[lineKeys.Count - 1];
                    break;

                case LineListPos.InBetween:
                    internalNextLineToThis[pointKey][lineKeys[index - 1]] = lineKeys[index];
                    internalLastLineToThis[pointKey][lineKeys[index + 1]] = lineKeys[index];
                    break;

                case LineListPos.Last:
                    //point the line before the new last line to the last line
                    internalNextLineToThis[pointKey][lineKeys[lineKeys.Count - 2]] = lineKeys[lineKeys.Count - 1];

                    //point the first line to the new last line
                    internalLastLineToThis[pointKey][lineKeys[0]] = lineKeys[lineKeys.Count - 1];
                    break;
            }
        }

        private float GetLineAngleUnsafe(uint lineKey, uint pointKey) => (database.lines[lineKey].PointKey1 == pointKey) ? lineAngle.fromPoint1[lineKey] : lineAngle.fromPoint2[lineKey];
        private bool GetLineAngleSafe(uint lineKey, uint pointKey, out float? angle)
        {
            if (database.lines[lineKey].PointKey1 == pointKey)      { angle = lineAngle.fromPoint1[lineKey]; return true; }
            else if (database.lines[lineKey].PointKey2 == pointKey) { angle = lineAngle.fromPoint2[lineKey]; return true; }
            else angle = null;
            return false;
        }

        private enum LineListPos
        {
            First, InBetween, Last, Only
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrackAngleBetweenLines : LineNetworkObserver, ILineAngleTracker
    {
        private readonly TrackLineAngles lineAngles;
        private readonly TrackOrderOfLinesOnPoint orderOfLines;
        private readonly ElementsDatabase database;

        //Separated the internal and public dictionary to avoid unwanted changes.
        private readonly Dictionary<uint, float> internalAngleFromPoint1;
        private readonly Dictionary<uint, float> internalAngleFromPoint2;

        /// <summary>
        /// Get the angle between the angle of this line from the perspective of point 1 and
        /// the angle of the line next to it from the perspective of point 1.
        /// </summary>
        public IReadOnlyDictionary<uint, float> fromPoint1 { get; }

        /// <summary>
        /// Get the angle between the angle of this line from the perspective of point 2 and
        /// the angle of the line next to it from the perspective of point 2.
        /// </summary>
        public IReadOnlyDictionary<uint, float> fromPoint2 { get; }

        public TrackAngleBetweenLines(TrackLineAngles lineAngles, ElementsDatabase database, TrackOrderOfLinesOnPoint orderOfLines) : base(2, true)
        {
            this.lineAngles = lineAngles;
            this.database = database;
            this.orderOfLines = orderOfLines;

            internalAngleFromPoint1 = new();
            internalAngleFromPoint2 = new();

            fromPoint1 = internalAngleFromPoint1;
            fromPoint2 = internalAngleFromPoint2;
        }

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() =>
            [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        protected override void PointModified(uint key, Point before, Point after)
        {
            foreach(uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                internalAngleFromPoint1[lineKey] = CalcAngleBetweenLines(database.lines[lineKey].PointKey1, lineKey, database.lines[lineKey]);
                internalAngleFromPoint2[lineKey] = CalcAngleBetweenLines(database.lines[lineKey].PointKey2, lineKey, database.lines[lineKey]);
            }
        }
        protected override void LineAdded(uint key, Line line)
        {
            internalAngleFromPoint1.Add(key, CalcAngleBetweenLines(line.PointKey1, key, line));
            internalAngleFromPoint2.Add(key, CalcAngleBetweenLines(line.PointKey2, key, line));
        }
        protected override void LineModified(uint key, Line before, Line after)
        {
            if (before.PointKey1 != after.PointKey1 || before.PointKey2 != after.PointKey2)
            {
                internalAngleFromPoint1[key] = CalcAngleBetweenLines(after.PointKey1, key, after);
                internalAngleFromPoint2[key] = CalcAngleBetweenLines(after.PointKey2, key, after);
            }
        }
        protected override void LineRemoved(uint key, Line line)
        {
            internalAngleFromPoint1.Remove(key);
            internalAngleFromPoint2.Remove(key);
        }
        protected override void LineClear()
        {
            internalAngleFromPoint1.Clear();
            internalAngleFromPoint2.Clear();
        }

        private float CalcAngleBetweenLines(uint pointKey, uint lineKey, Line line)
        {
            float thisLineAngle = (line.PointKey1 == pointKey) ? lineAngles.fromPoint1[pointKey] : lineAngles.fromPoint2[pointKey];
            uint getNextLineKey = orderOfLines.NextLineOfALine(pointKey, lineKey);
            float nextLineAngle = (database.lines[getNextLineKey].PointKey1 == pointKey) ? lineAngles.fromPoint1[getNextLineKey] : lineAngles.fromPoint2[getNextLineKey];

            //Check if the current line is the only line in the point, if so, return 2Pi to avoid calculation.
            if (database.linesOnPoint.linesOnPoint[pointKey].Count == 1) { return 2 * MathF.PI; }

            //Check if the current line is the last line to calculate the angle differently
            //Example scenario:
            //Last line (current line) = 2Pi rad
            //First line (next line) = 0 rad
            //doing 0 - 2Pi would obviously result in wrong calculation
            //Solution: add 2Pi to the next line so that 2Pi - 2Pi = 0, yay!
            if (thisLineAngle > nextLineAngle) { return nextLineAngle + (2 * MathF.PI) - thisLineAngle; }

            return nextLineAngle - thisLineAngle;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SortedAnglesInLineNetwork : LineNetworkObserver
    {
        private readonly ILineAngleTracker angleTracker;
        private readonly ElementsDatabase database;

        private SortedAngleSet<Tuple<uint, LineAtPoint>> lineAngles = new();

        public readonly IReadOnlySet<float> angles;
        public readonly IReadOnlyDictionary<float, Tuple<uint, LineAtPoint>> angleToPointKey;

        public SortedAnglesInLineNetwork(ILineAngleTracker angleTracker, ElementsDatabase database) : base(0, true)
        {
            this.angleTracker = angleTracker;
            this.database = database;

            angles = lineAngles.angles;
            angleToPointKey = lineAngles.angleToKey;
        }

        public IReadOnlySet<float> GetViewBetween(float min, float max) => lineAngles.GetViewBetween(min, max);

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() =>
              [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        protected override void PointModified(uint key, Point before, Point after)
        {
            foreach(uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                lineAngles.Modify(new(lineKey, LineAtPoint.Point1), angleTracker.fromPoint1[lineKey]);
                lineAngles.Modify(new(lineKey, LineAtPoint.Point1), angleTracker.fromPoint1[lineKey]);
            }
        }

        protected override void LineAdded(uint key, Line line)
        {
            lineAngles.Add(angleTracker.fromPoint1[key], new(key, LineAtPoint.Point1));
            lineAngles.Add(angleTracker.fromPoint2[key], new(key, LineAtPoint.Point2));
        }

        protected override void LineModified(uint key, Line before, Line after)
        {
            lineAngles.Modify(new(key, LineAtPoint.Point1), angleTracker.fromPoint1[key]);
            lineAngles.Modify(new(key, LineAtPoint.Point2), angleTracker.fromPoint2[key]);
        }

        protected override void LineRemoved(uint key, Line line)
        {
            lineAngles.Remove(new(key, LineAtPoint.Point1));
            lineAngles.Remove(new(key, LineAtPoint.Point2));
        }

        protected override void LineClear()
        {
            lineAngles = new();
        }
    }

    /// <summary>
    /// Custom data structure for SortedAnglesInLineNetwork that is mainly backed by a SortedSet of angles that allows dupelicate
    /// internally via nudging the values microscopically and that also act as a key for its associated line.
    /// </summary>
    public class SortedAngleSet<TKey> where TKey : notnull
    {
        public readonly SortedSet<float> internalAngles = new();
        public readonly Dictionary<float, TKey> internalAngleToKey = new();
        public readonly Dictionary<TKey, float> internalKeyToAngle = new();

        public IReadOnlySet<float> angles;
        public IReadOnlyDictionary<float, TKey> angleToKey;

        public SortedAngleSet()
        {
            angles = internalAngles;
            angleToKey = internalAngleToKey;
        }

        public IReadOnlySet<float> GetViewBetween(float min, float max) => internalAngles.GetViewBetween(min, max);

        /// <summary>
        /// Add a new angle with its corresponding line key. Note that the angle might differ slightly
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="lineKey"></param>
        public void Add(float angle, TKey lineKey)
        {
            if (!internalAngles.Add(angle))
            {
                //Nudge the angle very slightly so that it would get added to the sortedlist while having microscopic difference.
                float nudgeAngleAmount = (angle >= MathF.PI) ? 0.000001f : -0.000001f;
                float nudgeAngle = angle + nudgeAngleAmount;
                while (!internalAngles.Add(nudgeAngle))
                {
                    nudgeAngle += nudgeAngleAmount;
                }
                internalAngleToKey.Add(nudgeAngle, lineKey);
                internalKeyToAngle.Add(lineKey, nudgeAngle);
                return;
            }
            internalAngleToKey.Add(angle, lineKey);
            internalKeyToAngle.Add(lineKey, angle);
        }
        public void Modify(TKey lineKey, float newAngle)
        {
            float oldAngle = internalKeyToAngle[lineKey];
            internalAngles.Remove(oldAngle);
            internalAngleToKey.Remove(oldAngle);

            if (!internalAngles.Add(newAngle))
            {
                //Nudge the angle very slightly so that it would get added to the sortedlist while having microscopic difference.
                float nudgeAngleAmount = (newAngle >= MathF.PI) ? 0.000001f : -0.000001f;
                float nudgeAngle = newAngle + nudgeAngleAmount;
                while (!internalAngles.Add(nudgeAngle))
                {
                    nudgeAngle += nudgeAngleAmount;
                }
                internalKeyToAngle[lineKey] = nudgeAngle;
                internalAngleToKey.Add(nudgeAngle, lineKey);
                return;
            }
            internalKeyToAngle[lineKey] = newAngle;
            internalAngleToKey.Add(newAngle, lineKey);
        }
        public void Remove(TKey lineKey)
        {
            float angle = internalKeyToAngle[lineKey];
            internalKeyToAngle.Remove(lineKey);
            internalAngleToKey.Remove(angle);
            internalAngles.Remove(angle);
        }
    }
}
