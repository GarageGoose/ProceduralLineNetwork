using GarageGoose.ProceduralLineNetwork;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Component.Core;
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

            ln.AddObserver(lineAngle);
            ln.AddObserver(lineOrder);
            ln.AddObserver(abl);
            ln.AddObserver(sortedAngles);

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
    }
}
