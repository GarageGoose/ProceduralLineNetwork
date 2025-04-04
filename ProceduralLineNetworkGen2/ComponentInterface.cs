namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    /// <summary>
    /// Inherit the instance of a stuff within the LineNetwork.
    /// </summary>
    public interface ILineNetworkInherit
    {
        /// <summary>
        /// Inherit essential managers (ElementsDatabase, ModificationManager, TrackingManager) and other external components aswell (eg. TrackpointAngles, TrackConnectedLinesOnPoint).
        /// <returns>Returns the component (in the same index as Components) when the operation is successful and null if not</returns>
        void Inherit(LineNetwork LineNetwork);
    }

    /// <summary>
    /// Track various stuff on the line network and retrieving elements with specific traits.
    /// </summary>
    public interface ILineNetworkObserver
    {
        /// <returns>Types of events to track</returns>
        UpdateType[] SubscribeToEvents();

        /// <param name="UpdateType">Type of event that happened</param>
        /// <param name="Data">Data associated with the event (Look to TrackingUpdateType for more info)</param>
        void LineNetworkChange(UpdateType UpdateType, Object Data);
        bool ThreadSafeDataAccess();
    }

    public enum UpdateType
    {
        /// <summary>
        /// Point update (Returns the point key (uint) associated with the event in LineNetworkChange)
        /// </summary>
        OnPointAddition, OnPointModificationBefore, OnPointModificationAfter, OnPointRemoval,

        /// <summary>
        /// Line update (Returns the line key (uint) associated with the event in LineNetworkChange)
        /// </summary>
        OnLineAddition, OnLineModificationBefore, OnLineModificationAfter, OnLineRemoval,

        /// <summary>
        /// Modification status update (Returns null in LineNetworkChange)
        /// </summary>
        ModificationStart, ModificationFinished,

        /// <summary>
        /// Modification component status update (Returns the component associated with the update in LineNetworkChange)
        /// </summary>
        ModificationComponentStart, ModificationComponentFinished,

        /// <summary>
        /// Refresh data (Can be used when the data is suspected to be out of sync)
        /// </summary>
        RefreshData
    }

    public interface ILineNetworkElementSearch
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
