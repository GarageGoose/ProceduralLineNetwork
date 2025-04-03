namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    /// <summary>
    /// Inherit the instance of a stuff within the LineNetwork.
    /// </summary>
    public interface ILineNetworkInherit
    {
        /// <summary>
        /// Inherit essential managers (ElementsDatabase, ModificationManager, TrackingManager) and other external components aswell (eg. TrackpointAngles, TrackConnectedLinesOnPoint).
        /// Returns true when the operation is successful and false if not. Components is null if false aswall.
        /// </summary>
        /// <typeparam name="TLineNetwork">Target Type</typeparam>
        /// <param name="Component">Instance of the Target Type in the LineNetwork.</param>
        /// <returns></returns>
        bool GetComponent<TLineNetwork>(out TLineNetwork? Component);
    }

    /// <summary>
    /// Track various stuff on the line network and retrieving elements with specific traits.
    /// </summary>
    public interface ILineNetworkTracking
    {
        /// <returns>Types of events to track</returns>
        TrackingUpdateType[] SubscribeToEvents();

        /// <param name="UpdateType">Type of event that happened</param>
        /// <returns>Data associated with the event (Look to TrackingUpdateType for more info)</returns>
        Object? LineNetworkChange(TrackingUpdateType UpdateType);

        bool ThreadSafeAccess();
    }

    public enum TrackingUpdateType
    {
        //Point update (Returns the point key (uint) associated with the event)
        OnPointAddition, OnPointModificationBefore, OnPointModificationAfter, OnPointRemoval,

        //Line update (Returns the line key (uint) associated with the event)
        OnLineAddition, OnLineModificationBefore, OnLineModificationAfter, OnLineRemoval,

        //Modification status update (Returns null)
        ModificationStart, ModificationFinished,
        
        //Modification component status update (Returns the component associated with the update)
        ModificationComponentStart, ModificationComponentFinished
    }

    public interface ILineNetworkTrackingSearch
    {
        bool ThreadSafeSearch();

        /// <returns>The eligible elements key</returns>
        HashSet<uint> Search();
    }

    /// <summary>
    /// Used when modifying line network
    /// </summary>
    public interface ILineNetworkModification
    {
        /// <param name="SelectedElements">Target elements to perform the modification</param>
        void ExecuteModification(HashSet<uint> SelectedElements);
    }
}
