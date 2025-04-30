using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralLineNetwork.CoreComponents.LineNetworkModification
{
    public class AddLinesOnPoint
    {
        public void SetPointAngularBiasComponents(IPointAngularBias[] components)
        {

        }
        public void SetLineLengthBiasComponents(ILineLengthBias[] components)
        {

        }
    }

    public interface IPointAngularBias
    {
        public Vector2[] GetLineAngularBias(uint pointKey);
    }

    public interface ILineLengthBias
    {
        public Vector2[] GetLineLengthBias(uint pointKey, float angle);
    }

    public class AngularBias
    {
        public float From;
        public float To;
        public float Intensity;
        public AngularBias(float from, float to, float intensity)
        {
            From = from;
            To = to;
            Intensity = intensity;
        }
    }

    public class LineLengthBias
    {
        public float From;
        public bool ToEnd;
        public float Intensity;

        public LineLengthBias(float from, bool toEnd)
        {
            From = from;
            ToEnd = toEnd;
        }
    }
}
