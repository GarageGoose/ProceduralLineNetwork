using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Numerics;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        /// <summary>
        /// Handles managing data for points and lines.
        /// </summary>
        public ElementsDatabase DB = new();

        /// <summary>
        /// Handles managing behavior of the line network when modifying it
        /// </summary>
        public ModificationManager Behavior;

        /// <summary>
        /// Handles data related to the networks' elements.
        /// </summary>
        public TrackerManager Tracker = new();

        public LineNetwork()
        {
            Behavior = new(DB);
        }
    }

    public class ElementsDatabase
    {
        public PointDict Points;
        public LineDict Lines;

        public class PointDict : Dictionary<uint, Point>
        {
            public new void Add(uint Key, Point NewPoint)
            {
                base.Add(Key, NewPoint);
            }
            public new void Remove(uint Key)
            {
                base.Remove(Key);
            }

            public new Point this[uint Key]
            {
                get => base[Key];
                set
                {
                    base[Key] = value;
                }
            }
        }

        public class LineDict : Dictionary<uint, Line>
        {
            PointDict PDict;
            public LineDict(PointDict PointDict)
            {
                PDict = PointDict;
            }
            public new void Add(uint Key, Line NewLine)
            {
                base.Add(Key, NewLine);
            }
            public new void Remove(uint Key)
            {
                base.Remove(Key);
            }
            public new Line this[uint Key]
            {
                get => base[Key];
                set
                {
                    base[Key] = value;
                }
            }
            private void LineAdded()
            {

            }
            private void LineDeleted()
            {

            }
        }
    }

    public class TrackerManager
    {
        /// <summary>
        /// Components used for tracking various stuff in the line network.
        /// </summary>
        public TrackerComponents Components = new();

        /// <summary>
        /// Search for eligible elements determined by all modules.
        /// </summary>
        /// <param name="Intersect">Qualifies elements that is only eligible in all modules.</param>
        /// <param name="Multithread">Make thread-safe modules run simultaneously.</param>
        /// <returns></returns>
        public HashSet<uint> SearchAll(bool Intersect, bool Multithread)
        {
            return new HashSet<uint>();
        }

        /// <summary>
        /// Search for eligible elements determined by specified modules.
        /// </summary>
        /// <param name="Components">List of components that will be used.</param>
        /// <param name="Intersect">Qualifies elements that is only eligible in all modules.</param>
        /// <param name="Multithread">Make thread-safe components run simultaneously.</param>
        /// <returns></returns>
        public HashSet<uint> Search(Type[] Components, bool Intersect, bool Multithread)
        {
            return new HashSet<uint>();
        }

        public class TrackerComponents : List<ILineNetworkTracker>
        {
            public new void Add(ILineNetworkTracker Instance)
            {

            }
        }
    }

    public class ModificationManager
    {
        private ElementsDatabase Database;
        public ModificationManager(ElementsDatabase Database)
        {
            this.Database = Database;
        }
        public void Execute(ILineNetworkModification[] UsedComponents)
        {

        }
    }
}

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public struct Point
    {
        public Vector2 Location;
    }

    public struct Line
    {
        public uint PointKey1;
        public uint PointKey2;
    }
}


