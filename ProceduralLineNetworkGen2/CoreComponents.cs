using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Tracks the connected lines on a point. Use TrackPointAngles if angular information is needed and connected lines on a point.
    /// </summary>

    class TrackPointPositionQuadTree : ILineNetworkInherit, ILineNetworkObserver, ILineNetworkElementSearch
    {
        private LineNetwork LN = new(false);
        void ILineNetworkInherit.Inherit(LineNetwork LineNetwork) { LN = LineNetwork; }

        UpdateType[] ILineNetworkObserver.SubscribeToEvents()
        {
            return
            [
                UpdateType.OnPointAddition, UpdateType.OnPointModificationBefore,
                UpdateType.OnPointModificationAfter, UpdateType.OnPointRemoval,
                UpdateType.RefreshData
            ];
        }
        bool ILineNetworkObserver.ThreadSafeDataAccess() { return true; }
        void ILineNetworkObserver.LineNetworkChange(UpdateType UpdateType, object? Data)
        {

        }

        bool ILineNetworkElementSearch.ThreadSafeSearch() { return true; }
        HashSet<uint> ILineNetworkElementSearch.Search()
        {
            return new();
        }
    }

    class TrackLinesOnPoint : ILineNetworkInherit, ILineNetworkObserver, ILineNetworkElementSearch
    {
        private LineNetwork LN = new(false);
        void ILineNetworkInherit.Inherit(LineNetwork LineNetwork) { LN = LineNetwork; }

        public Dictionary<uint, SortedList<float, Angle?>> LinesOnPoint = new();
        public SortedDictionary<uint, float>? InternalMaxAngles;
        public SortedDictionary<uint, float>? InternalMinAngles;
     

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



        public TrackLinesOnPoint(bool TrackAngles, bool TrackMaxAngle, bool TrackMinAngle)
        {

        }

        bool ILineNetworkObserver.ThreadSafeDataAccess() { return true; }
        

        UpdateType[] ILineNetworkObserver.SubscribeToEvents()
        {
            return
            [
                UpdateType.OnLineAddition, UpdateType.OnLineModificationBefore,
                UpdateType.OnLineModificationAfter, UpdateType.OnLineRemoval,
                UpdateType.RefreshData
            ];
        }

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

        bool ILineNetworkElementSearch.ThreadSafeSearch() { return true; }
        HashSet<uint> ILineNetworkElementSearch.Search()
        {
            return new();
        }
    }
    class TrackModificationChanges
    {
        public List<uint, Point?> Before = new(); 
    }
    class TrackAndModifyCustomIdentifier : ILineNetworkInherit, ILineNetworkObserver, ILineNetworkElementSearch, ILineNetworkModification
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
