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
    /// 
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
    /// 
    /// </summary>
    public enum CollisionAlignment
    {
        BothSegmentEqualToLeft,

        BothSegmentEqualToRight,

        CurrentLeftAlignsToCollidingRight,

        CurrentRightAlignsToCollidingLeft,

        BothSidesEqual,

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
            if (CompareValue(current.endpoint.right, colliding.endpoint.left) == )
            {

            }

            //More lightweight than the builtin CompareTo
            CompVal rightToRight = CompareValue(current.endpoint.right, colliding.endpoint.right);
            CompVal leftToLeft   = CompareValue(current.endpoint.left,  colliding.endpoint.left);

            //Less readable but more efficient, comments is provided to improve readability (hopefully)
            //L----------------R  <- Bias segment
            //^Leftmost edge   ^Rightmost edge
            switch (leftToLeft)
            {
                case CompVal.Less:
                    switch (rightToRight)
                    {
                        case CompVal.Less:
                            //Check collision on the right edge of the current segment to the left edge of the current segment
                            switch(CompareValue(current.endpoint.right, colliding.endpoint.left))
                            {
                                case CompVal.Less:
                                    //L----R          <- Current bias segment
                                    //       L----R   <- Colliding bias segment
                                    return new(colliding, CollisionStatus.None);

                                case CompVal.More:
                                    //L----R    <- Current bias segment
                                    //   L----R <- Colliding bias segment
                                    return new(colliding, CollisionStatus.PartialToRight);

                                case CompVal.Equal:
                                    //L----R      <- Current bias segment
                                    //     L----R <- Colliding bias segment
                                    return new(colliding, CollisionStatus.PartialToRight, CollisionAlignment.CurrentRightAlignsToCollidingLeft);
                            }
                            break;

                        case CompVal.More:
                            //L---------R <- Current bias segment
                            //  L-----R   <- Colliding bias segment
                            return new(colliding, CollisionStatus.CollidingWithinCurrent);

                        case CompVal.Equal:
                            //L-------R <- Current bias segment
                            //  L-----R <- Colliding bias segment
                            return new(colliding, CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToRight);
                    }
                    break;

                case CompVal.More:
                    switch (rightToRight)
                    {
                        case CompVal.Less:
                            //  L---R   <- Current bias segment
                            //L-------R <- Colliding bias segment
                            return new(colliding, CollisionStatus.CurrentWithinColliding);

                        case CompVal.More:
                            //Check collision on the left edge of the current segment to the right edge of the colliding segment
                            switch(CompareValue(current.endpoint.left, colliding.endpoint.right))
                            {
                                case CompVal.Less:
                                    //  L----R <- Current bias segment
                                    //L----R   <- Colliding bias segment
                                    return new(colliding, CollisionStatus.PartialToLeft);

                                case CompVal.More:
                                    //       L----R <- Current bias segment
                                    //L----R        <- Colliding bias segment
                                    return new(colliding, CollisionStatus.None);

                                case CompVal.Equal:
                                    //     L----R <- Current bias segment
                                    //L----R      <- Colliding bias segment
                                    return new(colliding, CollisionStatus.PartialToLeft, CollisionAlignment.CurrentLeftAlignsToCollidingRight);
                            }
                            break;

                        case CompVal.Equal:
                            //  L-----R <- Current bias segment
                            //L-------R <- Colliding bias segment
                            return new(colliding, CollisionStatus.PartialToRight, CollisionAlignment.BothSegmentEqualToRight);
                    }
                    break;

                case CompVal.Equal:
                    switch (rightToRight)
                    {
                        case CompVal.Less:
                            //L-----R   <- Current bias segment
                            //L-------R <- Colliding bias segment
                            return new(colliding, CollisionStatus.CurrentWithinColliding, CollisionAlignment.BothSegmentEqualToLeft);

                        case CompVal.More:
                            //L--------R <- Current bias segment
                            //L------R   <- Colliding bias segment
                            return new(colliding, CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToLeft);

                        case CompVal.Equal:
                            //L------R <- Current bias segment
                            //L------R <- Colliding bias segment
                            return new(colliding, CollisionStatus.Equal, CollisionAlignment.BothSidesEqual);
                    }
                    break;
            }
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
