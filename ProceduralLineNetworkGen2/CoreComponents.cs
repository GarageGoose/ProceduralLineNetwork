using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Tracks the connected lines on a point. Use TrackPointAngles if angular information is needed and connected lines on a point.
    /// </summary>

    class TrackPointPositionQuadTree : ILineNetworkObserver, ILineNetworkElementSearch
    {
        bool ILineNetworkObserver.ThreadSafeDataAccess { get; } = true;
        bool ILineNetworkElementSearch.ThreadSafeSearch { get; } = true;
        UpdateType[] ILineNetworkObserver.SubscribeToEvents { get; } =
        [
            UpdateType.OnPointAddition, UpdateType.OnPointModificationBefore,
            UpdateType.OnPointModificationAfter, UpdateType.OnPointRemoval,
            UpdateType.RefreshData
        ];

        void ILineNetworkObserver.LineNetworkChange(UpdateType UpdateType, object? Data)
        {
            switch(UpdateType)
            {
                case UpdateType.OnPointAddition:

                    break;

                case UpdateType.OnPointModificationBefore: 
                    
                    break;

                case UpdateType.OnPointModificationAfter: 
                    
                    break;

                case UpdateType.OnPointRemoval: 
                    
                    break;

                case UpdateType.RefreshData:

                    break;
            }
        }


        public HashSet<uint> Search()
        {
            return new();
        }
    }

    class TrackLinesCountAndAnglesOnPoint : ILineNetworkObserver, ILineNetworkElementSearch
    {
        public bool ThreadSafeDataAccess { get; } = true;
        public bool ThreadSafeSearch { get; } = true;

        public readonly Dictionary<uint, SortedList<float, Angle?>> LinesOnPoint = new();
        public readonly SortedDictionary<uint, float>? MaxAngles;
        public readonly SortedDictionary<uint, float>? MinAngles;

        public class Angle
        {
            public Angle(float lineAngle, float angleBetweenLines)
            {
                this.lineAngle = lineAngle;
                this.angleBetweenLines = angleBetweenLines;
            }
            public Angle()
            {

            }
            public float lineAngle;
            public float angleBetweenLines;
        }

        public readonly bool trackAngles;
        public readonly bool trackMaxAngle;
        public readonly bool trackMinAngle;
        private ElementsDatabase elementsDatabase;

        public TrackLinesCountAndAnglesOnPoint(ElementsDatabase elementsDatabase, bool trackAngles, bool trackMaxAngle, bool trackMinAngle)
        {
            this.elementsDatabase = elementsDatabase;
            this.trackAngles = trackAngles;
            this.trackMaxAngle = trackMaxAngle;
            this.trackMinAngle = trackMinAngle;
        }
        


        UpdateType[] ILineNetworkObserver.SubscribeToEvents { get; } =
        [
            UpdateType.OnLineAddition, UpdateType.OnLineModificationBefore,
            UpdateType.OnLineModificationAfter, UpdateType.OnLineRemoval,

            UpdateType.OnPointAddition, UpdateType.OnPointModificationBefore,
            UpdateType.OnPointModificationAfter, UpdateType.OnPointRemoval,

            UpdateType.RefreshData
        ];
        void ILineNetworkObserver.LineNetworkChange(UpdateType UpdateType, object? Data)
        {
            switch (UpdateType)
            {
                //
                case UpdateType.OnLineAddition:
                    LinesOnPoint.TryAdd((uint)Data, new());
                    LinesOnPoint[elementsDatabase.Lines[(uint)Data].PointKey1].Add((uint)Data, null);
                    break;

                case UpdateType.OnLineModificationBefore:

                    break;

                case UpdateType.OnLineModificationAfter:

                    break;

                case UpdateType.OnLineRemoval:

                    break;

                //
                case UpdateType.OnPointAddition:

                    break;

                case UpdateType.OnPointModificationBefore:

                    break;

                case UpdateType.OnPointModificationAfter:

                    break;

                case UpdateType.OnPointRemoval:

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
        private void CalculateLineAngle(uint lineKey, out Angle? fromPoint1, out Angle? fromPoint2)
        {
            if (trackAngles)
            {
                Angle point1Angle = new();
                Angle point2Angle = new();

                point1Angle.lineAngle = CalcAngle(elementsDatabase.LinePoint1(lineKey), elementsDatabase.LinePoint2(lineKey));

                //Inverting the angle from LineAngleFromPoint1 as oppose to calculating it again saves performance
                //To prevent the angle from going <0 and >2Pi, subtract Pi when the angle is equal or more than Pi, else add Pi
                point1Angle.lineAngle = (point1Angle.lineAngle >= MathF.PI) ? point1Angle.lineAngle - MathF.PI : point1Angle.lineAngle + MathF.PI;
            }
            fromPoint1 = null;
            fromPoint2 = null;
        }
        private void LinesOnPointAddEntry(uint lineKey, Angle? angle, PointKey pointKey)
        {
            if(pointKey == PointKey.one)
            {
                LinesOnPoint.TryAdd(elementsDatabase.Lines[lineKey].PointKey1, new());
                LinesOnPoint[elementsDatabase.Lines[lineKey].PointKey1].Add(lineKey, angle);
            }

            LinesOnPoint.TryAdd(elementsDatabase.Lines[lineKey].PointKey2, new());
            LinesOnPoint[elementsDatabase.Lines[lineKey].PointKey2].Add(lineKey, angle);

            //TODO: Reflect changes in angles between lines
        }

        private void LinesOnPointRemoveEntry(uint lineKey, PointKey pointKey)
        {
            if(pointKey == PointKey.one)
            {
                LinesOnPoint[elementsDatabase.Lines[lineKey].PointKey1].Remove(lineKey);
            }
            LinesOnPoint[elementsDatabase.Lines[lineKey].PointKey2].Remove(lineKey);

            //TODO: Reflect changes in angles between lines
        }

        private enum PointKey
        {
            one, two
        }
    }
    class TrackModificationChanges
    {
        
    }
    class TrackAndModifyCustomIdentifier
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
