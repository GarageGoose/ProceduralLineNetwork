using GarageGoose.ProceduralLineNetwork.Elements;
using System.Numerics;
namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetwork
    {
        void InheritDatabase(ElementsDatabase DB) { }
        void InheritTrackerComponents(TrackerManager Tracker) { }
        void InheritModificationComponents(ModificationManager Modification) { }
        void Seed(int Seed) { }
        bool ThreadSafeOperation();
    }

    /// <summary>
    /// Used for tracking various stuff on the line network and retrieving elements with specific traits.
    /// </summary>
    public interface ILineNetworkTracker
    {
        //Point update
        void OnPointAddition(uint PointKey) { }
        void OnPointUpdate(uint PointKey) { }
        void OnPointRemoval(uint PointKey) { }

        //Line update
        void OnLineAddition(uint LineKey) { }
        void OnLineUpdate(uint LineKey) { }
        void OnLineRemoval(uint LineKey) { }

        //Modification update
        void ModificationStart() { }
        void ModificationComponentStart() { }
        void ModificationComponentFinished() { }
        void ModificationFinished() { }

        //General
        void RefreshData() { }
        bool ThreadSafeAccess();
        HashSet<uint> Search() { return new(); }
    }

    /// <summary>
    /// Used when modifying line network
    /// </summary>
    public interface ILineNetworkModification
    {
        void SelectedElements(HashSet<uint> Elements) { }
        void Execute();
    }
}
