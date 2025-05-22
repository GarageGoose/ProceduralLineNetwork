using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Captures data for angles between lines (or spacing between lines) at a point.
    /// </summary>
    public class ObserveAngleBetweenLines : ILineNetObserver, ILineAngleTracker
    {
        private readonly ObserveLineAngles lineAngles;
        private readonly ObserveOrderOfLinesOnPoint orderOfLines;
        private readonly ElementStorage database;

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

        public ObserverEvent[] eventSubscription => [ObserverEvent.PointModified, ObserverEvent.LineAdded, ObserverEvent.LineModified, ObserverEvent.LineRemoved, ObserverEvent.LineClear];
        public uint UpdateLevel => 2;
        public bool Multithread => true;

        /// <summary>
        /// Captures data for angles between lines (or spacing between lines) at a point.
        /// </summary>
        /// <param name="lineAngles">Oberver for line angles</param>
        /// <param name="storage">Storage of lines and points</param>
        /// <param name="orderOfLines">Observer for tracking the order of lines in a point</param>
        public ObserveAngleBetweenLines(ObserveLineAngles lineAngles, ElementStorage storage, ObserveOrderOfLinesOnPoint orderOfLines)
        {
            this.lineAngles = lineAngles;
            this.database = storage;
            this.orderOfLines = orderOfLines;

            internalAngleFromPoint1 = new();
            internalAngleFromPoint2 = new();

            fromPoint1 = internalAngleFromPoint1;
            fromPoint2 = internalAngleFromPoint2;

            //Add already existing lines
            foreach (uint lineKey in storage.lines.Keys) ((ILineNetObserver)this).LineAdded(lineKey, storage.lines[lineKey]);
        }



        void ILineNetObserver.PointModified(uint key, Point before, Point after)
        {
            foreach (uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                internalAngleFromPoint1[lineKey] = CalcAngleBetweenLinesOnCurrLinePoint1(lineKey, database.lines[lineKey]);
                internalAngleFromPoint2[lineKey] = CalcAngleBetweenLinesOnCurrLinePoint2(lineKey, database.lines[lineKey]);
            }
        }
        void ILineNetObserver.LineAdded(uint key, Line line)
        {
            internalAngleFromPoint1.Add(key, CalcAngleBetweenLinesOnCurrLinePoint1(key, line));
            uint lineKeyBeforeCurrLineFromPoint1 = orderOfLines.LastLineOfALine(line.PointKey1, key);
            internalAngleFromPoint1[lineKeyBeforeCurrLineFromPoint1] = CalcAngleBetweenLinesOnCurrLinePoint1(lineKeyBeforeCurrLineFromPoint1, database.lines[lineKeyBeforeCurrLineFromPoint1]);

            internalAngleFromPoint2.Add(key, CalcAngleBetweenLinesOnCurrLinePoint2(key, line));
            uint lineKeyBeforeCurrLineFromPoint2 = orderOfLines.LastLineOfALine(line.PointKey2, key);
            internalAngleFromPoint1[lineKeyBeforeCurrLineFromPoint2] = CalcAngleBetweenLinesOnCurrLinePoint1(lineKeyBeforeCurrLineFromPoint2, database.lines[lineKeyBeforeCurrLineFromPoint2]);

        }
        void ILineNetObserver.LineModified(uint key, Line before, Line after)
        {
            if (before.PointKey1 != after.PointKey1 || before.PointKey2 != after.PointKey2)
            {
                internalAngleFromPoint1[key] = CalcAngleBetweenLinesOnCurrLinePoint1(key, after);
                internalAngleFromPoint2[key] = CalcAngleBetweenLinesOnCurrLinePoint2(key, after);
            }
        }
        void ILineNetObserver.LineRemoved(uint key, Line line)
        {
            internalAngleFromPoint1.Remove(key);
            internalAngleFromPoint2.Remove(key);
        }

        void ILineNetObserver.LineClear()
        {
            internalAngleFromPoint1.Clear();
            internalAngleFromPoint2.Clear();
        }

        private float CalcAngleBetweenLinesOnCurrLinePoint1(uint lineKey, Line line)
        {
            float thisLineAngle = lineAngles.fromPoint1[lineKey];
            uint getNextLineKey = orderOfLines.NextLineOfALine(line.PointKey1, lineKey);
            float nextLineAngle = (database.lines[getNextLineKey].PointKey1 == line.PointKey1) ? lineAngles.fromPoint1[getNextLineKey] : lineAngles.fromPoint2[getNextLineKey];

            //Check if the current line is the only line in the point, if so, return 2Pi to avoid calculation.
            if (database.linesOnPoint.linesOnPoint[line.PointKey1].Count == 1) { return 2 * MathF.PI; }

            //Check if the current line is the last line to calculate the angle differently
            //Example scenario:
            //Last line (current line) = 2Pi rad
            //First line (next line) = 0 rad
            //doing 0 - 2Pi would obviously result in wrong calculation
            //Solution: add 2Pi to the next line so that 2Pi - 2Pi = 0, yay!
            if (thisLineAngle > nextLineAngle) { return nextLineAngle + (2 * MathF.PI) - thisLineAngle; }

            //Calculate the difference of the two edge case scenarios above is false
            return nextLineAngle - thisLineAngle;
        }
        private float CalcAngleBetweenLinesOnCurrLinePoint2(uint lineKey, Line line)
        {
            float thisLineAngle = lineAngles.fromPoint2[lineKey];
            uint getNextLineKey = orderOfLines.NextLineOfALine(line.PointKey2, lineKey);
            float nextLineAngle = (database.lines[getNextLineKey].PointKey2 == line.PointKey2) ? lineAngles.fromPoint2[getNextLineKey] : lineAngles.fromPoint1[getNextLineKey];

            //Check if the current line is the only line in the point, if so, return 2Pi to avoid calculation.
            if (database.linesOnPoint.linesOnPoint[line.PointKey2].Count == 1) { return 2 * MathF.PI; }

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
}
