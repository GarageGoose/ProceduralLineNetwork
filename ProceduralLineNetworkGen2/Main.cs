using GarageGoose.ProceduralLineNetwork.Component.Interface;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        /// <summary>
        /// Handles essential data (point position and connecting points on lines) related to the elements on the network.
        /// </summary>
        public readonly ElementsDatabase Database;

        /// <summary>
        /// Handles tracking changes and documenting the database further using additional components. This creates more comprehensive data that modification components can work out on.
        /// </summary>
        public readonly TrackerManager Tracker;

        public LineNetwork(bool MultithreadObservers)
        {
            Tracker = new(this, MultithreadObservers);
            Database = new(Tracker, this);
        }

        /// <summary>
        /// Generates unique keys for elements (Points and Lines). Used for identification.
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
                InheritLineNetworkAccess(Component);
                bool OperationSuccess = Component.ExecuteModification(SelectedElements);
                Tracker.NotifyObservers(UpdateType.ModificationComponentFinished, OperationSuccess);
            }
            Tracker.NotifyObservers(UpdateType.ModificationFinished);
        }

        public void InheritLineNetworkAccess(Object Component)
        {
            if(Component is ILineNetworkInherit)
            {
                ILineNetworkInherit ComponentAsILineNetworkInherit = (ILineNetworkInherit)Component;
                ComponentAsILineNetworkInherit.lineNetwork = this;
            }
            if (Component is ILineNetworkDatabaseInherit)
            {
                ILineNetworkDatabaseInherit ComponentAsILineNetworkDatabaseInherit = (ILineNetworkDatabaseInherit)Component;
                ComponentAsILineNetworkDatabaseInherit.elementsDatabase = Database;
            }
            if(Component is ILineNetworkTrackerInherit)
            {
                ILineNetworkTrackerInherit ComponentAsILineNetworkTrackerInherit = (ILineNetworkTrackerInherit)Component;
                ComponentAsILineNetworkTrackerInherit.trackerManager = Tracker;
            }
        }
    }
}