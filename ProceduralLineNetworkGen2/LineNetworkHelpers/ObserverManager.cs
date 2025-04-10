﻿using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

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
}
