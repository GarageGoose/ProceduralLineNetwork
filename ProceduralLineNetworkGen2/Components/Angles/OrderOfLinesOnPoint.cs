using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Tracks the order of lines arranged from 0 to 2pi radians or 360 degrees in a point.
    /// </summary>
    public class ObserveOrderOfLinesOnPoint : LineNetworkObserver
    {
        readonly ObserveLineAngles lineAngle;
        readonly ElementStorage database;

        //Key:   Point key
        //Value: Ordeed list of lines from a point
        private readonly Dictionary<uint, List<uint>> OrderedLinesOnPoint = new();

        //Key:   Point key
        //Value:
        //       Key:   Line key
        //       Value: Next line to the current line by angle
        private readonly Dictionary<uint, Dictionary<uint, uint>> internalNextLineToThis = new();

        //Key:   Point key
        //Value: 
        //       Key:   Line key
        //       Value: Last line to the current line by angle
        private readonly Dictionary<uint, Dictionary<uint, uint>> internalLastLineToThis = new();

        public ObserveOrderOfLinesOnPoint(ObserveLineAngles lineAngle, ElementStorage database) : 
        base(1, true, [ObserverEvent.PointModified, ObserverEvent.LineAdded, ObserverEvent.LineModified, ObserverEvent.LineRemoved, ObserverEvent.LineClear])
        {
            this.lineAngle = lineAngle;
            this.database = database;

            foreach (uint lineKey in database.lines.Keys)
            {
                LineAdded(lineKey, database.lines[lineKey]);
            }
        }

        /// <summary>
        /// Organised list of lines in a point from 0 to 2pi
        /// </summary>
        /// <param name="pointKey">Key of the target point.</param>
        /// <returns>Read only list of lines in a point from 0 to 2pi</returns>
        public IReadOnlyList<uint> GetOrderOfLinesOnPoint(uint pointKey) => OrderedLinesOnPoint[pointKey];

        /// <summary>
        /// Dictionary that point to the key of the next line by angle in key. Key is the current line then value is the next line.
        /// </summary>
        /// <param name="pointKey">Key of the target point.</param>
        /// <returns>Readonly dictionary that point to the key of the next line by angle in key. Key is the current line then value is the next line.</returns>
        public IReadOnlyDictionary<uint, uint> NextLineOfALineOnAPointDict(uint pointKey) => internalNextLineToThis[pointKey];

        /// <summary>
        /// Dictionary that point to the key of the last line by angle in key. Key is the current line then value is the next line.
        /// </summary>
        /// <param name="pointKey">Key of the target point.</param>
        /// <returns>Readonly dictionary that point to the key of the last line by angle in key. Key is the current line then value is the next line.</returns>
        public IReadOnlyDictionary<uint, uint> LastLineOfALineOnAPointDict(uint pointKey) => internalLastLineToThis[pointKey];

        /// <summary>
        /// Point to the next line of the current line (by its angle) in a point by its key.
        /// </summary>
        /// <param name="pointKey">Target point.</param>
        /// <param name="lineKey">Target line.</param>
        /// <returns>Key of the next line.</returns>
        public uint NextLineOfALine(uint pointKey, uint lineKey) => internalNextLineToThis[pointKey][lineKey];

        /// <summary>
        /// Point to the line before the current line (by its angle) in a point by its key.
        /// </summary>
        /// <param name="pointKey">Target point.</param>
        /// <param name="lineKey">Target line.</param>
        /// <returns>Key of the line before the line.</returns>
        public uint LastLineOfALine(uint pointKey, uint lineKey) => internalLastLineToThis[pointKey][lineKey];


        //When the point is moved
        protected override void PointModified(uint key, Point before, Point after)
        {
            if (!OrderedLinesOnPoint.ContainsKey(key)) { return; }
            OrderedLinesOnPoint[key].Clear();
            foreach (uint lineKey in database.linesOnPoint.linesOnPoint[key])
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
                return;
            }
            float angleBeforeCurrAngle = 0;
            float currAngle = GetLineAngleUnsafe(lineKeyToInsert, pointKey);
            float angleAfterCurrAngle;

            for (int i = 0; i < listOfLineKeys.Count; i++)
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

            if (OrderedLinesOnPoint[pointKey].Count == 0) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.Only); }
            else if (indexOfLine == OrderedLinesOnPoint[pointKey].Count - 1) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.Last); }
            else if (indexOfLine == 0) { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.First); }
            else { UpdateDictElementRemoved(pointKey, lineKey, indexOfLine, LineListPos.InBetween); }

            if (OrderedLinesOnPoint[pointKey].Count == 0)
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
                    internalLastLineToThis.TryAdd(pointKey, new());
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
                    internalLastLineToThis.TryAdd(pointKey, new());
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
                    internalLastLineToThis.TryAdd(pointKey, new());
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
            if (database.lines[lineKey].PointKey1 == pointKey) { angle = lineAngle.fromPoint1[lineKey]; return true; }
            else if (database.lines[lineKey].PointKey2 == pointKey) { angle = lineAngle.fromPoint2[lineKey]; return true; }
            else angle = null;
            return false;
        }

        private enum LineListPos
        {
            First, InBetween, Last, Only
        }
    }
}
