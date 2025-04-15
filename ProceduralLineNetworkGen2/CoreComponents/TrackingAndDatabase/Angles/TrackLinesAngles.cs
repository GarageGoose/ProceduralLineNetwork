using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class TrackLineAngles : LineNetworkObserver
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
        public readonly IReadOnlyDictionary<uint, float> lineAngleFromPoint1;

        /// <summary>
        /// Check line angle from the perspective of the second connected point.
        /// Key is line key and float is angle in radian.
        /// </summary>
        public readonly IReadOnlyDictionary<uint, float> lineAngleFromPoint2;

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates()
        {
            return [ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnPointModification];
        }

        public TrackLineAngles(ElementsDatabase database) : base(0, true)
        {
            this.database = database;
            lineAngleFromPoint1 = internalLineAngleFromPoint1;
            lineAngleFromPoint2 = internalLineAngleFromPoint2;
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
            //Store the angle from the perspective of point 1 to save performance by just inverting this angle for point 2 (as oppose to recalculating it) just incase both end on the line is changed.
            float? angleFromPoint1New = null;

            if (before.PointKey1 != after.PointKey1)
            {
                angleFromPoint1New = CalcAngle(database.LinePoint1(key), database.LinePoint2(key));
                internalLineAngleFromPoint1[key] = (float)angleFromPoint1New;
            }

            if (before.PointKey2 != after.PointKey2)
            {
                //Invert the angle if isn't null to avoid expensive computation, else recompute it from the perspective of point 2
                internalLineAngleFromPoint2[key] = (angleFromPoint1New != null) ? InvertAngle((float)angleFromPoint1New) : CalcAngle(database.LinePoint2(key), database.LinePoint1(key));
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

    public class TrackOrderOfLinesOnPoint : LineNetworkObserver
    {
        readonly TrackLineAngles lineAngle;

        private readonly Dictionary<uint, SortedList<float, uint>> internalOrderedLines = new();

        public TrackOrderOfLinesOnPoint(TrackLineAngles lineAngle) : base(1, true)
        {
            this.lineAngle = lineAngle;
        }

        public IReadOnlyList<uint> GetLineOrderInKeyAtPoint(uint pointKey) => (IReadOnlyList<uint>)internalOrderedLines[pointKey].Values;
        public IReadOnlyList<float> GetLineOrderInAngleAtPoint(uint pointKey) => (IReadOnlyList<float>)internalOrderedLines[pointKey].Keys;

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() => 
            [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification,ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        protected override void PointModified(uint key, Point before, Point after)
        {

        }
        protected override void LineAdded(uint key, Line line)
        {

        }
        protected override void LineModified(uint key, Line before, Line after)
        {

        }
        protected override void LineRemoved(uint key, Line line)
        {

        }
        protected override void LineClear()
        {

        }
    }

    public class TrackAngleBetweenLines : LineNetworkObserver
    {
        private readonly TrackLineAngles lineAngles;

        public Dictionary<uint, float> internalAngleFromPoint1;
        public Dictionary<uint, float> internalAngleFromPoint2;

        public IReadOnlyDictionary<uint, float> fromPoint1;
        public IReadOnlyDictionary<uint, float> fromPoint2;

        public TrackAngleBetweenLines(TrackLineAngles lineAngles) : base(2, true)
        {
            this.lineAngles = lineAngles;

            internalAngleFromPoint1 = new();
            internalAngleFromPoint2 = new();

            fromPoint1 = internalAngleFromPoint1;
            fromPoint2 = internalAngleFromPoint2;
        }

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() =>
            [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        protected override void PointModified(uint key, Point before, Point after)
        {
            
        }
        protected override void LineAdded(uint key, Line line)
        {

        }
        protected override void LineModified(uint key, Line before, Line after)
        {

        }
        protected override void LineRemoved(uint key, Line line)
        {

        }
        protected override void LineClear()
        {

        }
    }
}
