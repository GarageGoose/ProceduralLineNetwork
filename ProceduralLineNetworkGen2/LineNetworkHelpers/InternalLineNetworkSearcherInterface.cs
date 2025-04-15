using GarageGoose.ProceduralLineNetwork.Component.Interface;

namespace GarageGoose.ProceduralLineNetwork.Manager
{

    public interface ILineNetworkSearcherContainer
    {
        HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null);
    }

    //These obejcts are used to contain the searcher component itself and set its parameters so that it can be executed at will inside the searcher.
    public class SearcherComponentContainer : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher searcherInstance;
        public SearcherComponentContainer(LineNetworkElementSearcher searcherInstance) => this.searcherInstance = searcherInstance;
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T> searcherInstance;
        readonly T param;
        public SearcherComponentContainer(T param, LineNetworkElementSearcher<T> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param = param;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param, LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T1, T2> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T1, T2> searcherInstance;
        readonly T1 param1;
        readonly T2 param2;
        public SearcherComponentContainer(T1 param1, T2 param2, LineNetworkElementSearcher<T1, T2> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param1 = param1;
            this.param2 = param2;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param1, param2, LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T1, T2, T3> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T1, T2, T3> searcherInstance;
        readonly T1 param1;
        readonly T2 param2;
        readonly T3 param3;
        public SearcherComponentContainer(T1 param1, T2 param2, T3 param3, LineNetworkElementSearcher<T1, T2, T3> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param1, param2, param3, LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T1, T2, T3, T4> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T1, T2, T3, T4> searcherInstance;
        readonly T1 param1;
        readonly T2 param2;
        readonly T3 param3;
        readonly T4 param4;
        public SearcherComponentContainer(T1 param1, T2 param2, T3 param3, T4 param4, LineNetworkElementSearcher<T1, T2, T3, T4> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param1, param2, param3, param4, LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T1, T2, T3, T4, T5> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T1, T2, T3, T4, T5> searcherInstance;
        readonly T1 param1;
        readonly T2 param2;
        readonly T3 param3;
        readonly T4 param4;
        readonly T5 param5;
        public SearcherComponentContainer(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, LineNetworkElementSearcher<T1, T2, T3, T4, T5> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param1, param2, param3, param4, param5, LimitSearchToTheseElements);
    }
    public class SearcherComponentContainer<T1, T2, T3, T4, T5, T6> : ILineNetworkSearcherContainer
    {
        readonly LineNetworkElementSearcher<T1, T2, T3, T4, T5, T6> searcherInstance;
        readonly T1 param1;
        readonly T2 param2;
        readonly T3 param3;
        readonly T4 param4;
        readonly T5 param5;
        readonly T6 param6;
        public SearcherComponentContainer(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, LineNetworkElementSearcher<T1, T2, T3, T4, T5, T6> searcherInstance)
        {
            this.searcherInstance = searcherInstance;
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.param6 = param6;
        }
        public HashSet<uint> Search(HashSet<uint>? LimitSearchToTheseElements = null) => searcherInstance.Search(param1, param2, param3, param4, param5, param6, LimitSearchToTheseElements);
    }
}
