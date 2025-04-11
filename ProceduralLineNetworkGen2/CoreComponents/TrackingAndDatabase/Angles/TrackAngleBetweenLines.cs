using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Component.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralLineNetwork.Component.Core
{
    public class TrackAngleBetweenLines : ILineNetworkObserverComponentAction, ILineNetworkObserverElementSubscribe, ILineNetworkObserverElementModified, ILineNetworkObserverElementClear, ILineNetworkObserverElementAddedOrRemoved
    {
        public ComponentActionUpdate[] observerComponentActionSubscribeToEvents { get; }
        private TrackLineAngles trackLineAngles;
        public TrackAngleBetweenLines(TrackLineAngles trackLineAnglesInstance)
        {
            trackLineAngles = trackLineAnglesInstance;
            observerComponentActionSubscribeToEvents = [new(trackLineAnglesInstance, ComponentAction.Finished)];
        }

        void ILineNetworkObserverComponentAction.LineNetworkComponentUpdate(object Component, ComponentAction Action)
        {

        }
    }
}
