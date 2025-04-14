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
        public readonly ObservableCollection<LineNetworkObserver> observerComponents = new();
        private readonly ObserverManagerDatabase database = new();
        private readonly ObserverManagerDatabaseHandler databaseHandler;
        internal readonly ObserverManagerCallHandler callHandler;
        readonly bool MultithreadObservers;
        public ObserverManager(bool multithreadObservers)
        {
            databaseHandler = new(observerComponents, database);
            callHandler = new(database);
            MultithreadObservers = multithreadObservers;
        }
    }

    /// <summary>
    /// Tracks the subscriptions of components.
    /// </summary>
    internal class ObserverManagerDatabase
    {
        //Context: Update level
        // The order of update between components (Default value 0).
        // Components with lower level updates first before the higher ones. No defenite update order is imposed for the components with the same update level.
        // Useful for when a component needs information from another component but the other component need to be updated first.
        // 
        // Example: <c>TrackAngleBetweenLines</c> needs information from <c>TrackLineAngles</c>. Therefore, <c>TrackLineAngles</c> should have
        // update level of 0 while <c>TrackAngleBetweenLines</c> should have update level of 1.

        //Element update
        //Components sorted by update level, filtered by specific event subscribed to.
        //Key: Update level (See comment above)
        //Value: Hashset of observsers subscribed to the specific event.
        public readonly Dictionary<ElementUpdateType, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToElementUpdate = new();

        public ObserverManagerDatabase()
        {
            //Add all element update type since its very likely that atleast one element will subscribe to each element update type
            foreach(ElementUpdateType type in Enum.GetValues(typeof(ElementUpdateType)))
            {
                ComponentsSubscribedToElementUpdate.Add(type, new());
            }
        }

        //Component update
        //Components separated by the object to track, sorted by update level, and filtered by specific event subscribed to.
        //Repetition is present here to avoid additional code complexity down the line.
        //Dictionary key: instance of a component to track
        //SortedList key: Component's update level (See comment above)
        //Value: Hashset of observsers subscribed to the specific event.
        public readonly Dictionary<object, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToComponentStart = new();
        public readonly Dictionary<object, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToComponentFinished = new();
        //In this case, a target component will be deleted when none is subscribed to it in these two dictionary since in most cases none will be subscribed
        //to a specific component.
    }

    /// <summary>
    /// Updates <c>ObserverManagerDatabase</c> when a component is added or removed.
    /// </summary>
    internal class ObserverManagerDatabaseHandler
    {
        //Used for tracking added or removed observers to add their subscriptions to the database.
        private readonly ObservableCollection<LineNetworkObserver> observers;


        private readonly ObserverManagerDatabase database;
        public ObserverManagerDatabaseHandler(ObservableCollection<LineNetworkObserver> observers, ObserverManagerDatabase database)
        {
            this.observers = observers;
            observers.CollectionChanged += CollectionChanged;
            this.database = database;
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
                foreach (LineNetworkObserver observer in e.NewItems!)
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
            if(observer.SubscribeToComponentStart != null)
            {
                foreach(object component in observer.SubscribeToComponentStart)
                {
                    database.ComponentsSubscribedToComponentStart.TryAdd(component, new());
                    database.ComponentsSubscribedToComponentStart[component].TryAdd(observer.UpdateLevel, new());
                    database.ComponentsSubscribedToComponentStart[component][observer.UpdateLevel].Add(observer);
                }
            }
            if (observer.SubscribeToComponentFinished != null)
            {
                foreach (object component in observer.SubscribeToComponentFinished)
                {
                    database.ComponentsSubscribedToComponentFinished.TryAdd(component, new());
                    database.ComponentsSubscribedToComponentFinished[component].TryAdd(observer.UpdateLevel, new());
                    database.ComponentsSubscribedToComponentFinished[component][observer.UpdateLevel].Add(observer);
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
            if (observer.SubscribeToComponentStart != null)
            {
                foreach (object component in observer.SubscribeToComponentStart)
                {
                    database.ComponentsSubscribedToComponentStart[component][observer.UpdateLevel].Remove(observer);
                    if(database.ComponentsSubscribedToComponentStart[component][observer.UpdateLevel].Count == 0)
                    {
                        database.ComponentsSubscribedToComponentStart[component].Remove(observer.UpdateLevel);
                    }
                    if (database.ComponentsSubscribedToComponentStart[component].Count == 0)
                    {
                        database.ComponentsSubscribedToComponentStart.Remove(component);
                    }
                }
            }
            if (observer.SubscribeToComponentFinished != null)
            {
                foreach (object component in observer.SubscribeToComponentFinished)
                {
                    database.ComponentsSubscribedToComponentFinished[component][observer.UpdateLevel].Remove(observer);
                    if (database.ComponentsSubscribedToComponentFinished[component][observer.UpdateLevel].Count == 0)
                    {
                        database.ComponentsSubscribedToComponentFinished[component].Remove(observer.UpdateLevel);
                    }
                    if (database.ComponentsSubscribedToComponentFinished[component].Count == 0)
                    {
                        database.ComponentsSubscribedToComponentFinished.Remove(component);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles distributing an update call to the subscribed components via <c>ObserverManagerDatabase</c>.
    /// </summary>
    internal class ObserverManagerCallHandler
    {
        ObserverManagerDatabase database;
        internal ObserverManagerCallHandler(ObserverManagerDatabase database)
        {
            this.database = database;
        }
        //Element update
        public void PointUpdateAdd(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyPointAdded(key, point));
        public void PointUpdateModification(uint key, Point before, Point after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyPointModified(key, before, after));
        public void PointUpdateRemove(uint key, Point point) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyPointRemoved(key, point));
        public void PointUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyPointClear());
        public void LineUpdateAdd(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyLineAdded(key, line));
        public void LineUpdateModification(uint key, Line before, Line after) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyLineModified(key, before, after));
        public void LineUpdateRemove(uint key, Line line) => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyLineRemoved(key, line));
        public void LineUpdateClear() => CallHandler(database.ComponentsSubscribedToElementUpdate[ElementUpdateType.OnPointAddition], observer => observer.NotifyLineClear());

        //Component update
        public void ComponentStartUpdate(object component) => CallHandler(database.ComponentsSubscribedToComponentStart[component], observer => observer.NotifyComponentStart(component));
        public void ComponentFinishedUpdate(object component) => CallHandler(database.ComponentsSubscribedToComponentStart[component], observer => observer.NotifyComponentFinished(component));

        private void CallHandler(SortedList<uint, HashSet<LineNetworkObserver>> list, Action<LineNetworkObserver> CallInstruction)
        {
            foreach (HashSet<LineNetworkObserver> observersAtSameUpdateLevel in list.Values)
            {
                foreach(LineNetworkObserver observer in observersAtSameUpdateLevel)
                {
                    ComponentStartUpdate(observer);
                    CallInstruction(observer);
                    ComponentFinishedUpdate(observer);
                }
            }
        }

    }
}
