using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    /// <summary>
    /// Interface for observer components.
    /// </summary>
    public interface ILineNetObserver
    {
        /// <summary>
        /// Array of events the class is subscribed to
        /// </summary>
        public ObserverEvent[] eventSubscription { get; }

        /// <summary>
        /// The order of update between components.
        /// Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        /// Useful for when a component needs information from another component but the other component need to be updated first.
        /// 
        /// Example: <c>ObserveAngleBetweenLines</c> needs information from <c>ObserveLineAngles</c>. Therefore, <c>ObserveLineAngles</c> should have
        /// update level of 0 while <c>ObserveAngleBetweenLines</c> should have update level of 1.
        /// </summary>
        public uint UpdateLevel { get; }

        /// <summary>
        /// When true, make classes with the same update level run simultaneously.
        /// </summary>
        public bool Multithread { get; }

        /// <summary>
        /// Triggers when a point is added to the line network. Return <c>ObserverEvent.PointAdded</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the new point.</param>
        /// <param name="newPoint">The new point itself.</param>
        public void PointAdded(uint key, Point newPoint) { }

        /// <summary>
        /// Triggers when a point is modified. Return <c>ObserverEvent.PointModified</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the modified point.</param>
        /// <param name="before">The point itself before modification</param>
        /// <param name="after">The point itself after modification</param>
        public void PointModified(uint key, Point before, Point after) { }

        /// <summary>
        /// Triggers when a point is remove in the storage. Return <c>ObserverEvent.PointRemoved</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the point to be removed.</param>
        /// <param name="oldPoint">The point itself</param>
        public void PointRemoved(uint Key, Point oldPoint) { }

        /// <summary>
        /// Triggers when the point storage is cleared. Return <c>ObserverEvent.PointClear</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        public void PointClear() { }

        /// <summary>
        /// Triggers when a line is added to the line network. Return <c>ObserverEvent.LineAdded</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Key of the new line</param>
        /// <param name="newLine">The new line itself</param>
        public void LineAdded(uint key, Line newLine) { }

        /// <summary>
        /// Triggers when a line is modified in the line network. Return <c>ObserverEvent.LineModified</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="key">Key of the modified line</param>
        /// <param name="before">Line before modification</param>
        /// <param name="after">Line after modification</param>
        public void LineModified(uint key, Line before, Line after) { }

        /// <summary>
        /// Triggers when a line is removed in the line network. Return <c>ObserverEvent.LineRemoved</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the line to be removed</param>
        /// <param name="oldPoint">The line itself</param>
        public void LineRemoved(uint Key, Line oldPoint) { }

        /// <summary>
        /// Triggers when the line storage is cleared. Return <c>ObserverEvent.LineClear</c> on <c>SubscribedEvents</c> to use this.
        /// </summary>
        public void LineClear() { }
    }
}
