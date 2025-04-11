using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class TrackLineAngles : ILineNetworkObserverElementSubscribe, ILineNetworkObserverElementAddedOrRemoved, ILineNetworkObserverElementModified, ILineNetworkObserverElementClear
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

        public TrackLineAngles(ElementsDatabase database)
        {
            this.database = database;
            lineAngleFromPoint1 = internalLineAngleFromPoint1;
            lineAngleFromPoint2 = internalLineAngleFromPoint2;
        }

        ElementUpdateType[] ILineNetworkObserverElementSubscribe.observerElementSubscribeToEvents { get; } =
        [
            ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification,
            ElementUpdateType.OnLineRemoval, ElementUpdateType.OnPointModification
        ];

        void ILineNetworkObserverElementModified.LineNetworkElementModified(ElementUpdateType UpdateType, uint key, object oldObject, object newObject)
        {
            if(UpdateType == ElementUpdateType.OnLineModification)
            {
                //Store the angle from the perspective of point 1 to save performance by just inverting this angle for point 2 (as oppose to recalculating it) just incase both end on the line is changed.
                float? angleFromPoint1New = null;

                if (((Line)oldObject).PointKey1 != ((Line)newObject).PointKey1)
                {
                    internalLineAngleFromPoint1.Remove(key);
                    angleFromPoint1New = CalcAngle(database.LinePoint1(key), database.LinePoint2(key));
                    internalLineAngleFromPoint1.Add(key, (float)angleFromPoint1New);
                }

                if (((Line)oldObject).PointKey2 != ((Line)newObject).PointKey2)
                {
                    internalLineAngleFromPoint2.Remove(key);

                    //Invert the angle if isn't null to avoid expensive computation, else recompute it from the perspective of point 2
                    internalLineAngleFromPoint2.Add(key, (angleFromPoint1New != null) ? InvertAngle((float)angleFromPoint1New) : CalcAngle(database.LinePoint2(key), database.LinePoint1(key)));
                }

                return;
            }
        }

        void ILineNetworkObserverElementAddedOrRemoved.LineNetworkElementAddedOrRemoved(ElementUpdateType UpdateType, uint Key)
        {
            if(UpdateType == ElementUpdateType.OnLineAddition)
            {
                float Point1Angle = CalcAngle(database.LinePoint1(Key), database.LinePoint2(Key));
                internalLineAngleFromPoint1.Add(Key, Point1Angle);

                //Invert the angle from Point1Angle to avoid expensive computation
                internalLineAngleFromPoint2.Add(Key, InvertAngle(Point1Angle));
                return;
            }
            internalLineAngleFromPoint1.Remove(Key);
            internalLineAngleFromPoint2.Remove(Key);
        }

        void ILineNetworkObserverElementClear.LineNetworkElementClear(ElementUpdateType UpdateType)
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

    public class TrackAngleBetweenLines
    {

    }
}
