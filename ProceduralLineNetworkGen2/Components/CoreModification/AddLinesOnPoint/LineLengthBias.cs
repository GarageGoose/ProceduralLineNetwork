using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Bias range starting point.
        /// </summary>
        public float from;

        /// <summary>
        /// Bias range endpoint.
        /// </summary>
        public float to;

        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float bias;

        /// <param name="from">Bias range starting point.</param>
        /// <param name="to">Bias range endpoint.</param>
        /// <param name="bias">
        /// Bias from -1 to 1. from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public LineLengthBias(float from, float to, float bias)
        {
            this.from = from;
            this.to = to;
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

        private SortedSet<LineLengthBias> internalLineBiases = new();

        public LineLengthBiasAdvanced()
        {
            lineBiases = internalLineBiases;
        }

        /// <summary>
        /// Add a new segment
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Bias"></param>
        /// <param name="CollisionAction"></param>
        public void Add(float Left, float Right, float Bias, BiasSegmentCollisionAction CollisionAction = BiasSegmentCollisionAction.AdjustToMidpoint)
        {

        }

        /// <summary>
        /// Add a new segment
        /// </summary>
        /// <param name="Midpoint"></param>
        /// <param name="Extention"></param>
        /// <param name="Bias"></param>
        /// <param name="CollisionAction"></param>
        public void AddFromMidpoint(float Midpoint, float Extention, float Bias, BiasSegmentCollisionAction CollisionAction = BiasSegmentCollisionAction.AdjustToMidpoint)
        {

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
