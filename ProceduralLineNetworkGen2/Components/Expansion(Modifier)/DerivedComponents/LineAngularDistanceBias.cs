using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class LineAngularDistance
    {
        LineDict lines;
        LinesOnPoint linesOnPoint;
        ObserveLineAngles lineAngles;
        public LineAngularDistance(LineNetwork lineNet, ObserveLineAngles lineAngles)
        {
            lines = lineNet.elements.lines;
            linesOnPoint = lineNet.elements.linesOnPoint;
            this.lineAngles = lineAngles;
        }

        public IBiasSegmentContainer GetBias(uint pointKey, float biasIn, float angularDistance)
        {
            BSegContainerSingle segmentContainer = new(biasIn);

            foreach(uint lineKey in linesOnPoint.linesOnPoint[pointKey])
            {
                float currentLineAngle = lines[lineKey].PointKey1 == pointKey ? lineAngles.fromPoint1[lineKey] : lineAngles.fromPoint2[lineKey];
                segmentContainer.AddSegment(currentLineAngle - angularDistance, currentLineAngle + angularDistance);
            }

            return segmentContainer;
        }
    }
}
