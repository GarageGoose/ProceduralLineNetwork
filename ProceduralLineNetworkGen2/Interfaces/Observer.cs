namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{

    /// <summary>
    /// Observe changes on the line network. 
    /// </summary>
    public interface ILineNetworkObserverElementSubscribe
    {
        /// <returns>Types of events to track</returns>
        ElementUpdateType[] observerElementSubscribeToEvents { get; }
    }

    public interface ILineNetworkObserverElementAddedOrRemoved
    {
        void LineNetworkElementAddedOrRemoved(ElementUpdateType UpdateType, uint Key);
    }
    public interface ILineNetworkObserverElementModified
    {
        void LineNetworkElementModified(ElementUpdateType UpdateType, uint Key, object OldElement, object NewElement);
    }
    public interface ILineNetworkObserverElementClear
    {
        void LineNetworkElementClear(ElementUpdateType UpdateType);
    }

    public enum ElementUpdateType
    {
        /// <summary>
        /// Point update (Returns the point key (uint) associated with the event in LineNetworkElementUpdate)
        /// </summary>
        OnPointAddition, OnPointModification, OnPointRemoval, OnPointClear,

        /// <summary>
        /// Line update (Returns the line key (uint) associated with the event in LineNetworkElementUpdate)
        /// </summary>
        OnLineAddition, OnLineModification, OnLineRemoval, OnLineClear
    }

    public interface ILineNetworkObserverComponentAction
    {
        ComponentActionUpdate[] observerComponentActionSubscribeToEvents { get; }
        void LineNetworkComponentUpdate(object Component, ComponentAction Action);
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
