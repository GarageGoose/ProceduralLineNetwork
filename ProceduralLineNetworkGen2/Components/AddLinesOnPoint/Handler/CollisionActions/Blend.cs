using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    public class BiasCollisionBlend : IBiasSegmentCollisionAction
    {
        public BiasSegment[] CollisionAction(CollisionData collDat)
        {
            if(collDat.precedingSegment != null)
            {
                //Resolve collision on the left edge
                float leftToCurrentAverageBiasValue = (collDat.precedingSegment.bias + collDat.currentSegment.bias) / 2;

                if (collDat.leftEdgeCollision == CollisionStatus.Partial)
                {
                    //Collision visualisation
                    //L----------R         <- Preceding bias segment
                    //      L----------R   <- Current bias segment

                    //Fix
                    //L-----R    L-----R   <- Preceding and current bias segments.
                    //      L----R         <- New bias segment with the average bias value of both bias segments.

                    return new BiasSegment[3] 
                    {
                        //Adjusted endpoints on the bias segment before the current one
                        new(new(collDat.precedingSegment.endpoint.left, collDat.currentSegment.endpoint.left), collDat.precedingSegment.bias),

                        //New bias segment that is on the overlapping area
                        new(new(collDat.currentSegment.endpoint.left, collDat.precedingSegment.endpoint.right), leftToCurrentAverageBiasValue),

                        //Adjusted endpoint of the current bias segment
                        new(new(collDat.precedingSegment.endpoint.right, collDat.currentSegment.endpoint.right), collDat.currentSegment.bias),
                    };
                }

                //Collision visualization
                //L---------------R  <- Current bias segment
                //   L----R          <- Precding bias segment

                //Fix
                //L--R    L-------R  <- Current bias segment split into two
                //   L----R          <- Preceding bias segment with the average bias value of both
            }


            //Resolve collision on the right edge
            if (collDat.rightEdgeCollision == CollisionStatus.Partial)
            {
                
            }
            if (collDat.rightEdgeCollision == CollisionStatus.Full)
            {

            }
        }
    }
}
