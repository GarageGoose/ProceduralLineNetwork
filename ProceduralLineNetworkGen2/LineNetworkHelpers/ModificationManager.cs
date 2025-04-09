using GarageGoose.ProceduralLineNetwork.Component.Interface;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    public class ModificationManager
    {
        private readonly ObserverManager observer;

        public ModificationManager(ObserverManager observer)
        {
            this.observer = observer;
        }

        public void ExecuteModification(ILineNetworkModification[] UsedComponents, HashSet<uint> SelectedElements)
        {
            observer.NotifyObservers(ElementUpdateType.ModificationStart);
            foreach (ILineNetworkModification Component in UsedComponents)
            {
                observer.NotifyObservers(ElementUpdateType.ModificationComponentStart, Component);
                bool OperationSuccess = Component.ExecuteModification(SelectedElements);
                observer.NotifyObservers(ElementUpdateType.ModificationComponentFinished, OperationSuccess);
            }
            observer.NotifyObservers(ElementUpdateType.ModificationFinished);
        }
    }
}
