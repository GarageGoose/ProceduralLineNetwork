using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SortedAngles : LineNetworkObserver
    {
        private readonly ILineAngleTracker angleTracker;
        private readonly ElementStorage database;

        private SortedAngleSet<Tuple<uint, LineAtPoint>> lineAngles = new();

        public readonly IReadOnlySet<float> angles;
        public readonly IReadOnlyDictionary<float, Tuple<uint, LineAtPoint>> angleToPointKey;

        public SortedAngles(ILineAngleTracker angleTracker, ElementStorage database) : base(0, true)
        {
            this.angleTracker = angleTracker;
            this.database = database;

            angles = lineAngles.angles;
            angleToPointKey = lineAngles.angleToKey;
        }

        public IReadOnlySet<float> GetViewBetween(float min, float max) => lineAngles.GetViewBetween(min, max);

        protected override ElementUpdateType[]? SetSubscriptionToElementUpdates() =>
              [ElementUpdateType.OnPointModification, ElementUpdateType.OnLineAddition, ElementUpdateType.OnLineModification, ElementUpdateType.OnLineRemoval, ElementUpdateType.OnLineClear];

        protected override void PointModified(uint key, Point before, Point after)
        {
            foreach (uint lineKey in database.linesOnPoint.linesOnPoint[key])
            {
                lineAngles.Modify(new(lineKey, LineAtPoint.Point1), angleTracker.fromPoint1[lineKey]);
                lineAngles.Modify(new(lineKey, LineAtPoint.Point1), angleTracker.fromPoint1[lineKey]);
            }
        }

        protected override void LineAdded(uint key, Line line)
        {
            lineAngles.Add(angleTracker.fromPoint1[key], new(key, LineAtPoint.Point1));
            lineAngles.Add(angleTracker.fromPoint2[key], new(key, LineAtPoint.Point2));
        }

        protected override void LineModified(uint key, Line before, Line after)
        {
            lineAngles.Modify(new(key, LineAtPoint.Point1), angleTracker.fromPoint1[key]);
            lineAngles.Modify(new(key, LineAtPoint.Point2), angleTracker.fromPoint2[key]);
        }

        protected override void LineRemoved(uint key, Line line)
        {
            lineAngles.Remove(new(key, LineAtPoint.Point1));
            lineAngles.Remove(new(key, LineAtPoint.Point2));
        }

        protected override void LineClear()
        {
            lineAngles = new();
        }
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
