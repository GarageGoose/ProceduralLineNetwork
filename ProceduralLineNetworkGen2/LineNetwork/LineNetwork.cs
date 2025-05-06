using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Manager;
using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork
{
    /// <summary>
    /// 
    /// </summary>
    public class LineNetwork
    {
        /// <summary>
        /// Handles essential data (point position and connecting points on lines) related to the elements on the network.
        /// </summary>
        public readonly ElementStorage Storage;

        /// <summary>
        /// Handles observation of the elements using additional components. This creates more comprehensive data that modification components can work out on.
        /// </summary>
        public readonly ObserverManager Observer;

        /// <summary>
        /// Generates unique keys for elements (Points and Lines). Used for identification.
        /// </summary>
        public readonly FastKeyGen KeyGenerator;

        public LineNetwork(bool MultithreadObservers, bool MultithreadSearching)
        {
            Observer = new(MultithreadObservers);
            KeyGenerator = new();
            Storage = new(Observer);
        }

        //Down below is the shortcuts to commonly used methods for simplicity.

        /// <summary>
        /// Add a new line to the line network.
        /// </summary>
        /// <param name="line">The line to add.</param>
        /// <returns>Returns the key of the line.</returns>
        public uint AddLine(Line line)
        {
            uint Key = KeyGenerator.GenerateKey();
            Storage.lines.Add(Key, line);
            return Key;
        }

        /// <summary>
        /// Add a new line to the line network.
        /// </summary>
        /// <param name="pointKey1">First point the line connects to.</param>
        /// <param name="pointKey2">Second point the line connects to.</param>
        /// <returns>Returns the key of the line.</returns>
        public uint AddLine(uint pointKey1, uint pointKey2)
        {
            uint Key = KeyGenerator.GenerateKey();
            Storage.lines.Add(Key, new(pointKey1, pointKey2));
            return Key;
        }

        /// <summary>
        /// Modify a line in the line network.
        /// </summary>
        /// <param name="key">Key of the target line</param>
        /// <param name="newLine">Line to replace it with</param>
        public void ModifyLine(uint key, Line newLine) => Storage.lines[key] = newLine;

        /// <summary>
        /// Modify a line in the line network.
        /// </summary>
        /// <param name="key">Key of the target line</param>
        /// <param name="pointKey1">New first point the line connects to</param>
        /// <param name="pointKey2">New second point the line connects to</param>
        public void ModifyLine(uint key, uint pointKey1, uint pointKey2) => Storage.lines[key] = new(pointKey1, pointKey2);

        /// <summary>
        /// Remove a line in the line network
        /// </summary>
        /// <param name="key">Key of the line to remove</param>
        public void RemoveLine(uint key) => Storage.lines.Remove(key);

        /// <summary>
        /// Add a new point to the line network.
        /// </summary>
        /// <param name="point">The point to add.</param>
        /// <returns>Returns the key of the point.</returns>
        public uint AddPoint(Point point)
        {
            uint Key = KeyGenerator.GenerateKey();
            Storage.points.Add(Key, point);
            return Key;
        }

        /// <summary>
        /// Add a new point to the line network.
        /// </summary>
        /// <param name="x">X coordinates of the new point.</param>
        /// <param name="y">Y coordinates of the new point.</param>
        /// <returns>Returns the key of the point.</returns>
        public uint AddPoint(float x, float y)
        {
            uint Key = KeyGenerator.GenerateKey();
            Storage.points.Add(Key, new(x, y));
            return Key;
        }

        /// <summary>
        /// Modify a point in the line network.
        /// </summary>
        /// <param name="key">Key of the target point.</param>
        /// <param name="newPoint">Point to replace it with.</param>
        public void ModifyPoint(uint key, Point newPoint) => Storage.points[key] = newPoint;

        /// <summary>
        /// Modify a point in the line network.
        /// </summary>
        /// <param name="key">Key of the target point.</param>
        /// <param name="x">New X coordinates of the point.</param>
        /// <param name="y">New Y coordinates of the point.</param>
        public void ModifyPoint(uint key, float x, float y) => Storage.points[key] = new(x, y);

        /// <summary>
        /// Remove a point in the line network
        /// </summary>
        /// <param name="key">Key of the point to remove</param>
        public void DeletePoint(uint key) => Storage.points.Remove(key);

        /// <summary>
        /// Add an observer to the line network.
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer to add.</param>
        public void AddObserver(LineNetworkObserver observer) => Observer.observerComponents.Add(observer);

        /// <summary>
        /// Add an observer to the line network.
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer to add.</param>
        public TObserver AddObserver<TObserver>(TObserver observer) where TObserver : LineNetworkObserver
        {
            Observer.observerComponents.Add(observer);
            return observer;
        }

        /// <summary>
        /// Remove an observer to the line network.
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer instance to remove.</param>
        public void RemoveObserver(LineNetworkObserver observer) => Observer.observerComponents.Remove(observer);
    }
}