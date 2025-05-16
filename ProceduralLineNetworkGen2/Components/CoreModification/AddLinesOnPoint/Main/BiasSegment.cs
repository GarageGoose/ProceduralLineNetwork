using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for line length bias.
    /// </summary>
    public interface IBiasSegmentComponent
    {
        /// <param name="lineKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public IBiasSegmentContainer GetLineLengthBias(uint lineKey, Line TargetLine, float angle);
    }


    public interface IBiasSegmentContainer { }

    /// <summary>
    /// Single segment line length bias for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class BiasSegment : IBiasSegmentContainer
    {
        /// <summary>
        /// Bias segment midpoint.
        /// </summary>
        public readonly float midpoint;

        /// <summary>
        /// distance from the left and rightmost edges of a bias segment.
        /// </summary>
        public readonly float extention;

        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public readonly float bias;

        /// <summary>
        /// Left and right endpoints.
        /// </summary>
        public BiasSegmentEndpoint endpoint;

        /// <param name="midpoint">Bias segment midpoint.</param>
        /// <param name="extention">Distance from the left and rightmost edges of a bias segment.</param>
        /// <param name="bias">
        /// Bias intensity from -1 to 1. from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public BiasSegment(float midpoint, float extention, float bias)
        {
            this.midpoint = midpoint;
            this.extention = extention;
            this.bias = bias;
            endpoint = new(midpoint - extention, midpoint + extention);
        }

        /// <param name="endpoint">Leftmost and rightmost boundaries of a bias segment</param>
        /// <param name="bias">Bias intensity</param>
        public BiasSegment(BiasSegmentEndpoint endpoint, float bias)
        {
            extention = (endpoint.right - endpoint.left) / 2;
            midpoint = extention + endpoint.left;
            this.bias = bias;
            this.endpoint = endpoint;
        }
    }

    public class BiasSegmentEndpoint
    {
        public readonly float left;
        public readonly float right;
        public BiasSegmentEndpoint(float left, float right)
        {
            this.left = left;
            this.right = right;
        }
    }

    /// <summary>
    /// Hold and handles multiple line length bias segments for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class LineLengthBiasAdvanced : IBiasSegmentContainer
    {
        /// <summary>
        /// Sorted set of line length bias segments
        /// </summary>
        public readonly IReadOnlyList<BiasSegment> lineBiases;

        private List<BiasSegment> internalLineBiases = new();

        //Check if the midpoint at index is less than the current segment midpoint then inser it at i + 1.
        private int BiasSegmentAdd(BiasSegment segment)
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
        public void Add(BiasSegment Segment, IBiasSegmentCollisionAction? collisionAction = null)
        {
            int currSegmentIndex = BiasSegmentAdd(Segment);

            if(collisionAction == null)
            {
                return;
            }

            CollisionData collDat = CheckCollision(currSegmentIndex);

            //Check if any collision happened
            if((collDat.leftEdgeCollision != CollisionStatus.None || collDat.rightEdgeCollision != CollisionStatus.None))
            {
                internalLineBiases.RemoveAt(currSegmentIndex);
                foreach (BiasSegment biasSegment in collisionAction.CollisionAction(collDat))
                {
                    BiasSegmentAdd(biasSegment);
                }
            }
        }

        public CollisionData CheckCollision(int currSegmentIndex)
        {
            CollisionData collDat = new CollisionData(internalLineBiases[currSegmentIndex]);

            //Check collision on the left edge
            if (currSegmentIndex != 0)
            {
                collDat.precedingSegment = internalLineBiases[currSegmentIndex - 1];

                //Check if the left edge of the current segment is less than the right edge of the segment before it. If so, they are partially colliding.
                collDat.leftEdgeCollision = collDat.currentSegment.endpoint.left < collDat.precedingSegment.endpoint.right
                    ? CollisionStatus.Partial : CollisionStatus.None;

                //Check for full collision if a partial collision happned
                if (collDat.leftEdgeCollision == CollisionStatus.Partial)
                {
                    //Check if the left edge of the current segment is less than or equal to the left edge of the segment before it.
                    collDat.leftEdgeCollision = collDat.currentSegment.endpoint.left <= collDat.precedingSegment.endpoint.left
                        ? CollisionStatus.Full : CollisionStatus.Partial;
                }
            }
            else
            {
                //No collision happned on the left edge of the current segment since it is the last segment.
                collDat.leftEdgeCollision = CollisionStatus.None;
            }

            //Check collision on the right edge
            if (currSegmentIndex < internalLineBiases.Count)
            {
                collDat.succeedingSegment = internalLineBiases[currSegmentIndex + 1];

                //Check if the right edge of the current segment is more than the left edge of the segment after it. If so, they are partially colliding.
                collDat.rightEdgeCollision = collDat.currentSegment.endpoint.right > collDat.succeedingSegment.endpoint.left
                    ? CollisionStatus.Partial : CollisionStatus.None;

                //Check for full collision if a partial collision happned
                if (collDat.rightEdgeCollision == CollisionStatus.Partial)
                {
                    //Check if the right edge of the current segment is more than or equal to the right edge of the segment after it. If so, they are fully colliding.
                    collDat.rightEdgeCollision = collDat.currentSegment.endpoint.right >= collDat.succeedingSegment.endpoint.right
                        ? CollisionStatus.Full : CollisionStatus.Partial;
                }
            }
            else
            {
                //No collision happned on the right edge of the current segment since it is the last segment.
                collDat.rightEdgeCollision = CollisionStatus.None;
            }

            return collDat;
        }

        public List<CollisionData> CheckCollision2(int currSegmentIndex)
        {
            BiasSegment current = internalLineBiases[currSegmentIndex];
            List<CollisionData> collDat = new();

            //Check preceding bias segments
            for(int i = currSegmentIndex - 1; i >= 0; i--)
            {
                //Check if the right side of the colliding bias segment is greater than
                //the left side of the current bias segment (L = left edge, R = right edge)
                //L-------R     <- Colliding bias segment
                //    L-------R <- Current bias segment
                if (internalLineBiases[i].endpoint.right > internalLineBiases[currSegmentIndex].endpoint.left)
                {
                    //Check if the left side of the colliding bias segment is greater than
                    //the left side of the current bias segment (L = left edge, R = right edge)
                    //L-----------R <- Current bias segment
                    //  L-----R     <- Colliding bias segment
                    if (internalLineBiases[i].endpoint.left >= internalLineBiases[currSegmentIndex].endpoint.left)
                    {
                        collDat.Add(new(internalLineBiases[i], CollisionStatus.Full));
                    }
                    else
                    {
                        collDat.Add(new(internalLineBiases[i], CollisionStatus.PartialToLeft));
                    }
                }
                else
                {
                    break;
                }
            }

            //Check succeeding bias segments
            for(int i = currSegmentIndex + 1; i < internalLineBiases.Count; i++)
            {

            }
        }
    }
}
