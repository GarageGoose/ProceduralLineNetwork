using GarageGoose.ProceduralLineNetwork.Component.Interface;

namespace GarageGoose.ProceduralLineNetwork
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
            observer.NotifyObservers(UpdateType.ModificationStart);
            foreach (ILineNetworkModification Component in UsedComponents)
            {
                observer.NotifyObservers(UpdateType.ModificationComponentStart, Component);
                bool OperationSuccess = Component.ExecuteModification(SelectedElements);
                observer.NotifyObservers(UpdateType.ModificationComponentFinished, OperationSuccess);
            }
            observer.NotifyObservers(UpdateType.ModificationFinished);
        }
    }
}
