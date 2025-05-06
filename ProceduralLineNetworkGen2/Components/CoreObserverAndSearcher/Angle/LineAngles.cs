using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{

    /// <summary>
    /// Track the angle of a line from the perspective of Point1 and Point2
    /// </summary>
    public class ObserveLineAngles : LineNetworkObserver, ILineAngleTracker
    {
        public bool ThreadSafeDataAccess { get; } = true;

        //Access datas of lines and point to track
        private ElementStorage database;

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

        /// <summary>
        /// Track the angle of a line from the perspective of Point1 and Point2
        /// </summary>
        /// <param name="storage">Storage of elements</param>
        public ObserveLineAngles(ElementStorage storage) : base(0, true, [ObserverEvent.LineAdded, ObserverEvent.LineModified, ObserverEvent.LineRemoved, ObserverEvent.PointModified])
        {
            this.database = storage;
            fromPoint1 = internalLineAngleFromPoint1;
            fromPoint2 = internalLineAngleFromPoint2;

            foreach(uint lineKey in storage.lines.Keys)
            {
                LineAdded(lineKey, new(0, 0));
            }
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
            Console.WriteLine("called: " + key);
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

            float Angle = MathF.Atan2(diffY, diffX);

            //Convert angle to 0 to 2pi since Angle returns -pi to pi and the rest of the code assumes angle from 0 to 2pi
            return (Angle < 0) ? Angle += 2 * MathF.PI : Angle;
        }

        //Subtract pi if the angle is >pi else add pi to keep the angle from surpassing <0 and >2pi.
        private float InvertAngle(float Angle) => (Angle >= MathF.PI) ? Angle - MathF.PI : Angle + MathF.PI;
    }
}
