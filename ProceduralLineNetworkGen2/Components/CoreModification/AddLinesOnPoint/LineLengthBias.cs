using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralLineNetwork.Components.CoreModification.AddLinesOnPoint.AddLinesOnPointComponents
{
    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for line length bias.
    /// </summary>
    public interface ILineLengthBias
    {
        /// <param name="lineKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public LineLengthBias GetLineLengthBias(uint lineKey, Line TargetLine, float angle);
    }

    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for complex line length bias.
    /// </summary>
    public interface ILineLengthBiasAdvanced
    {
        /// <param name="lineKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public LineLengthBiasAdvanced GetLineLengthBias(uint lineKey, Line TargetLine, float angle);
    }

    /// <summary>
    /// Single segment line length bias for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class LineLengthBias
    {
        /// <summary>
        /// Bias segment midpoint.
        /// </summary>
        public float midpoint;

        /// <summary>
        /// distance from the left and rightmost edges of a bias segment.
        /// </summary>
        public float extention;

        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float bias;

        /// <param name="midpoint">Bias segment midpoint.</param>
        /// <param name="extention">Distance from the left and rightmost edges of a bias segment.</param>
        /// <param name="bias">
        /// Bias intensity from -1 to 1. from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public LineLengthBias(float midpoint, float extention, float bias)
        {
            this.midpoint = midpoint;
            this.extention = extention;
            this.bias = bias;
        }

        /// <param name="endpoints">Leftmost and rightmost boundaries of a bias segment</param>
        /// <param name="bias">Bias intensity</param>
        public LineLengthBias(Vector2 endpoints, float bias)
        {
            extention = (endpoints.Y - endpoints.X) / 2;
            midpoint = extention + endpoints.X;
            this.bias = bias;
        }
    }

    /// <summary>
    /// Hold and handles multiple line length bias segments for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class LineLengthBiasAdvanced
    {
        /// <summary>
        /// Sorted set of line length bias segments
        /// </summary>
        public readonly IReadOnlySet<LineLengthBias> lineBiases;

        private SortedSet<LineLengthBias> internalLineBiases = new(Comparer<LineLengthBias>.Create((a, b) => a.midpoint.CompareTo(b.midpoint)));

        public LineLengthBiasAdvanced()
        {
            lineBiases = internalLineBiases;
        }

        /// <summary>
        /// Add a new segment
        /// </summary>
        /// <param name="Segment"></param>
        /// <param name="CollisionAction"></param>
        public void Add(LineLengthBias Segment, BiasSegmentCollisionAction CollisionAction = BiasSegmentCollisionAction.AdjustToMidpoint)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrSegmentLeft"></param>
        /// <returns></returns>
        public bool CheckCollisionLeft(float CurrSegmentLeft)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrSegmentRight"></param>
        /// <returns></returns>
        public bool CheckCollisionRight(float CurrSegmentRight)
        {
            return false;
        }
    }
}
