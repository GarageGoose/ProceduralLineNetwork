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
        public readonly ElementStorage elements;

        /// <summary>
        /// Handles observation of the elements using additional components. This creates more comprehensive data that modification components can work out on.
        /// </summary>
        public readonly ObserverManager observer;

        /// <summary>
        /// Generates unique keys for elements (Points and Lines). Used for identification.
        /// </summary>
        public readonly FastKeyGen keyGenerator;

        public LineNetwork(bool MultithreadObservers)
        {
            observer = new(MultithreadObservers);
            keyGenerator = new();
            elements = new(observer);
        }






        /// <summary>
        /// Add a new line to the line network.
        /// </summary>
        /// <param name="line">The line to add.</param>
        /// <returns>Returns the key of the line.</returns>
        public uint AddLine(Line line)
        {
            uint Key = keyGenerator.GenerateKey();
            elements.lines.Add(Key, line);
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
            uint Key = keyGenerator.GenerateKey();
            elements.lines.Add(Key, new(pointKey1, pointKey2));
            return Key;
        }

        /// <summary>
        /// Modify a line in the line network.
        /// </summary>
        /// <param name="key">Key of the target line</param>
        /// <param name="newLine">Line to replace it with</param>
        public void ModifyLine(uint key, Line newLine) => elements.lines[key] = newLine;

        /// <summary>
        /// Modify a line in the line network.
        /// </summary>
        /// <param name="key">Key of the target line</param>
        /// <param name="pointKey1">New first point the line connects to</param>
        /// <param name="pointKey2">New second point the line connects to</param>
        public void ModifyLine(uint key, uint pointKey1, uint pointKey2) => elements.lines[key] = new(pointKey1, pointKey2);

        /// <summary>
        /// Remove a line in the line network
        /// </summary>
        /// <param name="key">Key of the line to remove</param>
        public void RemoveLine(uint key) => elements.lines.Remove(key);

        /// <summary>
        /// Add a new point to the line network.
        /// </summary>
        /// <param name="point">The point to add.</param>
        /// <returns>Returns the key of the point.</returns>
        public uint AddPoint(Point point)
        {
            uint Key = keyGenerator.GenerateKey();
            elements.points.Add(Key, point);
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
            uint Key = keyGenerator.GenerateKey();
            elements.points.Add(Key, new(x, y));
            return Key;
        }

        /// <summary>
        /// Modify a point in the line network.
        /// </summary>
        /// <param name="key">Key of the target point.</param>
        /// <param name="newPoint">Point to replace it with.</param>
        public void ModifyPoint(uint key, Point newPoint) => elements.points[key] = newPoint;

        /// <summary>
        /// Modify a point in the line network.
        /// </summary>
        /// <param name="key">Key of the target point.</param>
        /// <param name="x">New X coordinates of the point.</param>
        /// <param name="y">New Y coordinates of the point.</param>
        public void ModifyPoint(uint key, float x, float y) => elements.points[key] = new(x, y);

        /// <summary>
        /// Remove a point in the line network
        /// </summary>
        /// <param name="key">Key of the point to remove</param>
        public void DeletePoint(uint key) => elements.points.Remove(key);

        /// <summary>
        /// Link an observer to the line network.
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer to add.</param>
        public void LinkObserver(ILineNetObserver observer) => this.observer.linkedObservers.Add(observer);

        /// <summary>
        /// Link an observer to the line network.
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer to add.</param>
        public TObserver LinkObserver<TObserver>(TObserver observer) where TObserver : ILineNetObserver
        {
            this.observer.linkedObservers.Add(observer);
            return observer;
        }

        /// <summary>
        /// Unlink an observer to the line network. Do this before deleting the observer!
        /// An observer observes different specific parts of the line network and create data additional data from.
        /// </summary>
        /// <param name="observer">Observer instance to remove.</param>
        public void UnlinkObserver(ILineNetObserver observer) => this.observer.linkedObservers.Remove(observer);
    }
}