using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Interface for handling bias segment collisions
    /// </summary>
    public interface IBiasSegmentCollisionAction
    {
        BiasSegment[] CollisionAction(CollisionData collisionData);
    }

    /// <summary>
    /// Contains information on a bias segment collision
    /// </summary>
    public class CollisionData
    {
        /// <summary>
        /// The main bias segment which this class is for.
        /// </summary>
        public BiasSegment currentSegment;

        /// <summary>
        /// Collision informarion on the left edge
        /// </summary>
        public CollisionStatus leftEdgeCollision = CollisionStatus.None;

        /// <summary>
        /// Bias segment before this
        /// </summary>
        public BiasSegment? segmentBefore;

        /// <summary>
        /// Collision information on the right edge
        /// </summary>
        public CollisionStatus rightEdgeCollision = CollisionStatus.None;

        /// <summary>
        /// Bias segment after this
        /// </summary>
        public BiasSegment? segmentAfter;

        /// <summary>
        /// Contains information on a bias segment collision
        /// </summary>
        /// <param name="currentSegment">The main bias segment which this class is for.</param>
        public CollisionData(BiasSegment currentSegment) => this.currentSegment = currentSegment;
    }

    public enum CollisionStatus
    {
        /// <summary>
        /// No collision happned
        /// </summary>
        None,

        /// <summary>
        /// Only the edge of the segments is colliding
        /// </summary>
        Partial,

        /// <summary>
        /// One segment is inside of another
        /// </summary>
        Full,
    }
}
