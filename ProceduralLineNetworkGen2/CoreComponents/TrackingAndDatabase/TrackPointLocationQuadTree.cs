using GarageGoose.ProceduralLineNetwork.Component.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralLineNetwork.CoreComponents
{
    /// <summary>
    /// Tracks the connected lines on a point. Use TrackPointAngles if angular information is needed and connected lines on a point.
    /// </summary>
    class TrackPointPositionQuadTree : ILineNetworkObserverElement, ILineNetworkElementSearch
    {
        bool ILineNetworkElementSearch.ThreadSafeSearch { get; } = true;
        ElementUpdateType[] ILineNetworkObserverElement.observerElementSubscribeToEvents { get; } =
        [
            ElementUpdateType.OnPointAddition, ElementUpdateType.OnPointModification,
            ElementUpdateType.OnPointRemoval
        ];

        void ILineNetworkObserverElement.LineNetworkElementUpdate(ElementUpdateType UpdateType, object? Data)
        {
            switch (UpdateType)
            {
                case ElementUpdateType.OnPointAddition:

                    break;

                case ElementUpdateType.OnPointModification:
                    //Data returns an array consisting of the key, item before modification, item after modification
                    break;

                case ElementUpdateType.OnPointRemoval:

                    break;
            }
        }


        public HashSet<uint> Search()
        {
            return new();
        }
    }
}
