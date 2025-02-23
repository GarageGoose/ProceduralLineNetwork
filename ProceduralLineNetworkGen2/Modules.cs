using GarageGoose.ProceduralLineNetwork.Module.Interface;

namespace GarageGoose.ProceduralLineNetwork.Module.Default
{

    class TrackPointLocation : ILineNetworkTracker
    {
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }

        public void OnPointAddition(uint PointKey)
        {
            
        }

        public void OnPointUpdate(uint PointKey)
        {
            
        }
        
        public void OnPointRemoval(uint PointKey)
        {
            
        }
    }

    class TrackPointMaxRotation : ILineNetworkTracker
    {
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }

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
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }

        public void OnPointAddition(uint PointKey)
        {
            
        }

        public void OnPointUpdate(uint PointKey)
        {
            
        }

        public void OnPointRemoval(uint PointKey)
        {
            
        }
    }

    class TrackPointLineCount : ILineNetworkTracker
    {
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }
        public void OnLineAddition(uint LineKey)
        {
            
        }

        public void OnLineUpdate(uint LineKey)
        {
            
        }

        public void OnLineRemoval(uint LineKey)
        {
            
        }
    }

    class TrackLineLength : ILineNetworkTracker
    {
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }

        public void OnLineAddition(uint LineKey)
        {
            
        }

        public void OnLineUpdate(uint LineKey)
        {
            
        }

        public void OnLineRemoval(uint LineKey)
        {
            
        }
    }

    class TrackElementID : ILineNetworkTracker
    {
        public void Database(ElementsDatabase DB)
        {

        }

        public HashSet<uint> Search()
        {
            return new();
        }

        public void OnLineAddition(uint LineKey)
        {
            
        }

        public void OnLineUpdate(uint LineKey)
        {
            
        }

        public void OnLineRemoval(uint LineKey)
        {
            
        }
    }

}
