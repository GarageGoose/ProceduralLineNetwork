
using GarageGoose.ProceduralLineNetwork.Module.Interface;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        //Contains primary lines and points data
        private ElementsDatabase DB = new();

        //Contains line network modules for doing various stuff
        public ModuleHandler ModuleHandler = new();

        public HashSet<uint> FindPointsViaTrackers(bool Multithread)
        {
            
            foreach(ILineNetworkTracker Tracker in ModuleHandler.GetTrackerModules())
            {
                Tracker.FindObjects();
            }
        }

        public void ExpandLineNetwork(HashSet<uint> PointKeys)
        {
            
        }
    }
}


