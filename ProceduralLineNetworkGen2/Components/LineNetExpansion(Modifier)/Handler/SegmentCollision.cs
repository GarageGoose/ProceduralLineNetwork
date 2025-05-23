
namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Interface for handling bias segment collisions
    /// </summary>
    public interface IBiasSegmentCollisionAction
    {
        CollisionFix CollisionAction(IBiasSegmentSingle currSegment, List<CollisionData> collisionData);
    }

    /// <summary>
    /// Contains information on a bias segment collision.
    /// </summary>
    public class CollisionData
    {
        public readonly BiasSegment? collidingSegment;
        public readonly CollisionStatus collisionStatus;
        public readonly CollisionAlignment alignmentStatus;
        public CollisionData(CollisionStatus collisionStatus, CollisionAlignment collisionAlignment = CollisionAlignment.NoAlignment)
        {
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
        public bool addCurrentSegment;
        public bool removeCollidingSegments;
        public CollisionFix(IEnumerable<BiasSegment> newSegments, bool removeCurrentSegment, bool removeCollidingSegments)
        {
            this.newSegments = newSegments;
            this.addCurrentSegment = removeCurrentSegment;
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
        /// No overalap, colliding segment is to the left of the current segment. Also considered no overlap if only the edges overlapped (Check CollisionAlignment if that happned).
        /// </summary>
        //Eg.
        //L---R
        //    L---R 
        //    ^ Considered no overlap even if the edges overlap
        NoneCollidingAtLeft,

        /// <summary>
        /// No overalap, colliding segment is to the right of the current segment. Also considered no overlap if only the edges overlapped (Check CollisionAlignment if that happned).
        /// </summary>
        //Eg.
        //L---R
        //    L---R 
        //    ^ Considered no overlap even if the edges overlap
        NoneCollidingAtRight
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
        public CollisionData CheckCollision(BiasSegmentEndpoint current, BiasSegmentEndpoint colliding)
        {
            //More lightweight than the builtin CompareTo
            CompVal rightToRight = CompareValue(current.right, colliding.right);
            CompVal leftToLeft = CompareValue(current.left, colliding.left);

            //Less readable but more efficient, comments is provided to improve readability (hopefully)
            //L----------------R  <- Bias segment
            //^Leftmost edge   ^Rightmost edge
            return (leftToLeft, rightToRight) switch
            {
                //Check collision on the right edge of the current segment to the left edge of the current segment
                (CompVal.Less, CompVal.Less) => CompareValue(current.right, colliding.left) switch
                {
                    //L---R         <- Current bias segment
                    //        L---R <- Colliding bias segment
                    CompVal.Less => new(CollisionStatus.NoneCollidingAtRight),

                    //L----R    <- Current bias segment
                    //   L----R <- Colliding bias segment
                    CompVal.More => new(CollisionStatus.PartialToRight),

                    //L----R      <- Current bias segment
                    //     L----R <- Colliding bias segment
                    _ => new(CollisionStatus.NoneCollidingAtRight, CollisionAlignment.CurrentRightAlignsToCollidingLeft)
                },

                //L-------R <- Current bias segment
                //  L---R   <- Colliding bias segment
                (CompVal.Less, CompVal.More) => new(CollisionStatus.CollidingWithinCurrent),

                //L-------R <- Current bias segment
                //  L-----R <- Colliding bias segment
                (CompVal.Less, CompVal.Equal) => new(CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToRight),

                //  L---R   <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.More, CompVal.Less) => new(CollisionStatus.CurrentWithinColliding),

                //Check collision on the left edge of the current segment to the right edge of the colliding segment
                (CompVal.More, CompVal.More) => CompareValue(current.left, colliding.right) switch
                {
                    //  L----R <- Current bias segment
                    //L----R   <- Colliding bias segment
                    CompVal.Less => new(CollisionStatus.PartialToLeft),

                    //        L---R <- Current bias segment
                    //L---R         <- Colliding bias segment
                    CompVal.More => new(CollisionStatus.NoneCollidingAtLeft),

                    //     L----R <- Current bias segment
                    //L----R      <- Colliding bias segment
                    _ => new(CollisionStatus.NoneCollidingAtLeft, CollisionAlignment.CurrentLeftAlignsToCollidingRight)
                },

                //  L-----R <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.More, CompVal.Equal) => new(CollisionStatus.PartialToRight, CollisionAlignment.BothSegmentEqualToRight),

                //L-----R   <- Current bias segment
                //L-------R <- Colliding bias segment
                (CompVal.Equal, CompVal.Less) => new(CollisionStatus.CurrentWithinColliding, CollisionAlignment.BothSegmentEqualToLeft),

                //L--------R <- Current bias segment
                //L------R   <- Colliding bias segment
                (CompVal.Equal, CompVal.More) => new(CollisionStatus.CollidingWithinCurrent, CollisionAlignment.BothSegmentEqualToLeft),

                //L------R <- Current bias segment
                //L------R <- Colliding bias segment
                _ => new(CollisionStatus.Equal, CollisionAlignment.BothSidesEqual)
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

    /// <summary>
    /// Custom comparer for bias segment endpoints. -1 is less than, 0 if colliding with each other, then 1 is more than.
    /// </summary>
    public class endpointComparerNoCollision : IComparer<BiasSegmentEndpoint>
    {
        private CollisionCheck colCheck = new();
        public int Compare(BiasSegmentEndpoint x, BiasSegmentEndpoint y)
        {
            CollisionData colDat = colCheck.CheckCollision(x, y);
            return colDat.collisionStatus switch
            {
                (CollisionStatus.NoneCollidingAtLeft) => colDat.alignmentStatus switch
                {
                    //       L---R <- Current (x)
                    //L---R        <- Colliding (y)
                    (CollisionAlignment.NoAlignment) => 1,

                    //    L---R <- Current (x)
                    //L---R     <- Colliding (y)
                    _ => 0
                },

                (CollisionStatus.NoneCollidingAtRight) => colDat.alignmentStatus switch
                {
                    //L---R        <- Current (x)
                    //       L---R <- Colliding (y)
                    (CollisionAlignment.NoAlignment) => -1,

                    //L---R     <- Current (x)
                    //    L---R <- Colliding (y)
                    _ => 0
                },

                //Any other current and colliding segment collision
                _ => 0
            };
        }
    }


}
