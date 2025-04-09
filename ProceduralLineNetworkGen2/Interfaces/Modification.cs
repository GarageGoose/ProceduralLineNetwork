namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetworkModification
    {
        /// <param name="SelectedElements">Target elements to perform the modification</param>
        /// <returns>True if the operation is successful, false if not</returns>
        bool ExecuteModification(HashSet<uint> SelectedElements);
    }
}
