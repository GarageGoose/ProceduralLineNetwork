using GarageGoose.ProceduralLineNetwork.Elements;
namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetwork
    {
        void InheritDatabase(ElementsDatabase DB) { }
        void InheritTrackerComponents(TrackerManager Tracker) { }
        void InheritBehaviorComponents(BehaviorManager Behavior) { }
        void Seed(int Seed) { }
        bool ThreadSafeOperation();
    }

    /// <summary>
    /// Used for tracking various stuff on the line network and retrieving elements with specific traits.
    /// </summary>
    public interface ILineNetworkTracker
    {
        void OnElementAddition(uint PointKey, ElementType EType) { }
        void OnElementUpdate(uint PointKey, ElementType EType) { }
        void OnElementRemoval(uint PointKey, ElementType EType) { }
        void RefreshData(uint LineKey, ElementType EType) { }
        bool ThreadSafeAccess();
        HashSet<uint> Search() { return new(); }
    }

    /// <summary>
    /// Used for custom behavior when expanding the line network
    /// </summary>
    public interface ILineNetworkBehavior
    {
        Priority ComponentPriority();
        ModificationType ComponentModification(); 
        void CurrElement(uint CurrElem) { }
        void ElementSet(HashSet<uint> Elements) { }
        void Changes(NewChanges Changes);
    }

    public class NewChanges
    {
        //Make it null for deletion
        public Dictionary<uint, Point?> ExistingPointsModification = new();
        public Dictionary<uint, Line?> ExistingLinesModification = new();
        
        //
        public SortedList<uint, Point> NewPoints = new();
        public SortedList<uint, Line> NewLines = new();
    }

    /// <summary>
    /// Priority level for behavior components
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Executed before the main operation (eg. clearing way for new elements).
        /// </summary>
        PreChanges,
        
        /// <summary>
        /// Main operation (eg. adding new elements).
        /// </summary>
        MainChanges,

        /// <summary>
        /// Executed after the main operation (eg. clearing line intersections after adding new elements).
        /// </summary>
        PostChanges
    }

    /// <summary>
    /// Types of modification used by components
    /// </summary>
    public enum ModificationType
    {
        /// <summary>
        /// Line network expansion (ex. adding new elements)
        /// </summary>
        Expansion,
        
        /// <summary>
        /// Line network modification (ex. changing point positions)
        /// </summary>
        Modification,
        
        /// <summary>
        /// Line network element deletion (ex. deleting points close together)
        /// </summary>
        Deletion
    }
}
