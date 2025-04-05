using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

namespace GarageGoose.ProceduralLineNetwork
{
    public class TrackerManager
    {
        private LineNetwork LineNetwork;
        /// <summary>
        /// Add or remove components used for tracking various stuff in the line network.
        /// Order is preserved when an update happens if MultithreadObservers is false
        /// </summary>
        public readonly ObservableCollection<ILineNetworkObserver> ObserverComponents = new();

        public TrackerManager(LineNetwork LineNetwork, bool MultithreadObservers)
        {
            this.LineNetwork = LineNetwork;
            ObserverComponents.CollectionChanged += ObserverComponentSetup;
        }

        //Tracks update subscriptions of observer compnents so that only subscribers of
        //an specific update type is informed which eliminates unnecessary calls
        private readonly Dictionary<UpdateType, HashSet<ILineNetworkObserver>> SpecificUpdateListener = new();

        private void ObserverComponentSetup(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Adds new observers update type subscriptions to SpecificUpdateListener
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ILineNetworkObserver Observer in e.NewItems)
                {
                    //Inherit access to parts of the line network first if the component needs it before operation
                    LineNetwork.InheritLineNetworkAccess(Observer);
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

        /// <summary>
        /// Search for eligible elements determined by specified modules.
        /// </summary>
        /// <param name="Components">List of components that will be used.</param>
        /// <param name="Intersect">Qualifies elements that is only eligible in all modules.</param>
        /// <param name="Multithread">Make thread-safe components run simultaneously.</param>
        /// <returns>Eligible elements</returns>
        public HashSet<uint> Search(ILineNetworkElementSearch[] Components, bool Intersect, bool Multithread)
        {
            HashSet<uint> EligibleElements = new();
            if (Intersect)
            {
                //Separate eligible elements by component so that it can be intersected later
                List<HashSet<uint>> EligibleElementsByComponent = new();
                foreach (ILineNetworkElementSearch Component in Components)
                {
                    //Inherit access to parts of the line network first if the component needs it before operation
                    LineNetwork.InheritLineNetworkAccess(Component);
                    EligibleElementsByComponent.Add(Component.Search());
                }

                //Sort the HashSets (eligible elements by components) from least count to most count to
                //optimize performance by eliminating many elements early on and decreasing checks
                EligibleElementsByComponent.Sort(Comparer<HashSet<uint>>.Create((a, b) => a.Count.CompareTo(b.Count)));


                //Intersect HashSets from least to most count
                EligibleElements = EligibleElementsByComponent[0];
                for (int i = 1; i < EligibleElementsByComponent.Count; i++)
                {
                    EligibleElements.Intersect(EligibleElementsByComponent[i]);
                }
            }
            else
            {
                foreach (ILineNetworkElementSearch Component in Components)
                {
                    //Inherit access to parts of the line network first if the component needs it before operation
                    LineNetwork.InheritLineNetworkAccess(Component);
                    EligibleElements.UnionWith(Component.Search());
                }
            }
            return EligibleElements;
        }
    }

    public class ElementsDatabase
    {
        public readonly ElementDict<Point> Points;
        public readonly ElementDict<Line> Lines;

        public ElementsDatabase(TrackerManager Tracker, LineNetwork LineNetwork)
        {
            Points = new(Tracker, LineNetwork, Type.Point);
            Lines = new(Tracker, LineNetwork, Type.Line);
        }

        public Type CheckElementType(uint Key)
        {
            if (Points.ContainsKey(Key)) return Type.Point;
            if (Lines.ContainsKey(Key)) return Type.Line;
            return Type.Unknown;
        }

        /// <summary>
        /// Ignore this, only used internally. Custom dictionary is used to track addition, removal, and modification of an element
        /// </summary>
        public class ElementDict<TElement> : Dictionary<uint, TElement>
        {
            TrackerManager Tracker;
            LineNetwork LineNetwork;

            private readonly UpdateType Addition;
            private readonly UpdateType ModificationBefore;
            private readonly UpdateType ModificationAfter;
            private readonly UpdateType Removal;

            public ElementDict(TrackerManager Tracker, LineNetwork LineNetwork, Type ElementType)
            {
                this.Tracker = Tracker;
                this.LineNetwork = LineNetwork;

                Addition = (ElementType == Type.Point) ? UpdateType.OnPointAddition : UpdateType.OnLineAddition;
                ModificationBefore = (ElementType == Type.Point) ? UpdateType.OnPointModificationBefore : UpdateType.OnLineModificationBefore;
                ModificationAfter = (ElementType == Type.Point) ? UpdateType.OnPointModificationAfter : UpdateType.OnLineModificationAfter;
                Removal = (ElementType == Type.Point) ? UpdateType.OnPointRemoval : UpdateType.OnLineRemoval;
            }
            public uint AddAuto(TElement NewElement)
            {
                uint Key = LineNetwork.NewKey();
                base.Add(Key, NewElement);
                ElementAddition(Key);
                return Key;
            }
            public new void Add(uint Key, TElement NewElement)
            {
                base.Add(Key, NewElement);
                ElementAddition(Key);
            }
            public new bool TryAdd(uint Key, TElement NewElement)
            {
                bool Success = base.TryAdd(Key, NewElement);
                if (Success)
                {
                    ElementAddition(Key);
                }
                return Success;
            }
            public new void Remove(uint Key)
            {
                ElementRemoval(Key);
                base.Remove(Key);
            }
            public new void Remove(uint Key, out TElement? Element)
            {
                ElementRemoval(Key);
                base.Remove(Key, out Element);
            }

            public new TElement this[uint Key]
            {
                get => base[Key];
                set
                {
                    Tracker.NotifyObservers(ModificationBefore, Key);
                    base[Key] = value;
                    Tracker.NotifyObservers(ModificationAfter, Key);
                }
            }
            private void ElementAddition(uint Key)
            {
                Tracker.NotifyObservers(Addition, Key);
            }
            private void ElementRemoval(uint Key)
            {
                Tracker.NotifyObservers(Removal, Key);
            }
        }
        public enum Type
        {
            Point, Line, Unknown
        }
    }
}
