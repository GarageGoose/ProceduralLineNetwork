using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralLineNetwork.LineNetwork.Helpers
{
    public class LineNetAggregator
    {
        public readonly PointDict points;
        public readonly LineDict lines;
        public readonly LinesOnPoint connectedLinesOnPoint;
        public readonly IReadOnlyCollection<ILineNetObserver> observers;

        public LineNetAggregator(PointDict points, LineDict lines, LinesOnPoint connectedLinesOnPoint, Collection<ILineNetObserver> observers)
        {
            this.points = points;
            this.lines = lines;
            this.connectedLinesOnPoint = connectedLinesOnPoint;
            this.observers = observers;
        }
    }
}
