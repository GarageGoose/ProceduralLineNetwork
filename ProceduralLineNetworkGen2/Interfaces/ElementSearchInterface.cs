using System.Reflection.Metadata.Ecma335;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public abstract class LineNetworkElementSearcherBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool ThreadSafeSearch() { return false; }
    }

    /// <summary>
    /// Base class for element searcher components.
    /// </summary>
    public abstract class LineNetworkElementSearcher : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with a customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(TParam param, HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with two customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam1, TParam2> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param1">Customizable parameter.</param>
        /// <param name="param2">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset"></param>
        /// <returns></returns>
        public abstract HashSet<uint> Search(TParam1 param1, TParam2 param2, HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with three customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam1, TParam2, TParam3> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param1">Customizable parameter.</param>
        /// <param name="param2">Customizable parameter.</param>
        /// <param name="param3">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(TParam1 param1, TParam2 param2, TParam3 param3, HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with four customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam1, TParam2, TParam3, TParam4> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param1">Customizable parameter.</param>
        /// <param name="param2">Customizable parameter.</param>
        /// <param name="param3">Customizable parameter.</param>
        /// <param name="param4">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with five customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam1, TParam2, TParam3, TParam4, TParam5> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param1">Customizable parameter.</param>
        /// <param name="param2">Customizable parameter.</param>
        /// <param name="param3">Customizable parameter.</param>
        /// <param name="param4">Customizable parameter.</param>
        /// <param name="param5">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, HashSet<uint>? RestrictToThisSubset = null);
    }

    /// <summary>
    /// Base class for element searcher components with six customizable parameter.
    /// </summary>
    public abstract class LineNetworkElementSearcher<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : LineNetworkElementSearcherBase
    {
        /// <summary>
        /// Search for elements.
        /// </summary>
        /// <param name="param1">Customizable parameter.</param>
        /// <param name="param2">Customizable parameter.</param>
        /// <param name="param3">Customizable parameter.</param>
        /// <param name="param4">Customizable parameter.</param>
        /// <param name="param5">Customizable parameter.</param>
        /// <param name="param6">Customizable parameter.</param>
        /// <param name="RestrictToThisSubset">Limit the search to these elements.</param>
        /// <returns>Eligible element keys.</returns>
        public abstract HashSet<uint> Search(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, HashSet<uint>? RestrictToThisSubset = null);
    }


}
