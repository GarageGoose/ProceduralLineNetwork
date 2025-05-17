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

    public struct BiasSegmentEndpoint
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
    public class BiasSegmentSortedCollision : IBiasSegmentContainer
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

        public BiasSegmentSortedCollision()
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

            List<CollisionData> cData = CheckCollision(currSegmentIndex, out int precedingColCount, out int succeedingColCount);
            CollisionFix cFix = collisionAction.CollisionAction(cData);

            if (cFix.removeCurrentSegment)
            {
                internalLineBiases.RemoveAt(currSegmentIndex);
            }

            if (cFix.removeCollidingSegments)
            {
                for (int i = precedingColCount; i > 0; i--)
                {
                    internalLineBiases.RemoveAt(currSegmentIndex - precedingColCount);
                }
                for (int i = succeedingColCount; i > 0; i--)
                {
                    internalLineBiases.RemoveAt(currSegmentIndex - (cFix.removeCurrentSegment ? 0 : 1));
                }
            }

            foreach(BiasSegment bSeg in cFix.newSegments)
            {
                BiasSegmentAdd(bSeg);
            }
        }

        public void ResolveCollision(IBiasSegmentCollisionAction collisionAction)
        {

        }

        public List<CollisionData> CheckCollision(int currSegmentIndex, out int precedingSegmentsCollided, out int succeedingSegmentsCollided)
        {
            BiasSegment current = internalLineBiases[currSegmentIndex];
            List<CollisionData> collDat = new();

            CollisionCheck collision = new();

            //Check preceding bias segments
            for(int i = currSegmentIndex - 1; i >= 0; i--)
            {
                CollisionData collCheck = collision.CheckCollision(current, internalLineBiases[i]);
                if(collCheck.collisionStatus == CollisionStatus.None)
                {
                    break;
                }
                collDat.Add(collCheck);
            }
            precedingSegmentsCollided = collDat.Count;

            //Check succeeding bias segments
            for(int i = currSegmentIndex + 1; i < internalLineBiases.Count; i++)
            {
                CollisionData collCheck = collision.CheckCollision(current, internalLineBiases[i]);
                if (collCheck.collisionStatus == CollisionStatus.None)
                {
                    break;
                }
                collDat.Add(collCheck);
            }
            succeedingSegmentsCollided = collDat.Count - precedingSegmentsCollided;

            return collDat;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BiasSegmentCuttable : IBiasSegmentContainer
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class BiasSegmentCuttableMultipleVal : IBiasSegmentContainer
    {

    }
}
