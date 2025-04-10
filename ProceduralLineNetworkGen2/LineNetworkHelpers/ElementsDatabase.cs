using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    public class ElementsDatabase
    {
        public readonly PointDict points;
        public readonly LineDict lines;
        public readonly LinesOnPoint linesOnPoint = new();

        public ElementsDatabase(ObserverManager observer)
        {
            lines = linesOnPoint.NewLineDict(observer);
            points = linesOnPoint.NewPointDict(observer, lines);
        }

        public Type CheckElementType(uint Key)
        {
            if (points.ContainsKey(Key)) return Type.Point;
            if (lines.ContainsKey(Key)) return Type.Line;
            return Type.Unknown;
        }

        public Point LinePoint1(uint lineKey)
        {
            return points[lines[lineKey].PointKey1];
        }
        public Point LinePoint2(uint lineKey)
        {
            return points[lines[lineKey].PointKey2];
        }
        public enum Type
        {
            Point, Line, Unknown
        }

        public class BaseElementDict<TElement> : IDictionary<uint, TElement>
        {
            ObserverManager observer;

            private readonly ElementUpdateType addition;
            private readonly ElementUpdateType modification;
            private readonly ElementUpdateType removal;
            private readonly ElementUpdateType clear;

            public BaseElementDict(ObserverManager observer, ElementUpdateType addition, ElementUpdateType modification, ElementUpdateType removal, ElementUpdateType clear)
            {
                this.observer = observer;
                this.addition = addition;
                this.modification = modification;
                this.removal = removal;
                this.clear = clear;
            }

            private readonly Dictionary<uint, TElement> internalDict = new();

            //Cannot modify dictionary
            public bool IsReadOnly => false;
            public int Count => internalDict.Count;
            public ICollection<uint> Keys => internalDict.Keys;
            public ICollection<TElement> Values => internalDict.Values;
            public bool TryGetValue(uint key, out TElement element) => internalDict.TryGetValue(key, out element);
            public bool Contains(KeyValuePair<uint, TElement> item) => internalDict.Contains(item);
            public bool ContainsKey(uint key) => internalDict.ContainsKey(key);
            public void CopyTo(KeyValuePair<uint, TElement>[] item, int index) => ((IDictionary<uint, TElement>)internalDict).CopyTo(item, index);
            public IEnumerator<KeyValuePair<uint, TElement>> GetEnumerator() => internalDict.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            //Can modify dictionary by item addition
            public void Add(uint Key, TElement value)
            {
                internalDict.Add(Key, value);
                observer.ElementAddOrRemoveNotifyObservers(addition, Key);
            }
            void ICollection<KeyValuePair<uint, TElement>>.Add(KeyValuePair<uint, TElement> item)
            {
                Add(item.Key, item.Value);
            }

            //Can modify dictionary by item mutation
            public TElement this[uint key]
            {
                get => internalDict[key];
                set
                {
                    TElement BeforeModification = internalDict[key];
                    internalDict[key] = value;
                    observer.ElementModificationNotifyObservers(modification, key, BeforeModification!, value!);
                }
            }

            //Can modify dictionary by item removal
            public bool Remove(uint key)
            {
                observer.ElementAddOrRemoveNotifyObservers(removal, key);
                return internalDict.Remove(key);
            }
            bool ICollection<KeyValuePair<uint, TElement>>.Remove(KeyValuePair<uint, TElement> item)
            {
                return Remove(item.Key);
            }
            public void Clear()
            {
                observer.ElementClearNotifyObservers(clear);
                internalDict.Clear();
            }
        }
        public class PointDict : BaseElementDict<Point>
        {
            readonly LineDict lineDict;
            readonly Dictionary<uint, HashSet<uint>> internalLinesOnPoint;

            public PointDict(ObserverManager observer, Dictionary<uint, HashSet<uint>> internalLinesOnPoint, LineDict lineDict) : base(observer, ElementUpdateType.OnPointAddition, ElementUpdateType.OnPointModification, ElementUpdateType.OnPointRemoval, ElementUpdateType.OnPointClear)
            {
                this.lineDict = lineDict;
                this.internalLinesOnPoint = internalLinesOnPoint;
            }

            public new void Add(uint key, Point point)
            {
                base.Add(key, point);
                internalLinesOnPoint.Add(key, new());
            }

            public new void Remove(uint key)
            {
                foreach(uint affectedLineKeys in internalLinesOnPoint[key])
                {
                    lineDict.Remove(affectedLineKeys);
                }
                internalLinesOnPoint.Remove(key);
                base.Remove(key);
            }

            public new void Clear()
            {
                lineDict.Clear();
                internalLinesOnPoint.Clear();
                base.Clear();
            }
        }
        public class LineDict : BaseElementDict<Line>
        {
            private readonly Dictionary<uint, HashSet<uint>> internalLinesOnPoint;
            public LineDict(ObserverManager observer, Dictionary<uint, HashSet<uint>> internalLinesOnPoint) : base(observer, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnPointClear)
            {
                this.internalLinesOnPoint = internalLinesOnPoint;
            }

            public new void Add(uint key, Line line)
            {
                base.Add(key, line);
                internalLinesOnPoint[line.PointKey1].Add(key);
                internalLinesOnPoint[line.PointKey2].Add(key);
            }

            public new void Remove(uint key)
            {
                internalLinesOnPoint[key].Remove(base[key].PointKey1);
                internalLinesOnPoint[key].Remove(base[key].PointKey2);
                base.Remove(key);
            }
            public new void Clear()
            {
                foreach(uint key in internalLinesOnPoint.Keys) internalLinesOnPoint[key].Clear();
                base.Clear();
            }
            public new Line this[uint key]
            {
                get => base[key];
                set
                {
                    Line oldVer = base[key];
                    base[key] = value;
                    if(oldVer.PointKey1 != base[key].PointKey1)
                    {
                        internalLinesOnPoint[oldVer.PointKey1].Remove(key);
                        internalLinesOnPoint[base[key].PointKey1].Add(key);
                    }
                    if (oldVer.PointKey2 != base[key].PointKey2)
                    {
                        internalLinesOnPoint[oldVer.PointKey2].Remove(key);
                        internalLinesOnPoint[base[key].PointKey2].Add(key);
                    }
                }
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
