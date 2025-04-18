using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Manager;

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
        public readonly ObserverManager Observer;

        /// <summary>
        /// Generates unique keys for elements (Points and Lines). Used for identification.
        /// </summary>
        public readonly ElementKeyGenerator KeyGenerator;

        public LineNetwork(bool MultithreadObservers, bool MultithreadSearching)
        {
            Observer = new(MultithreadObservers);
            KeyGenerator = new();
            Database = new(Observer);
        }
    }
}