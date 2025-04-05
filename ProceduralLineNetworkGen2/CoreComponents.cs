using GarageGoose.ProceduralLineNetwork.Component.Interface;
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

    class TrackLinesOnPoint : ILineNetworkObserver, ILineNetworkElementSearch
    {
        public bool ThreadSafeDataAccess { get; } = true;
        public bool ThreadSafeSearch { get; } = true;

        public Dictionary<uint, SortedList<float, Angle?>> LinesOnPoint = new();
        public SortedDictionary<uint, float>? MaxAngles;
        public SortedDictionary<uint, float>? MinAngles;

        public class Angle
        {
            public Angle(float LineAngle, float AngleBetweenLines)
            {
                this.LineAngle = LineAngle;
                this.AngleBetweenLines = AngleBetweenLines;
            }
            public float LineAngle;
            public float AngleBetweenLines;
        }

        public readonly bool trackAngles;
        public readonly bool trackMaxAngle;
        public readonly bool trackMinAngle;

        public TrackLinesOnPoint(bool TrackAngles, bool TrackMaxAngle, bool TrackMinAngle)
        {

        }
        


        UpdateType[] ILineNetworkObserver.SubscribeToEvents { get; } =
        [
            UpdateType.OnLineAddition, UpdateType.OnLineModificationBefore,
            UpdateType.OnLineModificationAfter, UpdateType.OnLineRemoval,
            UpdateType.RefreshData
        ];
        void ILineNetworkObserver.LineNetworkChange(UpdateType UpdateType, object? Data)
        {
            switch (UpdateType)
            {
                case UpdateType.OnLineAddition:

                    break;

                case UpdateType.OnLineModificationBefore:

                    break;

                case UpdateType.OnLineModificationAfter:

                    break;

                case UpdateType.OnLineRemoval:

                    break;

                case UpdateType.RefreshData:

                    break;
            }
        }

        HashSet<uint> ILineNetworkElementSearch.Search()
        {
            return new();
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
