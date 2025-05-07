using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections;
using System.Xml.Linq;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    /// <summary>
    /// The main storage of a line network.
    /// </summary>
    public class ElementStorage
    {
        /// <summary>
        /// A dictionary of points in the line network.
        /// </summary>
        public readonly PointDict points;

        /// <summary>
        /// A dictionary of lines in the line network.
        /// </summary>
        public readonly LineDict lines;

        /// <summary>
        /// A dictionary of a set which represents the connected lines on a specified point.
        /// </summary>
        public readonly LinesOnPoint linesOnPoint = new();

        public ElementStorage(ObserverManager observer)
        {
            lines = linesOnPoint.NewLineDict(observer);
            points = linesOnPoint.NewPointDict(observer, lines);
        }

        /// <summary>
        /// Check the element type of the line network by its key
        /// </summary>
        public ElementType CheckElementType(uint Key)
        {
            if (points.ContainsKey(Key)) return ElementType.Point;
            if (lines.ContainsKey(Key)) return ElementType.Line;
            return ElementType.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineKey"></param>
        /// <returns></returns>
        public Point LinePoint1(uint lineKey)
        {
            return points[lines[lineKey].PointKey1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineKey"></param>
        /// <returns></returns>
        public Point LinePoint2(uint lineKey)
        {
            return points[lines[lineKey].PointKey2];
        }

        public class PointDict : IDictionary<uint, Point>
        {
            readonly ObserverManager observer;
            readonly LineDict lineDict;
            readonly Dictionary<uint, HashSet<uint>> internalLinesOnPoint;

            public PointDict(ObserverManager observer, Dictionary<uint, HashSet<uint>> internalLinesOnPoint, LineDict lineDict)
            {
                this.observer = observer;
                this.lineDict = lineDict;
                this.internalLinesOnPoint = internalLinesOnPoint;
            }

            private readonly Dictionary<uint, Point> internalDict = new();

            //Can modify dictionary by item addition
            public void Add(uint Key, Point value)
            {
                internalDict.Add(Key, value);
                internalLinesOnPoint.Add(Key, new());
                observer.callHandler.PointUpdateAdd(Key, value);
            }
            void ICollection<KeyValuePair<uint, Point>>.Add(KeyValuePair<uint, Point> item)
            {
                Add(item.Key, item.Value);
            }

            //Can modify dictionary by item mutation
            public Point this[uint key]
            {
                get => internalDict[key];
                set
                {
                    Point BeforeModification = internalDict[key];
                    internalDict[key] = value;
                    observer.callHandler.PointUpdateModification(key, BeforeModification, value!);
                }
            }

            //Can modify dictionary by item removal
            public bool Remove(uint key)
            {
                foreach (uint affectedLineKeys in internalLinesOnPoint[key])
                {
                    lineDict.Remove(affectedLineKeys);
                }
                internalLinesOnPoint.Remove(key);
                observer.callHandler.PointUpdateRemove(key, internalDict[key]);
                return internalDict.Remove(key);
            }
            bool ICollection<KeyValuePair<uint, Point>>.Remove(KeyValuePair<uint, Point> item)
            {
                return Remove(item.Key);
            }
            public void Clear()
            {
                observer.callHandler.PointUpdateClear();
                lineDict.Clear();
                internalLinesOnPoint.Clear();
                internalDict.Clear();
            }

            //Cannot modify dictionary
            public bool IsReadOnly => false;
            public int Count => internalDict.Count;
            public ICollection<uint> Keys => internalDict.Keys;
            public ICollection<Point> Values => internalDict.Values;
            public bool TryGetValue(uint key, out Point element) => internalDict.TryGetValue(key, out element!);
            public bool Contains(KeyValuePair<uint, Point> item) => internalDict.Contains(item);
            public bool ContainsKey(uint key) => internalDict.ContainsKey(key);
            public void CopyTo(KeyValuePair<uint, Point>[] item, int index) => ((IDictionary<uint, Point>)internalDict).CopyTo(item, index);
            public IEnumerator<KeyValuePair<uint, Point>> GetEnumerator() => internalDict.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public class LineDict : IDictionary<uint, Line>
        {
            ObserverManager observer;
            readonly Dictionary<uint, HashSet<uint>> internalLinesOnPoint;
            public LineDict(ObserverManager observer, Dictionary<uint, HashSet<uint>> internalLinesOnPoint)
            {
                this.observer = observer;
                this.internalLinesOnPoint = internalLinesOnPoint;
            }

            private readonly Dictionary<uint, Line> internalDict = new();

            //Cannot modify dictionary
            public bool IsReadOnly => false;
            public int Count => internalDict.Count;
            public ICollection<uint> Keys => internalDict.Keys;
            public ICollection<Line> Values => internalDict.Values;
            public bool TryGetValue(uint key, out Line element) => internalDict.TryGetValue(key, out element!);
            public bool Contains(KeyValuePair<uint, Line> item) => internalDict.Contains(item);
            public bool ContainsKey(uint key) => internalDict.ContainsKey(key);
            public void CopyTo(KeyValuePair<uint, Line>[] item, int index) => ((IDictionary<uint, Line>)internalDict).CopyTo(item, index);
            public IEnumerator<KeyValuePair<uint, Line>> GetEnumerator() => internalDict.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            //Can modify dictionary by item addition
            public void Add(uint Key, Line value)
            {
                internalDict.Add(Key, value);
                internalLinesOnPoint[value.PointKey1].Add(Key);
                internalLinesOnPoint[value.PointKey2].Add(Key);
                observer.callHandler.LineUpdateAdd(Key, value); //Add items first before observer notification
            }
            void ICollection<KeyValuePair<uint, Line>>.Add(KeyValuePair<uint, Line> item)
            {
                Add(item.Key, item.Value);
            }

            //Can modify dictionary by item mutation
            public Line this[uint key]
            {
                get => internalDict[key];
                set
                {
                    Line oldVer = internalDict[key];
                    internalDict[key] = value;
                    if (oldVer.PointKey1 != internalDict[key].PointKey1)
                    {
                        internalLinesOnPoint[oldVer.PointKey1].Remove(key);
                        internalLinesOnPoint[internalDict[key].PointKey1].Add(key);
                    }
                    if (oldVer.PointKey2 != internalDict[key].PointKey2)
                    {
                        internalLinesOnPoint[oldVer.PointKey2].Remove(key);
                        internalLinesOnPoint[internalDict[key].PointKey2].Add(key);
                    }
                    observer.callHandler.LineUpdateModification(key, oldVer, value!); //Modify all items first before observer notification
                }
            }

            //Can modify dictionary by item removal
            public bool Remove(uint key)
            {
                observer.callHandler.LineUpdateRemove(key, internalDict[key]); //Notify observer first before deletion
                internalLinesOnPoint[key].Remove(internalDict[key].PointKey1);
                internalLinesOnPoint[key].Remove(internalDict[key].PointKey2);
                return internalDict.Remove(key);
            }
            bool ICollection<KeyValuePair<uint, Line>>.Remove(KeyValuePair<uint, Line> item)
            {
                return Remove(item.Key);
            }
            public void Clear()
            {
                observer.callHandler.LineUpdateClear(); //Notify observer first before clearance
                foreach (uint key in internalLinesOnPoint.Keys) internalLinesOnPoint[key].Clear();
                internalDict.Clear();
            }
        }
        public class LinesOnPoint
        {
            public IReadOnlyDictionary<uint, HashSet<uint>> linesOnPoint;

            /// <summary>
            /// Key is point key, value is a hash set of lines connected to it
            /// </summary>
            private readonly Dictionary<uint, HashSet<uint>> internalLinesOnPoint;
            public LinesOnPoint() => linesOnPoint = internalLinesOnPoint = new();
            public PointDict NewPointDict(ObserverManager observer, LineDict lineDict) => new(observer, internalLinesOnPoint, lineDict);
            public LineDict NewLineDict(ObserverManager observer) => new(observer, internalLinesOnPoint);
        }
    }
}