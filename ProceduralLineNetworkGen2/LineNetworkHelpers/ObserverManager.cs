using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    /// <summary>
    /// This class handles 
    /// </summary>
    public class ObserverManager
    {
        private LineNetwork LineNetwork;
        /// <summary>
        /// Add or remove components used for tracking various stuff in the line network.
        /// Order is preserved when an update happens if MultithreadObservers is false
        /// </summary>
        public readonly ObservableCollection<object> ObserverComponents = new();

        //Tracks update subscriptions of observer compnents so that only subscribers of
        //an specific update type is informed which eliminates unnecessary calls
        private readonly Dictionary<ElementUpdateType, HashSet<ILineNetworkObserverElementSubscribe>> SpecificElementUpdateListener = new();
        private readonly Dictionary<ComponentActionUpdate, HashSet<ILineNetworkObserverComponentAction>> SpecificComponentActionUpdateListener = new();

        public ObserverManager(LineNetwork LineNetwork, bool MultithreadObservers)
        {
            this.LineNetwork = LineNetwork;
            ObserverComponents.CollectionChanged += ObserverComponentSetup;
            foreach(ElementUpdateType Type in Enum.GetValues(typeof(ElementUpdateType)))
            {
                SpecificElementUpdateListener.Add(Type, new());
            }
        }

        private void ObserverComponentSetup(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Adds new observers update type subscriptions to SpecificElementUpdateListener
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (object Observer in e.NewItems)
                {
                    if(Observer is ILineNetworkObserverElementSubscribe)
                    {
                        foreach (ElementUpdateType UpdateSubscription in ((ILineNetworkObserverElementSubscribe)Observer).observerElementSubscribeToEvents)
                        {
                            SpecificElementUpdateListener.TryAdd(UpdateSubscription, new());
                            SpecificElementUpdateListener[UpdateSubscription].Add((ILineNetworkObserverElementSubscribe)Observer);
                        }
                    }
                    else if(Observer is ILineNetworkObserverComponentAction)
                    {
                        foreach(ComponentActionUpdate componentActionUpdate in ((ILineNetworkObserverComponentAction)Observer).observerComponentActionSubscribeToEvents)
                        {
                            SpecificComponentActionUpdateListener.TryAdd(componentActionUpdate, new());
                            SpecificComponentActionUpdateListener[componentActionUpdate].Add((ILineNetworkObserverComponentAction)Observer);
                        }
                    }
                }
            }

            //Removes deleted observers update type subscriptions to SpecificElementUpdateListener
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (object Observer in e.OldItems)
                {
                    if(Observer is ILineNetworkObserverElementSubscribe)
                    {
                        foreach (ElementUpdateType UpdateSubscription in ((ILineNetworkObserverElementSubscribe)Observer).observerElementSubscribeToEvents)
                        {
                            SpecificElementUpdateListener[UpdateSubscription].Remove((ILineNetworkObserverElementSubscribe)Observer);
                        }
                    }
                    else if (Observer is ILineNetworkObserverComponentAction)
                    {
                        foreach (ComponentActionUpdate componentActionUpdate in ((ILineNetworkObserverComponentAction)Observer).observerComponentActionSubscribeToEvents)
                        {
                            SpecificComponentActionUpdateListener[componentActionUpdate].Remove((ILineNetworkObserverComponentAction)Observer);
                            if (SpecificComponentActionUpdateListener[componentActionUpdate].Count == 0) SpecificComponentActionUpdateListener.Remove(componentActionUpdate);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use with caution (Can easily mess data tracking)! Notifies observers when an element update happned
        /// </summary>
        /// <param name="AdditionalInfo">Additional information associated with the update (Check Componentinterface.Updatetype for more info)</param>
        public void ElementAddOrRemoveNotifyObservers(ElementUpdateType UpdateType, uint key)
        {
            foreach (ILineNetworkObserverElementSubscribe Observer in SpecificElementUpdateListener[UpdateType])
            {
                ObserverActionNotifyObservers(Observer, ComponentAction.Start);
                ((ILineNetworkObserverElementAddedOrRemoved)Observer).LineNetworkElementAddedOrRemoved(UpdateType, key);
                ObserverActionNotifyObservers(Observer, ComponentAction.Finished);
            }
        }
        public void ElementModificationNotifyObservers(ElementUpdateType UpdateType, uint key, object OldElement, object NewElement)
        {
            foreach (ILineNetworkObserverElementSubscribe Observer in SpecificElementUpdateListener[UpdateType])
            {
                ObserverActionNotifyObservers(Observer, ComponentAction.Start);
                ((ILineNetworkObserverElementClear)Observer).LineNetworkElementClear(UpdateType);
                ObserverActionNotifyObservers(Observer, ComponentAction.Finished);
            }
        }
        public void ElementClearNotifyObservers(ElementUpdateType UpdateType)
        {
            foreach (ILineNetworkObserverElementSubscribe Observer in SpecificElementUpdateListener[UpdateType])
            {
                ObserverActionNotifyObservers(Observer, ComponentAction.Start);
                ((ILineNetworkObserverElementClear)Observer).LineNetworkElementClear(UpdateType);
                ObserverActionNotifyObservers(Observer, ComponentAction.Finished);
            }
        }

        /// <summary>
        /// Use with caution (Can easily mess data tracking)! Notifies observers when an external component is used
        /// </summary>
        public void ObserverActionNotifyObservers(object Component, ComponentAction Action)
        {
            ComponentActionUpdate componentActionUpdate = new(Component.GetType(), Action);
            if (!SpecificComponentActionUpdateListener.ContainsKey(componentActionUpdate)) return;
            foreach(ILineNetworkObserverComponentAction Observer in SpecificComponentActionUpdateListener[componentActionUpdate])
            {
                ObserverActionNotifyObservers(Component, ComponentAction.Start);
                Observer.LineNetworkComponentUpdate(Component, Action);
                ObserverActionNotifyObservers(Component, ComponentAction.Finished);
            }
        }
    }

    public class ObserverManagerDatabase
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

        //Component update
        //Components separated by the object to track, sorted by update level, and filtered by specific event subscribed to.
        //Repetition is present here to avoid additional code complexity down the line.
        //Dictionary key: instance of a component to track
        //SortedList key: Component's update level (See comment above)
        //Value: Hashset of observsers subscribed to the specific event.
        public readonly Dictionary<object, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToComponentStart = new();
        public readonly Dictionary<object, SortedList<uint, HashSet<LineNetworkObserver>>> ComponentsSubscribedToComponentFinished = new();
    }

    internal class ObserverManagerDatabaseHandler
    {
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

    internal class ObserverManagerCallHandler
    {
        ObserverManagerDatabase database;
        internal ObserverManagerCallHandler(ObserverManagerDatabase database)
        {
            this.database = database;
        }

        public void PointUpdateAdd(uint key, Point point)
        {

        }
        public void PointUpdateModification(uint key, Point before, Point after)
        {

        }
        public void PointUpdateRemove(uint key, Point point)
        {

        }
        public void PointUpdateClear()
        {

        }
        public void LineUpdateAdd(uint key, Line line)
        {

        }
        public void LineUpdateModification(uint key, Line before, Line after)
        {

        }
        public void LineUpdateRemove(uint key, Line line)
        {

        }
        public void LineUpdateClear()
        {

        }

        private void CallHandler(SortedList<uint, HashSet<LineNetworkObserver>> list)
        {
            foreach (HashSet<LineNetworkObserver> observersAtSameUpdateLevel in list.Values)
            {
                foreach(LineNetworkObserver observer in observersAtSameUpdateLevel)
                {

                }
            }
        }
    }
}
