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
        public ElementsDatabase DB;

        /// <summary>
        /// Handles managing behavior of the line network when modifying it
        /// </summary>
        public BehaviorManager Behavior;

        /// <summary>
        /// Handles data related to the networks' elements.
        /// </summary>
        public TrackerManager  Tracker = new();

        public LineNetwork()
        {
            DB = new(Tracker);
            Behavior = new(DB);
        }
    }

    public class ElementsDatabase
    {
        public ObservableDict<Point> Points;
        public ObservableDict<Line> Lines;
        public ElementsDatabase(TrackerManager Tracker)
        {
            Points = new(Tracker);
            Lines = new(Tracker);
        }
    }

    public class TrackerManager
    {
        /// <summary>
        /// Components used for tracking various stuff in the line network.
        /// </summary>
        public SortedList<Type, ILineNetworkTracker> Components = new();

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
    }

    public class BehaviorManager
    {
        private ElementsDatabase Database;
        public BehaviorManager(ElementsDatabase Database)
        {
            this.Database = Database;
        }

        /// <summary>
        /// Components used for the behavior of points when expanding
        /// </summary>
        public SortedList<Type, ILineNetworkBehavior> Components = new();

        /// <summary>
        /// Expand the line network will all components having equal contribution.
        /// </summary>
        /// <param name="Points">PointKeys that will be used.</param>
        /// <param name="Multithread">Perform operation on multiple points simultaneously.</param>
        public void Expand(HashSet<uint> Points, bool Multithread)
        {

        }

        /// <summary>
        /// Expand the line network with having more control over the components.
        /// </summary>
        /// <param name="Points">PointKeys that will be used. (Better if more points at once)</param>
        /// <param name="Components">Components that will be used.</param>
        /// <param name="ComponentWeight">Relative contribution by each component where Components[i] == ComponentWeight[i].</param>
        /// <param name="Multithread">Perform operation on multiple points simultaneously.</param>
        public void ExpandAdvanced(HashSet<uint> Points, Type[] Components, float[] ComponentWeight, bool Multithread)
        {

        }
    }

    public class ObservableDict<TValue> : Dictionary<uint, TValue>
    {
        private TrackerManager Tracker;
        private readonly ElementType EType;

        public ObservableDict(TrackerManager Tracker)
        {
            this.Tracker = Tracker;
            if (typeof(TValue) == typeof(Point))
            {
                EType = ElementType.Point;
            }
            else if (typeof(TValue) == typeof(Line))
            {
                EType = ElementType.Line;
            }
            else
            {
                EType = ElementType.Unknown;
            }
        }

        public new void Add(uint key, TValue value)
        {
            base.Add(key, value);
            for (int i = 0; i < Tracker.Components.Count; i++)
            {
                Tracker.Components.Values[i].OnElementAddition(key, EType);
            }
        }

        public new void Remove(uint key)
        {
            base.Remove(key);
            for (int i = 0; i < Tracker.Components.Count; i++)
            {
                Tracker.Components.Values[i].OnElementRemoval(key, EType);
            }
        }

        public new TValue this[uint key]
        {
            get => base[key];
            set
            {
                for (int i = 0; i < Tracker.Components.Count; i++)
                {
                    Tracker.Components.Values[i].OnElementUpdate(key, EType);
                }
            }
        }
    }

    public enum ElementType
    {
        Point, Line, Unknown
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


