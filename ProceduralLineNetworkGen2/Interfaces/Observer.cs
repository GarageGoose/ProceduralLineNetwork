namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{

    /// <summary>
    /// Observe changes on the line network. 
    /// </summary>
    public interface ILineNetworkObserverElement
    {
        /// <returns>Types of events to track</returns>
        ElementUpdateType[] observerElementSubscribeToEvents { get; }

        /// <param name="UpdateType">Type of event that happened</param>
        /// <param name="Data">Data associated with the event (Look to TrackingUpdateType for more info)</param>
        void LineNetworkElementUpdate(ElementUpdateType UpdateType, Object? Data);
    }

    public enum ElementUpdateType
    {
        /// <summary>
        /// Point update (Returns the point key (uint) associated with the event in LineNetworkElementUpdate)
        /// </summary>
        OnPointAddition, OnPointModification, OnPointRemoval,

        /// <summary>
        /// Line update (Returns the line key (uint) associated with the event in LineNetworkElementUpdate)
        /// </summary>
        OnLineAddition, OnLineModification, OnLineRemoval
    }

    public interface ILineNetworkObserverComponentAction
    {
        ComponentActionUpdate[] observerComponentActionSubscribeToEvents { get; }
        void LineNetworkComponentUpdate(ComponentActionUpdate Update);
    }

    public enum ComponentAction
    {
        Start, Finished
    }

    public struct ComponentActionUpdate
    {
        public object component;
        public ComponentAction action;
        public ComponentActionUpdate(object component, ComponentAction action)
        {
            this.component = component;
            this.action = action;
        }
    }

}
