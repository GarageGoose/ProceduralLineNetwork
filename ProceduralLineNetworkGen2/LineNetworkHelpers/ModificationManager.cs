﻿using GarageGoose.ProceduralLineNetwork.Component.Interface;

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
            foreach (ILineNetworkModification Component in UsedComponents)
            {
                observer.ObserverActionNotifyObservers(Component, ComponentAction.Start);
                bool OperationSuccess = Component.ExecuteModification(SelectedElements);
                observer.ObserverActionNotifyObservers(Component, ComponentAction.Finished);
            }
        }
    }
}
