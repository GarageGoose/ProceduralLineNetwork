using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;


namespace GarageGoose.ProceduralLineNetwork.Manager
{
    /// <summary>
    /// This class handles observer components which tracks the line network.
    /// </summary>
    public class ObserverManager
    {
        /// <summary>
        /// Observers that listens to line network updates.
        /// </summary>
        public readonly ObservableCollection<ILineNetObserver> linkedObservers = new();

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
    /// Contains all the event types on a line network.
    /// </summary>
    public enum ObserverEvent
    {
        PointAdded, PointModified, PointRemoved, PointClear,

        LineAdded, LineModified, LineRemoved, LineClear
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
        public readonly Dictionary<ObserverEvent, SortedList<uint, HashSet<ILineNetObserver>>> ComponentsSubscribedToElementUpdate = new();

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
        private readonly ObservableCollection<ILineNetObserver> observers;

        private readonly ObserverManagerSubscriptionStorage database;

        private readonly ObserverManagerCallHandler callHandle;
        public ObserverManagerSubscriptionStorageHandler(ObservableCollection<ILineNetObserver> observers, ObserverManagerSubscriptionStorage database, ObserverManagerCallHandler callHandle)
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
                foreach(ILineNetObserver observer in e.NewItems!)
                {
                    AddObserversToDb(observer);

                }
            }
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (ILineNetObserver observer in e.OldItems!)
                {
                    RemoveObserversToDb(observer);
                }
            }
        }

        private void AddObserversToDb(ILineNetObserver observer)
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
        private void RemoveObserversToDb(ILineNetObserver observer)
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

        public void PointUpdateAdd(uint key, Point point)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointAdded].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.PointAdded(key, point);
                }
            }
        }

        public void PointUpdateModification(uint key, Point before, Point after)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointModified].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.PointModified(key, before, after);
                }
            }
        }

        public void PointUpdateRemove(uint key, Point point)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointRemoved].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.PointRemoved(key, point);
                }
            }
        }

        public void PointUpdateClear()
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.PointClear].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.PointClear();
                }
            }
        }

        public void LineUpdateAdd(uint key, Line line)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineAdded].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.LineAdded(key, line);
                }
            }
        }

        public void LineUpdateModification(uint key, Line before, Line after)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineModified].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.LineModified(key, before, after);
                }
            }
        }

        public void LineUpdateRemove(uint key, Line line)
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineRemoved].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.LineRemoved(key, line);
                }
            }
        }

        public void LineUpdateClear()
        {
            foreach (HashSet<ILineNetObserver> observersAtSameUpdateLevel in database.ComponentsSubscribedToElementUpdate[ObserverEvent.LineClear].Values)
            {
                foreach (ILineNetObserver observer in observersAtSameUpdateLevel)
                {
                    observer.LineClear();
                }
            }
        }
    }
}
