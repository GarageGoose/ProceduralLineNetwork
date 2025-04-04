using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        /// <summary>
        /// Handles essential data related to the networks' elements.
        /// </summary>
        public readonly ElementsDatabase Database;

        /// <summary>
        /// Handles additional data related to the networks' elements.
        /// </summary>
        public readonly TrackerManager Tracker;

        public LineNetwork(ElementsDatabase Database, TrackerManager Tracker)
        {
            this.Database = Database;
            this.Tracker = Tracker;
        }

        /// <summary>
        /// Generates unique keys for identifying elements (Points and Lines)
        /// </summary>
        private uint Keys = 0;
        public uint NewKey()
        {
            return Keys++;
        }

        /// <summary>
        /// Handles the modification of the line network.
        /// </summary>
        public void ExecuteModification(ILineNetworkModification[] UsedComponents, HashSet<uint> SelectedElements)
        {
            Tracker.NotifyObservers(UpdateType.ModificationStart);
            foreach (ILineNetworkModification Component in UsedComponents)
            {
                Tracker.NotifyObservers(UpdateType.ModificationComponentStart, Component);
                Component.ExecuteModification(SelectedElements);
                Tracker.NotifyObservers(UpdateType.ModificationComponentFinished, Component);
            }
            Tracker.NotifyObservers(UpdateType.ModificationFinished);
        }

        public void InheritComponentAccess(Object Component)
        {
            if(Component is ILineNetworkInherit)
            {
                ILineNetworkInherit ComponentAsILineNetworkInherit = (ILineNetworkInherit)Component;
                ComponentAsILineNetworkInherit.Inherit(this);
            }
        }
    }
}