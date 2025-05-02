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
        public readonly ObservableCollection<LineNetworkObserver> observerComponents = new();
        private readonly ObserverManagerSubscriptionStorage database = new();
        private readonly ObserverManagerSubscriptionStorageHandler databaseHandler;
        internal readonly ObserverManagerCallHandler callHandler;
        readonly bool MultithreadObservers;
        public ObserverManager(bool multithreadObservers)
        {
            callHandler = new(database);
            databaseHandler = new(observerComponents, database, callHandler);
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
        public readonly Dictionary<ElementUpdateType, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToElementUpdate = new();

        public ObserverManagerSubscriptionStorage()
        {
            //Add all element update type since its very likely that atleast one element will subscribe to each element update type
            foreach(ElementUpdateType type in Enum.GetValues(typeof(ElementUpdateType)))
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
        //Used for tracking added or removed observers to add their subscriptions to the database.
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
            Debug.WriteLine("g");
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(LineNetworkObserver observer in e.NewItems!)
                {
                    Debug.WriteLine("s");
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
            if(observer.SubscribeToElementUpdates != null)
            {
                foreach (ElementUpdateType UpdateType in observer.SubscribeToElementUpdates)
                {
                    database.ComponentsSubscribedToElementUpdate[UpdateType].TryAdd(observer.UpdateLevel, new());
                    database.ComponentsSubscribedToElementUpdate[UpdateType][observer.UpdateLevel].Add(observer);
                }
            }
        }
        private void RemoveObserversToDb(LineNetworkObserver observer)
        {
            if (observer.SubscribeToElementUpdates != null)
            {
                foreach (ElementUpdateType UpdateType in observer.SubscribeToElementUpdates)
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
        public void PointUpdateAdd(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyPointAdded(key, point));
        public void PointUpdateModification(uint key, Point before, Point after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointModification], observer => observer.NotifyPointModified(key, before, after));
        public void PointUpdateRemove(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointRemoval], observer => observer.NotifyPointRemoved(key, point));
        public void PointUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointClear], observer => observer.NotifyPointClear());
        public void LineUpdateAdd(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnLineAddition], observer => observer.NotifyLineAdded(key, line));
        public void LineUpdateModification(uint key, Line before, Line after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnLineModification], observer => observer.NotifyLineModified(key, before, after));
        public void LineUpdateRemove(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnLineRemoval], observer => observer.NotifyLineRemoved(key, line));
        public void LineUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnLineClear], observer => observer.NotifyLineClear());

        private void CallHandler(SortedList<uint, HashSet<LineNetworkObserver>> list, Action<LineNetworkObserver> CallInstruction)
        {
            Debug.WriteLine("o");
            foreach (HashSet<LineNetworkObserver> observersAtSameUpdateLevel in list.Values)
            {
                Debug.WriteLine("found obs, 1");
                foreach(LineNetworkObserver observer in observersAtSameUpdateLevel)
                {
                    Debug.WriteLine("found obs, 2: " + observer + ", " + CallInstruction);
                    CallInstruction(observer);
                }
            }
        }
    }
}
