
namespace GarageGoose.LineNetwork
{
    class LineNetwork
    {
        ElementsDatabase DB = new();
        List<ILineNetworkTracker> Trackers = new();
        List<ILineNetworkBehavior> Behaviors = new();
    }
}