using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{

    /// <summary>
    /// Marker interface for IBiasSegmentSingle and IBiasSegmentContainer
    /// </summary>
    public interface IBiasSegment { }

    /// <summary>
    /// Interface for a bias segment.
    /// </summary>
    public interface IBiasSegmentSingle
    {
        /// <summary>
        /// Single segment line length bias for <code>LineLengthAngularBias</code>.
        /// </summary>
        public float Midpoint { get; }

        /// <summary>
        /// Distance from the left and rightmost edges of a bias segment.
        /// </summary>
        public float Extention { get; }
        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float Bias { get; set; }

        /// <summary>
        /// Left and right endpoints.
        /// </summary>
        public BiasSegmentEndpoint Endpoint { get; }
    }

    /// <summary>
    /// Interface for holding many bias segments.
    /// </summary>
    public interface IBiasSegmentContainer
    {
        public IEnumerable<IBiasSegmentSingle> GetBiasSegments();
    }

    /// <summary>
    /// Single segment line length bias for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class BiasSegment : IBiasSegmentSingle
    {
        /// <summary>
        /// Bias segment midpoint.
        /// </summary>
        public float Midpoint { get; }

        /// <summary>
        /// Distance from the left and rightmost edges of a bias segment.
        /// </summary>
        public float Extention { get; }

        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float Bias { get; set; }

        /// <summary>
        /// Left and right endpoints.
        /// </summary>
        public BiasSegmentEndpoint Endpoint { get; }

        /// <param name="midpoint">Bias segment midpoint.</param>
        /// <param name="extention">Distance from the left and rightmost edges of a bias segment.</param>
        /// <param name="bias">
        /// Bias intensity from -1 to 1. from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public BiasSegment(float midpoint, float extention, float bias)
        {
            Midpoint = midpoint;
            Extention = extention;
            Bias = bias;
            Endpoint = new(midpoint - extention, midpoint + extention);
        }

        /// <param name="endpoint">Leftmost and rightmost boundaries of a bias segment</param>
        /// <param name="bias">Bias intensity</param>
        public BiasSegment(BiasSegmentEndpoint endpoint, float bias)
        {
            Extention = (endpoint.right - endpoint.left) / 2;
            Midpoint = Extention + endpoint.left;
            Bias = bias;
            Endpoint = endpoint;
        }
    }

    public struct BiasSegmentEndpoint
    {
        public readonly float left;
        public readonly float right;
        public BiasSegmentEndpoint(float left, float right)
        {
            if(left > right)
            {
                right = left;
                left = right;
            }
            this.left = left;
            this.right = right;
        }
    }

    /// <summary>
    /// Hold and handles multiple line length bias segments for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class BiasSegmentContainer : IBiasSegmentContainer
    {
        /// <summary>
        /// Sorted set of line length bias segments
        /// </summary>
        public readonly IReadOnlyList<BiasSegment> lineBiases;

        private List<BiasSegment> internalLineBiases = new();
        private Comparer<BiasSegment> lineBiasesComparer = Comparer<BiasSegment>.Create((a, b) => a.Midpoint.CompareTo(b.Midpoint));

        private int SortedAdd(BiasSegment Segment)
        {
            int index = internalLineBiases.BinarySearch(Segment, lineBiasesComparer);
            index = index < 0 ? ~index : index;
            internalLineBiases.Insert(index, Segment);
            return index;
        }

        public BiasSegmentContainer()
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
            if(collisionAction == null)
            {
                SortedAdd(Segment);
                return;
            }

            //Get the index of the new segment if it would be in internalLineBiases.
            int currSegmentIndex = internalLineBiases.BinarySearch(Segment, lineBiasesComparer);

            //If the midpoint of the new index dosent match any other midpoints in the list, it returns the bitwise negative index. This line of code corrects it.
            currSegmentIndex = currSegmentIndex < 0 ? ~currSegmentIndex : currSegmentIndex;

            //Check for segment overlap
            List<CollisionData> cData = CheckCollision(currSegmentIndex, Segment, out int precedingColCount, out int succeedingColCount);

            if(cData.Count == 0)
            {
                internalLineBiases.Insert(currSegmentIndex, Segment);
                return;
            }
            FixCollision(cData, collisionAction, precedingColCount, currSegmentIndex, succeedingColCount);
        }

        private List<CollisionData> CheckCollision(int currSegmentIndex, BiasSegment current, out int precedingSegmentsCollided, out int succeedingSegmentsCollided)
        {
            List<CollisionData> collDat = new();

            CollisionCheck collision = new();

            //Check preceding bias segments
            for (int i = currSegmentIndex - 1; i >= 0; i--)
            {
                CollisionData collCheck = collision.CheckCollision(current.Endpoint, internalLineBiases[i].Endpoint);
                if (collCheck.collisionStatus == CollisionStatus.NoneCollidingAtLeft || collCheck.collisionStatus == CollisionStatus.NoneCollidingAtRight)
                {
                    break;
                }
                collDat.Add(collCheck);
            }
            precedingSegmentsCollided = collDat.Count;

            //Check succeeding bias segments
            for (int i = currSegmentIndex + 1; i < internalLineBiases.Count; i++)
            {
                CollisionData collCheck = collision.CheckCollision(current.Endpoint, internalLineBiases[i].Endpoint);
                if (collCheck.collisionStatus == CollisionStatus.NoneCollidingAtRight || collCheck.collisionStatus == CollisionStatus.NoneCollidingAtLeft)
                {
                    break;
                }
                collDat.Add(collCheck);
            }
            succeedingSegmentsCollided = collDat.Count - precedingSegmentsCollided;

            return collDat;
        }

        /// <summary>
        /// Check and resolve pre-existing bias segment collisions at O(n * n at worst case scenario).
        /// </summary>
        /// <param name="collisionAction"></param>
        public void ResolveCollision(IBiasSegmentCollisionAction collisionAction)
        {
            for(int i = 0; i < internalLineBiases.Count; i++)
            {
                List<CollisionData> cData = CheckCollision(i, internalLineBiases[i], out int precedingColCount, out int succeedingColCount);

                if (cData.Count == 0)
                {
                    internalLineBiases.Insert(i, internalLineBiases[i]);
                    return;
                }
                FixCollision(cData, collisionAction, precedingColCount, i, succeedingColCount);
            }
        }

        private void FixCollision(List<CollisionData> cData, IBiasSegmentCollisionAction collisionAction, int precedingColCount, int currSegmentIndex, int succeedingColCount)
        {
            CollisionFix cFix = collisionAction.CollisionAction(cData);

            if (cFix.addCurrentSegment)
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
                    internalLineBiases.RemoveAt(currSegmentIndex - (cFix.addCurrentSegment ? 0 : 1));
                }
            }

            foreach (BiasSegment bSeg in cFix.newSegments)
            {
                SortedAdd(bSeg);
            }
        }

        public IEnumerable<IBiasSegmentSingle> GetBiasSegments() => internalLineBiases;

        /// <summary>
        /// Remove a segment at index
        /// </summary>
        public void RemoveAt(int index) => internalLineBiases.RemoveAt(index);

        /// <summary>
        /// Remove a segment
        /// </summary>
        public void Remove(BiasSegment segment) => internalLineBiases.Remove(segment);
    }

    /// <summary>
    /// Hold and handles multiple line length bias segments with the same bias value. More performant than BiasSegmentContainer
    /// </summary>
    public class BiasSegmentContainerSingleValue : IBiasSegmentContainer
    {
        public IReadOnlyList<BiasSegmentEndpoint> endpoints;
        public float biasValue;
        private static endpointComparerNoCollision endpointComparer = new();
        private List<BiasSegmentEndpoint> internalEndpoints = new();

        public BiasSegmentContainerSingleValue(float biasValue)
        {
            endpoints = internalEndpoints;
            this.biasValue = biasValue;
        }

        public void AddSegment(float leftEdge, float rightEdge)
        {
            int collidingSegmentLeftmost = BinarySearch(leftEdge, out bool locationInSegmentLeft);
            int collidingSegmentRightmost = BinarySearch(rightEdge, out bool locationInSegmentRight);

            if (collidingSegmentLeftmost + 1 < collidingSegmentRightmost)
            {
                for (int i = collidingSegmentLeftmost + 1; i < collidingSegmentRightmost; i++)
                {
                    internalEndpoints.RemoveAt(collidingSegmentLeftmost + 1);
                }
            }

            if(collidingSegmentLeftmost == collidingSegmentRightmost)
            {
                if (locationInSegmentLeft)
                {
                    bool adjustLeftEndpoint = endpoints[collidingSegmentLeftmost].left > leftEdge;
                    bool adjustRightEndpoint = endpoints[collidingSegmentLeftmost].right < rightEdge;
                    if (adjustLeftEndpoint || adjustRightEndpoint)
                    {
                        internalEndpoints[collidingSegmentLeftmost] = new(adjustLeftEndpoint ? leftEdge : endpoints[collidingSegmentLeftmost].left, adjustRightEndpoint ? rightEdge : endpoints[collidingSegmentLeftmost].right);
                    }
                    return;
                }
                internalEndpoints.Insert(collidingSegmentLeftmost + 1, new(leftEdge, rightEdge));
                return;
            }

            collidingSegmentRightmost = collidingSegmentLeftmost + 1;

            float newSegmentLeftmost = MathF.Min(endpoints[collidingSegmentLeftmost].left, leftEdge);
            float newSegmentRightmost = MathF.Max(endpoints[collidingSegmentRightmost].right, rightEdge);

            internalEndpoints[collidingSegmentLeftmost] = new(newSegmentLeftmost, newSegmentRightmost);
            internalEndpoints.RemoveAt(collidingSegmentRightmost);
        }

        public void RemoveSegment(float leftEdge, float rightEdge)
        {
            int collidingSegmentLeftmost = BinarySearch(leftEdge, out bool locationInSegmentLeft);
            int collidingSegmentRightmost = BinarySearch(rightEdge, out bool locationInSegmentRight);

            if (collidingSegmentLeftmost + 1 < collidingSegmentRightmost)
            {
                for (int i = collidingSegmentLeftmost + 1; i < collidingSegmentRightmost; i++)
                {
                    internalEndpoints.RemoveAt(collidingSegmentLeftmost + 1);
                }
            }

            if (collidingSegmentLeftmost == collidingSegmentRightmost)
            {
                if (!locationInSegmentLeft)
                {
                    return;
                }

                float collSegLeftLoc = endpoints[collidingSegmentLeftmost].left;
                float collSegRightLoc = endpoints[collidingSegmentLeftmost].right;

                bool createLeftSeg = collSegLeftLoc < leftEdge;
                bool createRightSeg = collSegRightLoc > rightEdge;

                if (createLeftSeg)
                {
                    internalEndpoints[collidingSegmentLeftmost] = new(collSegLeftLoc, leftEdge);

                    if (createRightSeg)
                    {
                        internalEndpoints.Insert(collidingSegmentLeftmost + 1, new(rightEdge, collSegRightLoc));
                    }
                }
                else if (createRightSeg)
                {
                    internalEndpoints[collidingSegmentLeftmost] = new(rightEdge, collSegRightLoc);
                }
                else
                {
                    internalEndpoints.RemoveAt(collidingSegmentLeftmost);
                }
            }

            collidingSegmentRightmost = collidingSegmentLeftmost + 1;

            if (endpoints[collidingSegmentLeftmost].left > leftEdge)
            {
                internalEndpoints.RemoveAt(collidingSegmentLeftmost);
            }
            else
            {
                internalEndpoints[collidingSegmentLeftmost] = new(endpoints[collidingSegmentLeftmost].left, leftEdge);
            }

            if (endpoints[collidingSegmentRightmost].right < rightEdge)
            {
                internalEndpoints.RemoveAt(collidingSegmentRightmost);
            }
            else
            {
                internalEndpoints[collidingSegmentRightmost] = new(rightEdge, internalEndpoints[collidingSegmentRightmost].right);
            }
        }
   
        public int BinarySearch(float location, out bool locationInSegment)
        {
            int min = 0;
            int max = internalEndpoints.Count() - 1;
            int mid = 0;
            while(min <= max)
            {
                mid = min + (int)MathF.Floor((max - min) / 2);
                if (internalEndpoints[mid].right < location)
                {
                    min = mid + 1;
                }
                else if(internalEndpoints[mid].left > location)
                {
                    min = max - 1;
                }
                else
                {
                    locationInSegment = true;
                    return mid;
                }
            }
            locationInSegment = false;
            return mid;
        }
        public int[] BinarySearchRange(float leftEdge, float rightEdge)
        {
            int min = BinarySearch(leftEdge, out bool locInSegLeft);
            int max = BinarySearch(rightEdge, out bool locInSegRight);
            int[] segmentsInRange = new int[max - min];
            for (int i = min; i <= max; i++)
            {
                segmentsInRange[i - min] = i;
            }
            return segmentsInRange;
        }

        public IEnumerable<IBiasSegmentSingle> GetBiasSegments()
        {
            BiasSegment[] biasSegments = new BiasSegment[endpoints.Count];
            int index = 0;
            foreach(BiasSegmentEndpoint endpoint in endpoints)
            {
                biasSegments[index++] = new(endpoint, biasValue);
            }
            return biasSegments;
        }
    }
}
