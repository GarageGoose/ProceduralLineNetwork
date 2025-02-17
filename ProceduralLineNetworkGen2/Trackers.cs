using GarageGoose.ProceduralLineNetwork.Elements;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork.Trackers
{
    interface ILineNetworkTracker
    {
        //Database of points and lines
        void Database(ref ElementsDatabase DB);
        //Unique tracker key
        void TrackerKey(uint Key);
        void Add(uint PointKey);
        void Remove(uint PointKey);
        void Update(uint PointKey);
        //Search for objects
        HashSet<uint> Search(Object[] Params);
    }

    class Location : ILineNetworkTracker
    {

    }
}
