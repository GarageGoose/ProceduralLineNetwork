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

        public IBiasSegmentContainer GetBias(uint pointKey, float biasIn, float angularDistance)
        {
            BiasSegmentContainerSingleValue segmentContainer = new(biasIn);

            foreach(uint lineKey in lineNet.connectedLinesOnPoint.linesOnPoint[pointKey])
            {
                float currentLineAngle = lineNet.lines[lineKey].PointKey1 == pointKey ? lineAngles.fromPoint1[lineKey] : lineAngles.fromPoint2[lineKey];
                segmentContainer.AddSegment(currentLineAngle - angularDistance, currentLineAngle + angularDistance);
            }

            return segmentContainer;
        }
    }
}
