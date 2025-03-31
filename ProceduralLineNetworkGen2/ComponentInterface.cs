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
        void OnPointModification(uint PointKey) { }
        void OnPointRemoval(uint PointKey) { }

        //Line update
        void OnLineAddition(uint LineKey) { }
        void OnLineModification(uint LineKey) { }
        void OnLineRemoval(uint LineKey) { }

        //Modification update
        void ModificationStart() { }
        void ModificationFinished() { }
        void ModificationComponentStart(ILineNetworkModification Component) { }
        void ModificationComponentFinished(ILineNetworkModification Component) { }

        //General
        void RefreshData() { }  //Triggered when the database is supected to be out of sync
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
