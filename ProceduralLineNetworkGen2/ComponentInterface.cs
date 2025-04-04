namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetworkInherit
    {
        void Inherit(LineNetwork LineNetwork);
    }

    /// <summary>
    /// Observe changes on the line network. 
    /// </summary>
    public interface ILineNetworkObserver
    {
        /// <returns>Types of events to track</returns>
        UpdateType[] SubscribeToEvents();

        /// <param name="UpdateType">Type of event that happened</param>
        /// <param name="Data">Data associated with the event (Look to TrackingUpdateType for more info)</param>
        void LineNetworkChange(UpdateType UpdateType, Object? Data);
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
        /// Refresh data (Can be used when the data is suspected to be out of sync, return null)
        /// </summary>
        RefreshData
    }

    public interface ILineNetworkElementSearch
    {
        bool ThreadSafeSearch();

        /// <returns>The eligible elements key</returns>
        HashSet<uint> Search();
    }

    public interface ILineNetworkModification
    {
        /// <param name="SelectedElements">Target elements to perform the modification</param>
        /// <returns>True if the operation is successful, false if not</returns>
        bool ExecuteModification(HashSet<uint> SelectedElements);
    }
}
