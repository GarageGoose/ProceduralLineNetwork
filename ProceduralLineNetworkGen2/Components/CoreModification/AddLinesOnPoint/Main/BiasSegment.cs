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
        public BiasSegment(float midpoint, float extention, float bias)
        {
            this.midpoint = midpoint;
            this.extention = extention;
            this.bias = bias;
        }

        /// <param name="endpoints">Leftmost and rightmost boundaries of a bias segment</param>
        /// <param name="bias">Bias intensity</param>
        public BiasSegment(Vector2 endpoints, float bias)
        {
            extention = (endpoints.Y - endpoints.X) / 2;
            midpoint = extention + endpoints.X;
            this.bias = bias;
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
                collDat.segmentBefore = internalLineBiases[currSegmentIndex - 1];

                //Check if the left edge of the current segment is less than the right edge of the segment before it. If so, they are partially colliding.
                collDat.leftEdgeCollision = (collDat.currentSegment.midpoint - collDat.currentSegment.extention) < (collDat.segmentBefore.midpoint + collDat.segmentBefore.extention) 
                    ? CollisionStatus.Partial : CollisionStatus.None;

                //Check for full collision if a partial collision happned
                if (collDat.leftEdgeCollision == CollisionStatus.Partial)
                {
                    //Check if the left edge of the current segment is less than or equal to the left edge of the segment before it.
                    collDat.leftEdgeCollision = (collDat.currentSegment.midpoint - collDat.currentSegment.extention) <= (collDat.segmentBefore.midpoint + collDat.segmentBefore.extention) 
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
                collDat.segmentAfter = internalLineBiases[currSegmentIndex + 1];

                //Check if the right edge of the current segment is more than the left edge of the segment after it. If so, they are partially colliding.
                collDat.rightEdgeCollision = (collDat.currentSegment.midpoint + collDat.currentSegment.extention) > (collDat.segmentAfter.midpoint - collDat.segmentAfter.extention)
                    ? CollisionStatus.Partial : CollisionStatus.None;

                //Check for full collision if a partial collision happned
                if (collDat.rightEdgeCollision == CollisionStatus.Partial)
                {
                    //Check if the right edge of the current segment is more than or equal to the right edge of the segment after it. If so, they are fully colliding.
                    collDat.rightEdgeCollision = (collDat.currentSegment.midpoint + collDat.currentSegment.extention) >= (collDat.segmentAfter.midpoint + collDat.segmentAfter.extention)
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
    }
}
