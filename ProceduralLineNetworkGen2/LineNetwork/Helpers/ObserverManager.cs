using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    /// <summary>
    /// This class handles observer components which tracks the line network.
    /// </summary>
    public class ObserverManager
    {
        public readonly ObservableCollection<LineNetworkObserver> linkedObservers = new();
        private readonly ObserverManagerSubscriptionStorage storage = new();
        private readonly ObserverManagerSubscriptionStorageHandler storageHandler;
        internal readonly ObserverManagerCallHandler callHandler;
        readonly bool MultithreadObservers;
        public ObserverManager(bool multithreadObservers)
        {
            callHandler = new(storage);
            storageHandler = new(linkedObservers, storage, callHandler);
            MultithreadObservers = multithreadObservers;
        }
    }

    /// <summary>
    /// Tracks the subscriptions of components.
    /// </summary>
    internal class ObserverManagerSubscriptionStorage
    {
        //Context: Update level
        // The order of update between components (Default value 0).
        // Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        // Useful for when a component needs information from another component but the other component need to be updated first.
        // 
        // Example: <c>ObserveAngleBetweenLines</c> needs information from <c>ObserveLineAngles</c>. Therefore, <c>ObserveLineAngles</c> should have
        // update level of 0 while <c>ObserveAngleBetweenLines</c> should have update level of 1.

        //Element update
        //Components sorted by update level, filtered by specific event subscribed to.
        //Key: Update level (See comment above)
        //Value: Hashset of observsers subscribed to the specific event.
        public readonly Dictionary<ObserverEvent, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToElementUpdate = new();

        public ObserverManagerSubscriptionStorage()
        {
            foreach(ObserverEvent type in Enum.GetValues(typeof(ObserverEvent)))
            {
                ComponentsSubscribedToElementUpdate.Add(type, new());
            }
        }
    }

    /// <summary>
    /// Updates <c>ObserverManagerSubscriptionStorage</c> when a component is added or removed.
    /// </summary>
    internal class ObserverManagerSubscriptionStorageHandler
    {
        //Used for tracking added or removed observers to add their subscriptions to the storage.
        private readonly ObservableCollection<LineNetworkObserver> observers;

        private readonly ObserverManagerSubscriptionStorage database;

        private readonly ObserverManagerCallHandler callHandle;
        public ObserverManagerSubscriptionStorageHandler(ObservableCollection<LineNetworkObserver> observers, ObserverManagerSubscriptionStorage database, ObserverManagerCallHandler callHandle)
        {
            this.observers = observers;
            observers.CollectionChanged += CollectionChanged;
            this.database = database;
            this.callHandle = callHandle;
        }

        private void CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(LineNetworkObserver observer in e.NewItems!)
                {
                    AddObserversToDb(observer);

                }
            }
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (LineNetworkObserver observer in e.OldItems!)
                {
                    RemoveObserversToDb(observer);
                }
            }
        }

        private void AddObserversToDb(LineNetworkObserver observer)
        {
            if(observer.eventSubscription != null)
            {
                foreach (ObserverEvent UpdateType in observer.eventSubscription)
                {
                    database.ComponentsSubscribedToElementUpdate[UpdateType].TryAdd(observer.UpdateLevel, new());
                    database.ComponentsSubscribedToElementUpdate[UpdateType][observer.UpdateLevel].Add(observer);
                }
            }
        }
        private void RemoveObserversToDb(LineNetworkObserver observer)
        {
            if (observer.eventSubscription != null)
            {
                foreach (ObserverEvent UpdateType in observer.eventSubscription)
                {
                    database.ComponentsSubscribedToElementUpdate[UpdateType][observer.UpdateLevel].Remove(observer);
                    if (database.ComponentsSubscribedToElementUpdate[UpdateType][observer.UpdateLevel].Count == 0) 
                    {
                        database.ComponentsSubscribedToElementUpdate[UpdateType].Remove(observer.UpdateLevel);
                    } 
                }
            }
        }
    }

    /// <summary>
    /// Handles distributing an update call to the subscribed components via <c>ObserverManagerSubscriptionStorage</c>.
    /// </summary>
    internal class ObserverManagerCallHandler
    {
        ObserverManagerSubscriptionStorage database;
        internal ObserverManagerCallHandler(ObserverManagerSubscriptionStorage database)
        {
            this.database = database;
        }
        //Element update
        public void PointUpdateAdd(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointAdded], observer => observer.NotifyPointAdded(key, point));
        public void PointUpdateModification(uint key, Point before, Point after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointModified], observer => observer.NotifyPointModified(key, before, after));
        public void PointUpdateRemove(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointRemoved], observer => observer.NotifyPointRemoved(key, point));
        public void PointUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointClear], observer => observer.NotifyPointClear());
        public void LineUpdateAdd(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineAdded], observer => observer.NotifyLineAdded(key, line));
        public void LineUpdateModification(uint key, Line before, Line after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineModified], observer => observer.NotifyLineModified(key, before, after));
        public void LineUpdateRemove(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineRemoved], observer => observer.NotifyLineRemoved(key, line));
        public void LineUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineClear], observer => observer.NotifyLineClear());

        private void CallHandler(SortedList<uint, HashSet<LineNetworkObserver>> list, Action<ILineNetworkObserverCall> CallInstruction)
        {
            foreach (HashSet<LineNetworkObserver> observersAtSameUpdateLevel in list.Values)
            {
                foreach(LineNetworkObserver observer in observersAtSameUpdateLevel)
                {
                    CallInstruction(observer);
                }
            }
        }
    }
}
