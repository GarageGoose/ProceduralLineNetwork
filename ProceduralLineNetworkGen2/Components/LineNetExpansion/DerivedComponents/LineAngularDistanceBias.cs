using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;
using ProceduralLineNetwork.LineNetwork.Helpers;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class LineAngularDistance
    {
        LineNetAggregator lineNet;
        ObserveLineAngles lineAngles;
        public LineAngularDistance(LineNetAggregator aggregator, ObserveLineAngles lineAngles)
        {
            lineNet = aggregator;
            this.lineAngles = lineAngles;
        }

        public IBiasSegmentContainer GetBias(uint pointKey, Point targetPoint, float BiasIn)
        {
            BiasSegmentContainerSingleValue segmentContainer = new();
        }
    }
}
