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
        public LineLengthBiasSegment GetLineLengthBias(uint lineKey, Line TargetLine, float angle);
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
    public class LineLengthBiasSegment
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
        public LineLengthBiasSegment(float midpoint, float extention, float bias)
        {
            this.midpoint = midpoint;
            this.extention = extention;
            this.bias = bias;
        }

        /// <param name="endpoints">Leftmost and rightmost boundaries of a bias segment</param>
        /// <param name="bias">Bias intensity</param>
        public LineLengthBiasSegment(Vector2 endpoints, float bias)
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
        public readonly IReadOnlyList<LineLengthBiasSegment> lineBiases;

        private List<LineLengthBiasSegment> internalLineBiases = new();

        private int LineBiasSegmentAdd(LineLengthBiasSegment segment)
        {
            for(int i = 0; i < internalLineBiases.Count; i++)
            {
                if (internalLineBiases[i].midpoint < segment.midpoint)
                {
                    internalLineBiases.Insert(i + 1, segment);
                    return i + 1;
                }
            }
            internalLineBiases.Add(segment);
            return internalLineBiases.Count - 1;
        }

        public LineLengthBiasAdvanced()
        {
            lineBiases = internalLineBiases;
        }

        /// <summary>
        /// Add a new segment
        /// </summary>
        /// <param name="Segment"></param>
        /// <param name="CollisionAction"></param>
        public void Add(LineLengthBiasSegment Segment, BiasSegmentCollisionAction CollisionAction)
        {
            int currSegmentIndex = LineBiasSegmentAdd(Segment);
            bool LeftCollision;
            bool RightCollision;
            CheckCollision(currSegmentIndex, out LeftCollision, out RightCollision);

            if (LeftCollision)
            {
                switch (CollisionAction)
                {
                    case BiasSegmentCollisionAction.Blend:

                        break;

                    case BiasSegmentCollisionAction.PrioritizeThis:

                        break;

                    case BiasSegmentCollisionAction.PrioritizeColliding:

                        break;
                }
            }
        }

        public void CheckCollision(int currSegmentIndex, out bool LeftCol, out bool RightCol)
        {
            LineLengthBiasSegment CurrSegment = internalLineBiases[currSegmentIndex];

            if (currSegmentIndex < internalLineBiases.Count)
            {
                LineLengthBiasSegment SegmentAfter = internalLineBiases[currSegmentIndex + 1];
                RightCol = (CurrSegment.midpoint + CurrSegment.extention) > (SegmentAfter.midpoint - SegmentAfter.extention);
            }
            else RightCol = false;

            if (currSegmentIndex != 0)
            {
                LineLengthBiasSegment SegmentBefore = internalLineBiases[currSegmentIndex - 1];
                LeftCol = (CurrSegment.midpoint - CurrSegment.extention) < (SegmentBefore.midpoint + SegmentBefore.extention); 
            }
            else LeftCol = false;
        }
    }
}
