using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

namespace GarageGoose.ProceduralLineNetwork
{
    public class TrackerManager
    {
        private LineNetwork LineNetwork;
        /// <summary>
        /// Components used for tracking various stuff in the line network.
        /// </summary>
        public ObservableCollection<ILineNetworkObserver> ObserverComponents = new();

        private readonly Dictionary<UpdateType, HashSet<ILineNetworkObserver>> SpecificUpdateListener = new();
        public TrackerManager(LineNetwork LineNetwork, bool MultithreadObservers)
        {
            this.LineNetwork = LineNetwork;
            ObserverComponents.CollectionChanged += ObserverComponentSetup;
        }

        private void ObserverComponentSetup(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ILineNetworkObserver Observer in e.NewItems)
                {
                    foreach (UpdateType UpdateSubscription in Observer.SubscribeToEvents())
                    {
                        SpecificUpdateListener.TryAdd(UpdateSubscription, new());
                        SpecificUpdateListener[UpdateSubscription].Add(Observer);
                    }
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (ILineNetworkObserver Observer in e.OldItems)
                {
                    foreach (UpdateType UpdateSubscription in Observer.SubscribeToEvents())
                    {
                        SpecificUpdateListener[UpdateSubscription].Remove(Observer);
                    }
                }
            }
        }

        /// <summary>
        /// Use with caution (Can easily mess data tracking)! Notifies observers when an update happned
        /// </summary>
        /// <param name="UpdateType"></param>
        /// <param name="AdditionalInfo"></param>
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
        /// <returns></returns>
        public HashSet<uint> Search(ILineNetworkElementSearch[] Components, bool Intersect, bool Multithread)
        {
            List<HashSet<uint>> EligibleElements = new();
            foreach (ILineNetworkElementSearch Component in Components)
            {
                EligibleElements.Add(Component.Search());
            }
            HashSet<uint> FinalEligibleElements;
            if (Intersect)
            {
                EligibleElements.Sort(Comparer<HashSet<uint>>.Create((a, b) => a.Count.CompareTo(b.Count)));
                FinalEligibleElements = EligibleElements[0];
                for (int i = 1; i < EligibleElements.Count; i++)
                {
                    FinalEligibleElements.Intersect(EligibleElements[i]);
                }
            }
            else
            {
                FinalEligibleElements = EligibleElements[0];
                for (int i = 1; i < EligibleElements.Count; i++)
                {
                    FinalEligibleElements.UnionWith(EligibleElements[i]);
                }
            }
            return FinalEligibleElements;
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
