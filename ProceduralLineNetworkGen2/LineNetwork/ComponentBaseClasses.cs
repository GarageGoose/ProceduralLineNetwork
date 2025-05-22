using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    /// <summary>
    /// Base class for the element searcher components.
    /// </summary>
    public abstract class LineNetworkSearcher
    {

    }

    /// <summary>
    /// Base class for the line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier
    {

    }


    /// <summary>
    /// Base class for the observer components.
    /// </summary>
    public abstract class LineNetworkObserver : ILineNetworkObserverCall
    {
        /// <summary>
        /// The order of update between components.
        /// Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        /// Useful for when a component needs information from another component but the other component need to be updated first.
        /// 
        /// Example: <c>ObserveAngleBetweenLines</c> needs information from <c>ObserveLineAngles</c>. Therefore, <c>ObserveLineAngles</c> should have
        /// update level of 0 while <c>ObserveAngleBetweenLines</c> should have update level of 1.
        /// </summary>
        public readonly uint UpdateLevel;

        /// <summary>
        /// When true, make classes with the same update level run simultaneously.
        /// </summary>
        public readonly bool Multithread;

        /// <summary>
        /// Array of events the class is subscribed to
        /// </summary>
        public readonly ObserverEvent[] eventSubscription;

        /// <summary>
        /// Base class for the observer components.
        /// </summary>
        /// 
        /// <param name="updateLevel">
        /// The order of update between components (Default value 0).
        /// Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        /// Useful for when a component needs information from another component but the other component need to be updated first.
        /// 
        /// Example: <c>ObserveAngleBetweenLines</c> needs information from <c>ObserveLineAngles</c>. Therefore, <c>ObserveLineAngles</c> should have
        /// update level of 0 while <c>ObserveAngleBetweenLines</c> should have update level of 1.
        /// </param>
        /// <param name="SubscribedEvents">List of all the events the class is subscribed to. Can be left blank unless reflection dosen't work</param>
        public LineNetworkObserver(uint updateLevel, bool multithread, ObserverEvent[] SubscribedEvents)
        {
            UpdateLevel = updateLevel;
            Multithread = multithread;
            eventSubscription = SubscribedEvents;
        }


        void ILineNetworkObserverCall.NotifyPointAdded(uint key, Point newPoint) => PointAdded(key, newPoint);
        /// <summary>
        /// Triggers when a point is added to the storage. Return <c>ObserverEvent.PointAdded</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the new point.</param>
        /// <param name="newPoint">The new point itself.</param>
        protected virtual void PointAdded(uint key, Point newPoint) { }

        void ILineNetworkObserverCall.NotifyPointModified(uint key, Point before, Point after) => PointModified(key, before, after);
        /// <summary>
        /// Triggers when a point is modified in the storage. Return <c>ObserverEvent.PointModified</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the modified point.</param>
        /// <param name="before">The point itself before modification</param>
        /// <param name="after">The point itself after modification</param>
        protected virtual void PointModified(uint key, Point before, Point after) { }

        void ILineNetworkObserverCall.NotifyPointRemoved(uint Key, Point oldPoint) => PointRemoved(Key, oldPoint);
        /// <summary>
        /// Triggers when a point is remove in the storage. Return <c>ObserverEvent.PointRemoved</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the point to be removed.</param>
        /// <param name="oldPoint">The point itself</param>
        protected virtual void PointRemoved(uint Key, Point oldPoint) { }

        void ILineNetworkObserverCall.NotifyPointClear() => PointClear();
        /// <summary>
        /// Triggers when the point storage is cleared. Return <c>ObserverEvent.PointClear</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        protected virtual void PointClear() { }

        void ILineNetworkObserverCall.NotifyLineAdded(uint key, Line newLine) => LineAdded(key, newLine);
        /// <summary>
        /// Triggers when a line is added to the storage. Return <c>ObserverEvent.LineAdded</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Key of the new line</param>
        /// <param name="newLine">The new line itself</param>
        protected virtual void LineAdded(uint key, Line newLine) { }

        void ILineNetworkObserverCall.NotifyLineModified(uint key, Line before, Line after) => LineModified(key, before, after);
        /// <summary>
        /// Triggers when a line is modified in the storage. Return <c>ObserverEvent.LineModified</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Key of the modified line</param>
        /// <param name="before">Line before modification</param>
        /// <param name="after">Line after modification</param>
        protected virtual void LineModified(uint key, Line before, Line after) { }

        void ILineNetworkObserverCall.NotifyLineRemoved(uint Key, Line oldPoint) => LineRemoved(Key, oldPoint);
        /// <summary>
        /// Triggers when a line is remove in the storage. Return <c>ObserverEvent.LineRemoved</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the line to be removed</param>
        /// <param name="oldPoint">The line itself</param>
        protected virtual void LineRemoved(uint Key, Line oldPoint) { }

        void ILineNetworkObserverCall.NotifyLineClear() => LineClear();
        /// <summary>
        /// Triggers when the line storage is cleared. Return <c>ObserverEvent.LineClear</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        protected virtual void LineClear() { }
    }

    /// <summary>
    /// Used for calling updates to classes with LineNetworkObserver as base. Use carefully!
    /// </summary>
    public interface ILineNetworkObserverCall
    {
        public void NotifyPointAdded(uint key, Point newPoint);
        public void NotifyPointModified(uint key, Point before, Point after);
        public void NotifyPointRemoved(uint Key, Point oldPoint);
        public void NotifyPointClear();
        public void NotifyLineAdded(uint key, Line newLine);
        public void NotifyLineModified(uint key, Line before, Line after);
        public void NotifyLineRemoved(uint Key, Line oldPoint);
        public void NotifyLineClear();
    }

    /// <summary>
    /// Contains all the event types on a line network.
    /// </summary>
    public enum ObserverEvent
    {
        PointAdded, PointModified, PointRemoved, PointClear,

        LineAdded, LineModified, LineRemoved, LineClear
    }
}
