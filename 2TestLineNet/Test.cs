using GarageGoose.ProceduralLineNetwork;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Component.Core;
using GarageGoose.ProceduralLineNetwork.Component.Interface;
namespace _2TestLineNet
{
    internal class Test
    {
        static void Main(string[] args)
        {
            //Crude test: add a line and check if LineAngle recorded the angle of the line

            Console.WriteLine("Line Network Test:");

            LineNetwork ln = new(false, false);

            ObserveLineAngles lineAngle = new(ln.Storage);
            ObserveOrderOfLinesOnPoint lineOrder = new(lineAngle, ln.Storage);
            ObserveAngleBetweenLines abl = new(lineAngle, ln.Storage, lineOrder);
            SortedAngles sortedAngles = new(lineAngle, ln.Storage);

            Notifier s = new();

            ln.LinkObserver(lineAngle);
            ln.LinkObserver(lineOrder);
            ln.LinkObserver(abl);
            ln.LinkObserver(sortedAngles);
            ln.LinkObserver(s);

            uint pointKey1 = ln.AddPoint(0, 0);
            uint pointKey2 = ln.AddPoint(0, 1);
            uint pointKey3 = ln.AddPoint(1, 0);

            uint lineKey1 = ln.AddLine(pointKey1, pointKey2);
            uint lineKey2 = ln.AddLine(pointKey1, pointKey3);

            Console.WriteLine(lineAngle.fromPoint1[lineKey1]);
            Console.WriteLine(abl.fromPoint1[lineKey1]);
            Console.WriteLine(abl.fromPoint1[lineKey2]);
            Console.WriteLine(abl.fromPoint2[lineKey1]);
            Console.WriteLine(abl.fromPoint2[lineKey2]);

            Console.WriteLine("---");

            

            foreach(float angle in sortedAngles.angles)
            {
                Console.WriteLine(angle);
            }
        }

        public class Notifier : ILineNetObserver
        {
            public ObserverEvent[] eventSubscription => [ObserverEvent.LineAdded, ObserverEvent.PointAdded];

            public uint UpdateLevel => 0;

            public bool Multithread => false;

            void ILineNetObserver.LineAdded(uint key, Line newLine)
            {
                Console.WriteLine("NewLine!");
            }

            void ILineNetObserver.PointAdded(uint key, Point newPoint)
            {
                Console.WriteLine("NewPoint!");
            }
        }
    }
}
