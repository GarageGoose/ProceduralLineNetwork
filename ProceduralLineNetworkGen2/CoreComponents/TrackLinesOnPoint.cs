using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    class TrackLineAngles : ILineNetworkObserverElement, ILineNetworkElementSearch
    {
        public bool ThreadSafeDataAccess { get; } = true;
        public bool ThreadSafeSearch { get; } = true;

        private ElementsDatabase database;

        private readonly Dictionary<uint, float> internalLineAngleFromPoint1 = new();
        private readonly Dictionary<uint, float> internalLineAngleFromPoint2 = new();

        public readonly IReadOnlyDictionary<uint, float> lineAngleFromPoint1;
        public readonly IReadOnlyDictionary<uint, float> lineAngleFromPoint2;

        public TrackLineAngles(ElementsDatabase database)
        {
            this.database = database;
            lineAngleFromPoint1 = internalLineAngleFromPoint1;
            lineAngleFromPoint2 = internalLineAngleFromPoint2;
        }



        ElementUpdateType[] ILineNetworkObserverElement.observerElementSubscribeToEvents { get; } =
        [
            ElementUpdateType.OnLineAddition,
            ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval,

            ElementUpdateType.OnPointAddition,
            ElementUpdateType.OnPointModification, ElementUpdateType.OnPointRemoval,
        ];
        void ILineNetworkObserverElement.LineNetworkElementUpdate(ElementUpdateType UpdateType, object? Data)
        {
            switch (UpdateType)
            {
                //
                case ElementUpdateType.OnLineAddition:
                    float Point1Angle = CalcAngle(database.LinePoint1((uint)Data!), database.LinePoint2((uint)Data!));
                    internalLineAngleFromPoint1.Add((uint)Data!, Point1Angle);

                    //Invert the angle from Point1Angle to avoid expensive computation
                    //Subtract pi if the angle is >pi else add pi to keep the angle from surpassing <0 and >2pi.
                    internalLineAngleFromPoint2.Add((uint)Data!, (Point1Angle >= MathF.PI) ? Point1Angle - MathF.PI : Point1Angle + MathF.PI);
                    break;

                case ElementUpdateType.OnLineModification:

                    break;

                case ElementUpdateType.OnLineRemoval:

                    break;

                //
                case ElementUpdateType.OnPointModification:

                    break;

                //
                case UpdateType.RefreshData:

                    break;
            }
        }

        public HashSet<uint> Search()
        {
            return new();
        }

        //https://stackoverflow.com/questions/2676719/calculating-the-angle-between-a-line-and-the-x-axis
        private float CalcAngle(Point one, Point two)
        {
            float diffX = two.x - one.x;
            float diffY = two.y - one.y;
            return MathF.Atan2(diffY, diffX);
        }

        private enum LinePoint
        {
            one, two
        }
    }

    public class TrackAngleBetweenLines
    {

    }
}
