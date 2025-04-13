using GarageGoose.ProceduralLineNetwork.Component.Interface;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    public class SearcherManager
    {
        public readonly bool Multithread;
        private ObserverManager observer;
        public SearcherManager(bool Multithread, ObserverManager observer)
        {
            this.Multithread = Multithread;
            this.observer = observer;
        }
        /// <summary>
        /// Search for eligible elements determined by specified components.
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
                    observer.callHandler.ComponentStartUpdate(Component);
                    EligibleElementsByComponent.Add(Component.Search());
                    observer.callHandler.ComponentFinishedUpdate(Component);
                }

                //Sort the HashSets (eligible elements by components) from least count to most count to
                //optimize performance by eliminating many elements early on and decreasing checks
                EligibleElementsByComponent.Sort(Comparer<HashSet<uint>>.Create((a, b) => a.Count.CompareTo(b.Count)));


                //Intersect HashSets from least to most count because of the reason above
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
                    observer.callHandler.ComponentStartUpdate(Component);
                    EligibleElements.UnionWith(Component.Search());
                    observer.callHandler.ComponentFinishedUpdate(Component);
                }
            }
            return EligibleElements;
        }
    }
}
