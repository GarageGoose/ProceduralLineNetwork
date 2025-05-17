using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Interface for handling bias segment collisions
    /// </summary>
    public interface IBiasSegmentCollisionAction
    {
        CollisionFix CollisionAction(List<CollisionData> collisionData);
    }

    /// <summary>
    /// Contains information on a bias segment collision.
    /// </summary>
    public class CollisionData
    {
        public readonly BiasSegment collidingSegment;
        public readonly CollisionStatus collisionStatus;
        public readonly CollisionAlignment? alignmentStatus;
        public CollisionData(BiasSegment collidingSegment, CollisionStatus collisionStatus, CollisionAlignment collisionAlignment = CollisionAlignment.NoAlignment)
        {
            this.collidingSegment = collidingSegment;
            this.collisionStatus = collisionStatus;
            alignmentStatus = collisionAlignment;
        }
    }

    /// <summary>
    /// Contains information for fixing bias segment overlap
    /// </summary>
    public class CollisionFix
    {
        public IEnumerable<BiasSegment> newSegments;
        public bool removeCurrentSegment;
        public bool removeCollidingSegments;
        public CollisionFix(IEnumerable<BiasSegment> newSegments, bool removeCurrentSegment, bool removeCollidingSegments)
        {
            this.newSegments = newSegments;
            this.removeCurrentSegment = removeCurrentSegment;
            this.removeCollidingSegments = removeCollidingSegments;
        }
    }

    /// <summary>
    /// Provides information if and how both segments overlapped
    /// </summary>
    public enum CollisionStatus
    {
        /// <summary>
        /// Only the left side of the current bias segment is colliding.
        /// </summary>
        PartialToLeft,

        /// <summary>
        /// Only the right side of the current bias segment is colliding.
        /// </summary>
        PartialToRight,

        /// <summary>
        /// The colliding bias segment is inside the current bias segment.
        /// </summary>
        CollidingWithinCurrent,

        /// <summary>
        /// The current segment is inside the colliding segment.
        /// </summary>
        CurrentWithinColliding,

        /// <summary>
        /// Both endpoints of the current and colliding segments is equal.
        /// </summary>
        Equal,

        /// <summary>
        /// No collision happned
        /// </summary>
        None
    }

    /// <summary>
    /// Additional data if the two edges of a segment is aligned for edge case scenarios
    /// </summary>
    public enum CollisionAlignment
    {
        /// <summary>
        /// Left edge of both segments is aligned
        /// </summary>
        //L-----R
        //L---R
        BothSegmentEqualToLeft,

        /// <summary>
        /// Right edge of both segments is aligned
        /// </summary>
        //L-----R
        //  L---R
        BothSegmentEqualToRight,

        /// <summary>
        /// Left edge of the current segment aligns with the right edge of the colliding segment
        /// </summary>
        //    L---R <- Current
        //L---R     <- Colliding
        CurrentLeftAlignsToCollidingRight,

        /// <summary>
        /// Right edge of the current segment aligns with the left edge of the colliding segment
        /// </summary>
        //L---R     <- Current
        //    L---R <- Colliding
        CurrentRightAlignsToCollidingLeft,

        /// <summary>
        /// Both edge of both segments are equal
        /// </summary>
        //L---R
        //L---R
        BothSidesEqual,

        //L---R
        //  L---R
        NoAlignment
    }

    public class CollisionCheck
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="colliding"></param>
        /// <returns></returns>
        public CollisionData CheckCollision(BiasSegment current, BiasSegment colliding)
        {
            //More lightweight than the builtin CompareTo
            CompVal rightToRight = CompareValue(current.endpoint.right, colliding.endpoint.right);
            CompVal leftToLeft = CompareValue(current.endpoint.left, colliding.endpoint.left);

            //Less readable but more efficient, comments is provided to improve readability (hopefully)
            //L----------------R  <- Bias segment
            //^Leftmost edge   ^Rightmost edge
            return (leftToLeft, rightToRight) switch
            {
                //Check collision on the right edge of the current segment to the left edge of the current segment
                (CompVal.Less, CompVal.Less) => CompareValue(current.endpoint.right, colliding.endpoint.left) switch
                {
                    //L---R         <- Current bias segment
                    //        L---R <- Colliding bias segment
                    CompVal.Less => new(colliding, CollisionStatus.None),

                    //L----R    <- Current bias segment
                    //   L----R <- Colliding bias segment
                    CompVal.More => new(colliding, CollisionStatus.PartialToRight),

                    //L----R      <- Current bias segment
                    //     L----R <- Colliding bias segment
                    _ => new(colliding, CollisionStatus.PartialToRight, CollisionAlignment.CurrentRightAlignsToCollidingLeft)
                },

                //L-------R <- Current bias segment
                //  L---R   <- Colliding bias segment
                (CompVal.Less, CompVal.More) => new(colliding, CollisionStatus.CollidingWithinCurrent),

                //L-------R <- Current bias segment
                //  L-----R <- Colliding bias segment
                (CompVal.Less, CompVal.Equal) => new(colliding, CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToRight),

                //  L---R   <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.More, CompVal.Less) => new(colliding, CollisionStatus.CurrentWithinColliding),

                //Check collision on the left edge of the current segment to the right edge of the colliding segment
                (CompVal.More, CompVal.More) => CompareValue(current.endpoint.left, colliding.endpoint.right) switch
                {
                    //  L----R <- Current bias segment
                    //L----R   <- Colliding bias segment
                    CompVal.Less => new(colliding, CollisionStatus.PartialToLeft),

                    //        L---R <- Current bias segment
                    //L---R         <- Colliding bias segment
                    CompVal.More => new(colliding, CollisionStatus.None),

                    //     L----R <- Current bias segment
                    //L----R      <- Colliding bias segment
                    _ => new(colliding, CollisionStatus.PartialToLeft, CollisionAlignment.CurrentLeftAlignsToCollidingRight)
                },

                //  L-----R <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.More, CompVal.Equal) => new(colliding, CollisionStatus.PartialToRight, CollisionAlignment.BothSegmentEqualToRight),

                //L-----R   <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.Equal, CompVal.Less) => new(colliding, CollisionStatus.CurrentWithinColliding, CollisionAlignment.BothSegmentEqualToLeft),

                //L--------R <- Current bias segment
                //L------R   <- Colliding bias segment
                (CompVal.Equal, CompVal.More) => new(colliding, CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToLeft),

                //L------R <- Current bias segment
                //L------R <- Colliding bias segment
                _ => new(colliding, CollisionStatus.Equal, CollisionAlignment.BothSidesEqual)
            };
        }

        //More readable and lightweight than the builtin CompareTo
        private static CompVal CompareValue(float current, float toCompare)
        {
            if (current < toCompare) return CompVal.Less;
            if (current > toCompare) return CompVal.More;
            else return CompVal.Equal;
        }
        private enum CompVal
        {
            Equal, Less, More
        }
    }
}
