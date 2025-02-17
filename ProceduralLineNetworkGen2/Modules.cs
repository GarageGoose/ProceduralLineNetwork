using GarageGoose.ProceduralLineNetwork.Elements;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Trackers
{
    interface ILineNetworkTracker
    {
        //Database of points and lines
        void Database(ref ElementsDatabase DB);

        //Unique tracker key
        void TrackerKey(uint ElementKey);
        
        //Data in
        bool DataInput(Object[] Datas) => return false;

        void OnPointAddition(uint PointKey) => return;
        void OnLineAddition(uint LineKey)  => return;

        void OnPointUpdate(uint PointKey) => return;
        void OnLineUpdate(uint LineKey)  => return;

        void OnPointRemoval(uint PointKey) => return;
        void OnPointRemoval(uint LineKey) => return;

        //Search for objects
        HashSet<uint> Search(Object[] Params);
    }

    interface ILineNetworkBehavior
    {
        void Database(ref ElementsDatabase DB);
        void CurrPoint(uint PointKey);
        float ApplyRotation(float Radians);
        float ApplyDist(float Dist);
    }

    class TrackPointLocation : ILineNetworkTracker
    {
        void OnPointAddition(uint PointKey)
        {
            
        }

        void OnPointUpdate(uint PointKey)
        {
            
        }
        
        void OnPointRemoval(uint PointKey)
        {
            
        }
    }

    class TrackPointMaxRotation : ILineNetworkTracker
    {
        void OnPointAddition(uint PointKey)
        {
            
        }

        void OnPointUpdate(uint PointKey)
        {
            
        }
        
        void OnPointRemoval(uint PointKey)
        {
            
        }
    }
    
    class TrackPointMinRotation : ILineNetworkTracker
    {
        void OnPointAddition(uint PointKey)
        {
            
        }

        void OnPointUpdate(uint PointKey)
        {
            
        }
        
        void OnPointRemoval(uint PointKey)
        {
            
        }
    }

    class TrackPointLineCount : ILineNetworkTracker
    {
        void OnLineAddition(uint LineKey)
        {
            
        }

        void OnLineUpdate(uint LineKey)
        {
            
        }
        
        void OnLineRemoval(uint LineKey)
        {
            
        }
    }

    class TrackLineLength : ILineNetworkTracker
    {
        void OnLineAddition(uint LineKey)
        {
            
        }

        void OnLineUpdate(uint LineKey)
        {
            
        }
        
        void OnLineRemoval(uint LineKey)
        {
            
        }
    }

    class TrackElementID : ILineNetworkTracker
    {
        void OnLineAddition(uint LineKey)
        {
            
        }

        void OnLineUpdate(uint LineKey)
        {
            
        }
        
        void OnLineRemoval(uint LineKey)
        {
            
        }
    }

}
