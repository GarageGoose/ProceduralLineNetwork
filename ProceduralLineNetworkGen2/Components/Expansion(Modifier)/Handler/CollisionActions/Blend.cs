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

            for(int i = 0; i < collisionData.Count; i++)
            {
                CollisionData currCollDat = collisionData[i];

                if (currCollDat.collisionStatus == CollisionStatus.PartialToLeft)
                {
                    //
                    //L-----R
                    //   L-----R
                    //
                    //L--R
                    //   L-----R
                    collSegments.Add(new(new(currCollDat.collidingSegment!.Endpoint.left, currSeg.Endpoint.left), currCollDat.collidingSegment!.Bias));
                }

                if (currCollDat.collisionStatus == CollisionStatus.PartialToRight)
                {
                    //
                    //   L-----R
                    //L-----R
                    //
                    //      L--R
                    //L-----R
                    collSegments.Add(new(new(currSeg.Endpoint.right, currCollDat.collidingSegment!.Endpoint.right), currCollDat.collidingSegment!.Bias));
                }

                if (currCollDat.alignmentStatus == CollisionAlignment.NoAlignment)
                {
                    //   L-----R
                    //L-----R

                    collSegments.Add()
                }
            }
        }
    }
}
