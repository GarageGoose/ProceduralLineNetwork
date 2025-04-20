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

            TrackLineAngles lineAngle = new(ln.Database);
            TrackOrderOfLinesOnPoint lineOrder = new(lineAngle, ln.Database);
            TrackAngleBetweenLines abl = new(lineAngle, ln.Database, lineOrder);
            //ln.Observer.observerComponents.Add(lineAngle);
            //ln.Observer.observerComponents.Add(lineOrder);
            //ln.Observer.observerComponents.Add(abl);

            ln.AddObserver(lineAngle);
            ln.AddObserver(lineOrder);
            ln.AddObserver(abl);

            uint pointKey1 = ln.KeyGenerator.GenerateKey();
            uint pointKey2 = ln.KeyGenerator.GenerateKey();
            uint pointKey3 = ln.KeyGenerator.GenerateKey();
            ln.Database.points.Add(pointKey1, new(0, 0));
            ln.Database.points.Add(pointKey2, new(0, 1));
            ln.Database.points.Add(pointKey3, new(1, 0));

            uint lineKey1 = ln.KeyGenerator.GenerateKey();
            uint lineKey2 = ln.KeyGenerator.GenerateKey();
            ln.Database.lines.Add(lineKey2, new(pointKey1, pointKey3));
            ln.Database.lines.Add(lineKey1, new(pointKey2, pointKey1));

            Console.WriteLine(lineAngle.fromPoint1[lineKey1]);
            Console.WriteLine(abl.fromPoint1[lineKey1]);
            Console.WriteLine(abl.fromPoint1[lineKey2]);
            Console.WriteLine(abl.fromPoint2[lineKey1]);
            Console.WriteLine(abl.fromPoint2[lineKey2]);
        }
    }
}
