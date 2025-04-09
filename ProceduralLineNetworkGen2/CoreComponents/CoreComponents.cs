using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Tracks the connected lines on a point. Use TrackPointAngles if angular information is needed and connected lines on a point.
    /// </summary>

    class TrackPointPositionQuadTree : ILineNetworkObserverElement, ILineNetworkElementSearch
    {
        bool ILineNetworkObserverElement.ThreadSafeDataAccess { get; } = true;
        bool ILineNetworkElementSearch.ThreadSafeSearch { get; } = true;
        ElementUpdateType[] ILineNetworkObserverElement.observerElementSubscribeToEvents { get; } =
        [
            ElementUpdateType.OnPointAddition, ElementUpdateType.OnPointModification, 
            ElementUpdateType.OnPointRemoval, ElementUpdateType.RefreshData, ElementUpdateType.ClearData
        ];

        void ILineNetworkObserverElement.LineNetworkElementUpdate(ElementUpdateType UpdateType, object? Data)
        {
            switch(UpdateType)
            {
                case ElementUpdateType.OnPointAddition:

                    break;

                case ElementUpdateType.OnPointModification: 
                    //Data returns an array consisting of the key, item before modification, item after modification
                    break;

                case ElementUpdateType.OnPointRemoval: 
                    
                    break;

                case UpdateType.RefreshData:

                    break;

                case UpdateType.ClearData:

                    break;
            }
        }


        public HashSet<uint> Search()
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
