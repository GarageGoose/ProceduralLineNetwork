using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class BSegmentCollisionBlend : IBiasSegmentCollisionAction
    {
        public CollisionFix CollisionAction(IBiasSegmentSingle currSeg, List<CollisionData> collisionData)
        {
            BSegContainerSingle currSegment = new(currSeg.Bias);
            BSegContainer collSegments = new();

            currSegment.AddSegment(currSeg.Endpoint.left, currSeg.Endpoint.right);

            foreach(CollisionData collDat in collisionData)
            {
                currSegment.RemoveSegment(collDat.collidingSegment!.Endpoint.left, collDat.collidingSegment!.Endpoint.right);
            }

        }
    }
}
