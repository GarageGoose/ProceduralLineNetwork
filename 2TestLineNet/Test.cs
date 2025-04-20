using GarageGoose.ProceduralLineNetwork;
using GarageGoose.ProceduralLineNetwork.Component;
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
            ln.Observer.observerComponents.Add(lineAngle);

            uint pointKey1 = ln.KeyGenerator.GenerateKey();
            uint pointKey2 = ln.KeyGenerator.GenerateKey();
            ln.Database.points.Add(pointKey1, new(0, 0));
            ln.Database.points.Add(pointKey2, new(0, 1));

            uint lineKey = ln.KeyGenerator.GenerateKey();
            ln.Database.lines.Add(ln.KeyGenerator.GenerateKey(), new(pointKey1, pointKey2));

            Console.WriteLine(lineAngle.fromPoint1[lineKey]);
        }
    }
}
