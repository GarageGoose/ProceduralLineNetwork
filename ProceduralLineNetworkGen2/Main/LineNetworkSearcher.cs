using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GarageGoose.ProceduralLineNetwork.Manager;
using GarageGoose.ProceduralLineNetwork.Component.Interface;
using System.Reflection.Metadata.Ecma335;
namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetworkSearcher
    {
        private HashSet<ILineNetworkSearcherContainer> SearcherComponents = new();

        public void Reset() => SearcherComponents.Clear();

        public void AddComponent(LineNetworkElementSearcher searcherObject) => SearcherComponents.Add(new SearcherComponentContainer(searcherObject));
        public void AddComponent<T>(LineNetworkElementSearcher<T> searcherObject, T param) => SearcherComponents.Add(new SearcherComponentContainer<T>(param, searcherObject));
        public void AddComponent<T1, T2>(LineNetworkElementSearcher<T1, T2> searcherObject, T1 param1, T2 param2) => SearcherComponents.Add(new SearcherComponentContainer<T1, T2>(param1, param2, searcherObject));
        public void AddComponent<T1, T2, T3>(LineNetworkElementSearcher<T1, T2, T3> searcherObject, T1 param1, T2 param2, T3 param3) => SearcherComponents.Add(new SearcherComponentContainer<T1, T2, T3>(param1, param2, param3, searcherObject));
        public void AddComponent<T1, T2, T3, T4>(LineNetworkElementSearcher<T1, T2, T3, T4> searcherObject, T1 param1, T2 param2, T3 param3, T4 param4) => SearcherComponents.Add(new SearcherComponentContainer<T1, T2, T3, T4>(param1, param2, param3, param4, searcherObject));
        public void AddComponent<T1, T2, T3, T4, T5>(LineNetworkElementSearcher<T1, T2, T3, T4, T5> searcherObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => SearcherComponents.Add(new SearcherComponentContainer<T1, T2, T3, T4, T5>(param1, param2, param3, param4, param5, searcherObject));
        public void AddComponent<T1, T2, T3, T4, T5, T6>(LineNetworkElementSearcher<T1, T2, T3, T4, T5, T6> searcherObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) => SearcherComponents.Add(new SearcherComponentContainer<T1, T2, T3, T4, T5, T6>(param1, param2, param3, param4, param5, param6, searcherObject));

        public HashSet<uint> SearchUnion(bool Multithread, HashSet<uint> SearchOnlyThisElements)
        {
            HashSet<uint> finalEligibleElements = new();
            if (Multithread)
            {
                
            }
            foreach (ILineNetworkSearcherContainer searcher in SearcherComponents)
            {
                finalEligibleElements.UnionWith(searcher.Search(SearchOnlyThisElements));
            }
            finalEligibleElements.ExceptWith(SearchOnlyThisElements);
            return finalEligibleElements;
        }
        public HashSet<uint> Searchintersect(bool Multithread, HashSet<uint> SearchOnlyThisElements)
        {
            HashSet<uint> finalEligibleElements = new();
            if (Multithread)
            {
                
            }

            List<HashSet<uint>> eligibleElementsBySearcherComponents = new();
            foreach (ILineNetworkSearcherContainer searcher in SearcherComponents)
            {
                eligibleElementsBySearcherComponents.Add(searcher.Search(SearchOnlyThisElements));
            }
            eligibleElementsBySearcherComponents.Sort(Comparer<HashSet<uint>>.Create((a, b) => a.Count.CompareTo(b.Count)));


            if(eligibleElementsBySearcherComponents.Count == 0)
            {
                return new();
            }
            finalEligibleElements = eligibleElementsBySearcherComponents[0];
            for(int i = 1; i < eligibleElementsBySearcherComponents.Count; i++)
            {
                finalEligibleElements.IntersectWith(eligibleElementsBySearcherComponents[i]);
            }
            finalEligibleElements.ExceptWith(SearchOnlyThisElements);
            return finalEligibleElements;
        }
    }
}
