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
        BiasSegment collidingSegment;
        CollisionStatus collisionStatus;
        public CollisionData(BiasSegment collidingSegment, CollisionStatus collisionStatus)
        {
            this.collidingSegment = collidingSegment;
            this.collisionStatus = collisionStatus;
        }
    }

    public enum CollisionStatus
    {
        /// <summary>
        /// Only the left edge of the segment is colliding to the right edge of the colliding segment
        /// </summary>
        PartialToLeft,

        /// <summary>
        /// Only the right edge of the segment is colliding to the left edge of the colliding segment
        /// </summary>
        PartialToRight,

        /// <summary>
        /// One segment is inside of another
        /// </summary>
        Full,

        /// <summary>
        /// No collision happned
        /// </summary>
        None
    }
}
