namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetworkElementSearch
    {
        bool ThreadSafeSearch { get; }

        /// <returns>Returns the eligible elements keys</returns>
        HashSet<uint> Search();
    }
}
