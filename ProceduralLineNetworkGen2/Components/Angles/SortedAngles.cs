using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Angles of a line (from the perspective of Point1 and Point2) in the entire line network sorted by 0 to 2pi.  
    /// </summary>
    public class SortedAngles : ILineNetObserver
    {
        private readonly ILineAngleTracker angleTracker;
        private readonly ElementStorage database;

        private SortedAngleSet<Tuple<uint, LineEndPoint>> lineAngles = new();

        /// <summary>
        /// Sorted line angles (from the perspective of Point1 and Point2) of the entire line network.
        /// Use <code>angleToPointKey</code> to determine which line it belongs (and also the perspective from <code>Point1</code> or <code>Point2</code>).
        /// </summary>
        public readonly IReadOnlySet<float> angles;

        /// <summary>
        /// Angle to line (and the perspective from <code>Point1</code> or <code>Point2</code>) pointer.
        /// </summary>
        public readonly IReadOnlyDictionary<float, Tuple<uint, LineEndPoint>> angleToPointKey;

        public ObserverEvent[] eventSubscription => [ObserverEvent.PointModified, ObserverEvent.LineAdded, ObserverEvent.LineModified, ObserverEvent.LineRemoved, ObserverEvent.LineClear];

        public uint UpdateLevel => ((ILineNetObserver)angleTracker).UpdateLevel + 1;

        public bool Multithread => throw new NotImplementedException();

        /// <summary>
        /// Angles of a line (from the perspective of Point1 and Point2) in the entire line network sorted by 0 to 2pi.  
        /// </summary>
        /// <param name="angleTracker">Target angle tracker</param>
        /// <param name="storage">Target element storage</param>
        public SortedAngles(ILineAngleTracker angleTracker, ElementStorage storage)
        {
            this.angleTracker = angleTracker;
            this.database = storage;

            angles = lineAngles.angles;
            angleToPointKey = lineAngles.angleToKey;

            foreach (uint lineKey in storage.lines.Keys)
            {
                ((ILineNetObserver)this).LineAdded(lineKey, new(0, 0));
            }
        }

        public IReadOnlySet<float> GetViewBetween(float min, float max) => lineAngles.GetViewBetween(min, max);

        void ILineNetObserver.PointModified(uint key, Point before, Point after)
        {
            foreach (uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                lineAngles.Modify(new(lineKey, LineEndPoint.Point1), angleTracker.fromPoint1[lineKey]);
                lineAngles.Modify(new(lineKey, LineEndPoint.Point1), angleTracker.fromPoint1[lineKey]);
            }
        }

        void ILineNetObserver.LineAdded(uint key, Line line)
        {
            lineAngles.Add(angleTracker.fromPoint1[key], new(key, LineEndPoint.Point1));
            lineAngles.Add(angleTracker.fromPoint2[key], new(key, LineEndPoint.Point2));
        }

        void ILineNetObserver.LineModified(uint key, Line before, Line after)
        {
            lineAngles.Modify(new(key, LineEndPoint.Point1), angleTracker.fromPoint1[key]);
            lineAngles.Modify(new(key, LineEndPoint.Point2), angleTracker.fromPoint2[key]);
        }

        void ILineNetObserver.LineRemoved(uint key, Line line)
        {
            lineAngles.Remove(new(key, LineEndPoint.Point1));
            lineAngles.Remove(new(key, LineEndPoint.Point2));
        }

        void ILineNetObserver.LineClear()
        {
            lineAngles = new();
        }
    }

    public interface ILineAngleTracker
    {
        IReadOnlyDictionary<uint, float> fromPoint1 { get; }
        IReadOnlyDictionary<uint, float> fromPoint2 { get; }
    }

    /// <summary>
    /// Custom data structure for SortedAngles that is mainly backed by a SortedSet of angles that allows dupelicate
    /// internally via nudging the values microscopically and that also act as a key for its associated line.
    /// </summary>
    public class SortedAngleSet<TKey> where TKey : notnull
    {
        public readonly SortedSet<float> internalAngles = new();
        public readonly Dictionary<float, TKey> internalAngleToKey = new();
        public readonly Dictionary<TKey, float> internalKeyToAngle = new();

        public IReadOnlySet<float> angles;
        public IReadOnlyDictionary<float, TKey> angleToKey;

        public SortedAngleSet()
        {
            angles = internalAngles;
            angleToKey = internalAngleToKey;
        }

        public IReadOnlySet<float> GetViewBetween(float min, float max) => internalAngles.GetViewBetween(min, max);

        /// <summary>
        /// Add a new angle with its corresponding line key. Note that the angle might differ slightly
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="lineKey"></param>
        public void Add(float angle, TKey lineKey)
        {
            if (!internalAngles.Add(angle))
            {
                //Nudge the angle very slightly so that it would get added to the sortedlist while having microscopic difference.
                float nudgeAngleAmount = (angle >= MathF.PI) ? 0.000001f : -0.000001f;
                float nudgeAngle = angle + nudgeAngleAmount;
                while (!internalAngles.Add(nudgeAngle))
                {
                    nudgeAngle += nudgeAngleAmount;
                }
                internalAngleToKey.Add(nudgeAngle, lineKey);
                internalKeyToAngle.Add(lineKey, nudgeAngle);
                return;
            }
            internalAngleToKey.Add(angle, lineKey);
            internalKeyToAngle.Add(lineKey, angle);
        }
        public void Modify(TKey lineKey, float newAngle)
        {
            float oldAngle = internalKeyToAngle[lineKey];
            internalAngles.Remove(oldAngle);
            internalAngleToKey.Remove(oldAngle);

            if (!internalAngles.Add(newAngle))
            {
                //Nudge the angle very slightly so that it would get added to the sortedlist while having microscopic difference.
                float nudgeAngleAmount = (newAngle >= MathF.PI) ? 0.000001f : -0.000001f;
                float nudgeAngle = newAngle + nudgeAngleAmount;
                while (!internalAngles.Add(nudgeAngle))
                {
                    nudgeAngle += nudgeAngleAmount;
                }
                internalKeyToAngle[lineKey] = nudgeAngle;
                internalAngleToKey.Add(nudgeAngle, lineKey);
                return;
            }
            internalKeyToAngle[lineKey] = newAngle;
            internalAngleToKey.Add(newAngle, lineKey);
        }
        public void Remove(TKey lineKey)
        {
            float angle = internalKeyToAngle[lineKey];
            internalKeyToAngle.Remove(lineKey);
            internalAngleToKey.Remove(angle);
            internalAngles.Remove(angle);
        }
    }
}
