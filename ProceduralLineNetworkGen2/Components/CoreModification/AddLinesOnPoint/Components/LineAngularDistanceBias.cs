using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class LineAngularDistanceBias : IPointAngleBias
    {
        public float biasIntensity;
        public float linePadding;

        public LineAngularDistanceBias(float biasIntensity, float linePadding)
        {
            this.biasIntensity = biasIntensity;
            this.linePadding = linePadding;
        }

        public LineLengthBias GetLineAngularBias(uint pointKey, Point targetPoint)
        {
            return new(0, 0, 0);
        }
    }
}
