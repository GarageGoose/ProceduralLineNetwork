using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
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

    /// <summary>
    /// Base class for the observer components.
    /// </summary>
    public abstract class LineNetworkObserver
    {
        /// <summary>
        /// The order of update between components (Default value 0).
        /// Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        /// Useful for when a component needs information from another component but the other component need to be updated first.
        /// 
        /// Example: <c>TrackAngleBetweenLines</c> needs information from <c>TrackLineAngles</c>. Therefore, <c>TrackLineAngles</c> should have
        /// update level of 0 while <c>TrackAngleBetweenLines</c> should have update level of 1.
        /// </summary>
        public readonly uint UpdateLevel;

        /// <summary>
        /// When true, make classes with the same update level run simultaneously.
        /// </summary>
        public readonly bool Multithread;

        /// <summary>
        /// Subscribe to different element updates (ex. line added, point modified, and so on...). Subscribing is a must for related methods to be called.
        /// </summary>
        public readonly ElementUpdateType[]? SubscribeToElementUpdates;

        /// <summary>
        /// Subscribe a specific component to track when its going to be used in the line network.
        /// </summary>
        public readonly object[]? SubscribeToComponentStart;

        /// <summary>
        /// Subscribe a specific component to track when the component is finished doint its task in the line network.
        /// </summary>
        public readonly object[]? SubscribeToComponentFinished;

        /// <summary>
        /// Base class for the observer components.
        /// </summary>
        /// 
        /// <param name="updateLevel">
        /// The order of update between components (Default value 0).
        /// Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        /// Useful for when a component needs information from another component but the other component need to be updated first.
        /// 
        /// Example: <c>TrackAngleBetweenLines</c> needs information from <c>TrackLineAngles</c>. Therefore, <c>TrackLineAngles</c> should have
        /// update level of 0 while <c>TrackAngleBetweenLines</c> should have update level of 1.
        /// </param>
        /// 
        /// <param name="subscribeToElementsUpdate">
        /// Subscribe to different element updates (ex. line added, point modified, and so on...). Subscribing is a must for related methods to be called.
        /// </param>
        /// 
        /// <param name="subscribeToComponentStart">
        /// Subscribe a specific component to track when its going to be used in the line network.
        /// </param>
        /// 
        /// <param name="subscribeToComponentFinished">
        /// Subscribe a specific component to track when the component is finished doint its task in the line network.
        /// </param>
        public LineNetworkObserver(uint updateLevel, ElementUpdateType[]? subscribeToElementsUpdate, object[]? subscribeToComponentStart, object[]? subscribeToComponentFinished)
        {
            UpdateLevel = updateLevel;
            SubscribeToElementUpdates = subscribeToElementsUpdate;
            SubscribeToComponentStart = subscribeToComponentStart;
            SubscribeToComponentFinished = subscribeToComponentFinished;
        }



        public void NotifyPointAdded(uint key, Point newPoint) => PointAdded(key, newPoint);
        /// <summary>
        /// Triggers when a point is added to the database. Return <c>ElementUpdateType.OnPointAddition</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the new point.</param>
        /// <param name="newPoint">The new point itself.</param>
        protected virtual void PointAdded(uint key, Point newPoint) { }

        public void NotifyPointModified(uint key, Point before, Point after) => PointModified(key, before, after);
        /// <summary>
        /// Triggers when a point is modified in the database. Return <c>ElementUpdateType.OnPointModification</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="key">Point key of the modified point.</param>
        /// <param name="before">The point itself before modification</param>
        /// <param name="after">The point itself after modification</param>
        protected virtual void PointModified(uint key, Point before, Point after) { }

        public void NotifyPointRemoved(uint Key, Point oldPoint) => PointRemoved(Key, oldPoint);
        /// <summary>
        /// Triggers when a point is remove in the database. Return <c>ElementUpdateType.OnPointRemoval</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the point to be removed.</param>
        /// <param name="oldPoint">The point itself</param>
        protected virtual void PointRemoved(uint Key, Point oldPoint) { }

        public void NotifyPointClear() => PointClear();
        /// <summary>
        /// Triggers when the point database is cleared. Return <c>ElementUpdateType.OnPointClear</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        protected virtual void PointClear() { }

        public void NotifyLineAdded(uint key, Line newLine) => LineAdded(key, newLine);
        /// <summary>
        /// Triggers when a line is added to the database. Return <c>ElementUpdateType.OnLineAddition</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="key">Key of the new line</param>
        /// <param name="newLine">The new line itself</param>
        protected virtual void LineAdded(uint key, Line newLine) { }

        public void NotifyLineModified(uint key, Line before, Line after) => LineModified(key, before, after);
        /// <summary>
        /// Triggers when a line is modified in the database. Return <c>ElementUpdateType.OnLineModification</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="key">Key of the modified line</param>
        /// <param name="before">Line before modification</param>
        /// <param name="after">Line after modification</param>
        protected virtual void LineModified(uint key, Line before, Line after) { }

        public void NotifyLineRemoved(uint Key, Line oldPoint) => LineRemoved(Key, oldPoint);
        /// <summary>
        /// Triggers when a line is remove in the database. Return <c>ElementUpdateType.OnLineRemoval</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        /// <param name="Key">Key of the line to be removed</param>
        /// <param name="oldPoint">The line itself</param>
        protected virtual void LineRemoved(uint Key, Line oldPoint) { }

        public void NotifyLineClear() => LineClear();
        /// <summary>
        /// Triggers when the line database is cleared. Return <c>ElementUpdateType.OnLineClear</c> on <c>SubscribeToElementUpdates</c> to use this.
        /// </summary>
        protected virtual void LineClear() { }

        public void NotifyComponentStart(object Component) => ComponentStart(Component);
        /// <summary>
        /// Triggers when a component is about to be used. Return the target component on <c>SubscribeToComponentStart</c> to track it.
        /// Example: Triggers just before <c>TrackLineAngles</c> is notified when a line is added to the database, assuming it is tracking it.
        /// Note that this isn't going to trigger when the component is used outside the line network. 
        /// </summary>
        protected virtual void ComponentStart(object Component) { }

        public void NotifyComponentFinished(object Component) => ComponentFinished(Component);
        /// <summary>
        /// Triggers when a component is used. Return the target component on <c>SubscribeToComponentStart</c> to track it.
        /// Example: Triggers after <c>TrackLineAngles</c> is notified when a line is added to the database and done doing its thing, assuming it is tracking it.
        /// Note that this isn't going to trigger when the component is used outside the line network. 
        /// </summary>
        protected virtual void ComponentFinished(object Component) { }
    }
}
