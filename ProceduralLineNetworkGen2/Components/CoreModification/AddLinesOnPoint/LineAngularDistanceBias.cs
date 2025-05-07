using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;

namespace ProceduralLineNetwork.Components.CoreModification.AddLinesOnPoint
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

        public NewLineBias GetLineAngularBias(uint pointKey, Point targetPoint)
        {
            
        }
    }
}
