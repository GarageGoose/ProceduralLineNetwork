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
        public readonly ObserverManager Observer;

        /// <summary>
        /// Search for eligible elements determined by specified components.
        /// </summary>
        public readonly SearcherManager Searcher;

        /// <summary>
        /// Generates unique keys for elements (Points and Lines). Used for identification.
        /// </summary>
        public readonly ElementKeyGenerator KeyGenerator;

        /// <summary>
        /// Handles the modification of the line network.
        /// </summary>
        public readonly ModificationManager Modification;

        public LineNetwork(bool MultithreadObservers, bool MultithreadSearching)
        {
            Observer = new(this, MultithreadObservers);
            KeyGenerator = new();
            Database = new(Observer, KeyGenerator);
            Modification = new(Observer);
            Searcher = new(MultithreadSearching);
        }
    }
}