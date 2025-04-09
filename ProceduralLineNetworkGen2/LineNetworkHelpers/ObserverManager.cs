using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
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
        private readonly Dictionary<ElementUpdateType, HashSet<ILineNetworkObserverElement>> SpecificElementUpdateListener = new();
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
                    if(Observer is ILineNetworkObserverElement)
                    {
                        foreach (ElementUpdateType UpdateSubscription in ((ILineNetworkObserverElement)Observer).observerElementSubscribeToEvents)
                        {
                            SpecificElementUpdateListener.TryAdd(UpdateSubscription, new());
                            SpecificElementUpdateListener[UpdateSubscription].Add((ILineNetworkObserverElement)Observer);
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
                    if(Observer is ILineNetworkObserverElement)
                    {
                        foreach (ElementUpdateType UpdateSubscription in ((ILineNetworkObserverElement)Observer).observerElementSubscribeToEvents)
                        {
                            SpecificElementUpdateListener[UpdateSubscription].Remove((ILineNetworkObserverElement)Observer);
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
        public void ElementUpdateNotifyObservers(ElementUpdateType UpdateType, Object? AdditionalInfo = null)
        {
            foreach (ILineNetworkObserverElement Observer in SpecificElementUpdateListener[UpdateType])
            {
                Observer.LineNetworkElementUpdate(UpdateType, Observer);
            }
        }

        /// <summary>
        /// Use with caution (Can easily mess data tracking)! Notifies observers when an external component is used
        /// </summary>
        public void ObserverActionNotifyObservers(ComponentActionUpdate UpdateType)
        {
            if (!SpecificComponentActionUpdateListener.ContainsKey(UpdateType)) return;
            foreach(ILineNetworkObserverComponentAction Observer in SpecificComponentActionUpdateListener[UpdateType])
            {
                Observer.LineNetworkComponentUpdate(UpdateType);
            }
        }
    }
}
