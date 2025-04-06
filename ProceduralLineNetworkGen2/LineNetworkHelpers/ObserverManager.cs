using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

namespace GarageGoose.ProceduralLineNetwork
{
    public class ObserverManager
    {
        private LineNetwork LineNetwork;
        /// <summary>
        /// Add or remove components used for tracking various stuff in the line network.
        /// Order is preserved when an update happens if MultithreadObservers is false
        /// </summary>
        public readonly ObservableCollection<ILineNetworkObserver> ObserverComponents = new();

        //Tracks update subscriptions of observer compnents so that only subscribers of
        //an specific update type is informed which eliminates unnecessary calls
        private readonly Dictionary<UpdateType, HashSet<ILineNetworkObserver>> SpecificUpdateListener = new();

        public ObserverManager(LineNetwork LineNetwork, bool MultithreadObservers)
        {
            this.LineNetwork = LineNetwork;
            ObserverComponents.CollectionChanged += ObserverComponentSetup;
            foreach(UpdateType Type in Enum.GetValues(typeof(UpdateType)))
            {
                SpecificUpdateListener.Add(Type, new());
            }
        }

        private void ObserverComponentSetup(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Adds new observers update type subscriptions to SpecificUpdateListener
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ILineNetworkObserver Observer in e.NewItems)
                {
                    foreach (UpdateType UpdateSubscription in Observer.SubscribeToEvents)
                    {
                        SpecificUpdateListener.TryAdd(UpdateSubscription, new());
                        SpecificUpdateListener[UpdateSubscription].Add(Observer);
                    }
                }
            }

            //Removes deleted observers update type subscriptions to SpecificUpdateListener
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (ILineNetworkObserver Observer in e.OldItems)
                {
                    foreach (UpdateType UpdateSubscription in Observer.SubscribeToEvents)
                    {
                        SpecificUpdateListener[UpdateSubscription].Remove(Observer);
                    }
                }
            }
        }

        /// <summary>
        /// Use with caution (Can easily mess data tracking)! Notifies observers when an update happned
        /// </summary>
        /// <param name="AdditionalInfo">Additional information associated with the update (Check Componentinterface.Updatetype for more info)</param>
        public void NotifyObservers(UpdateType UpdateType, Object? AdditionalInfo = null)
        {
            foreach (ILineNetworkObserver Observer in SpecificUpdateListener[UpdateType])
            {
                Observer.LineNetworkChange(UpdateType, Observer);
            }
        }
    }
}
