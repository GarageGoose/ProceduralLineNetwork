using System.Numerics;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Module.Interface;

public class ElementsDatabase
{
    Dictionary<uint, Point> Points = new();
    Dictionary<uint, Line> Lines = new();
}

public class BehaviorHandler
{
    private ElementsDatabase DB;
    public BehaviorHandler(ElementsDatabase DB)
    {
        this.DB = DB;
    }
    public Dictionary<Type, ILineNetworkBehavior> LineNetworkBehaviors = new();
    public void Add(ILineNetworkBehavior LNB)
    {
        LineNetworkBehaviors.Add(LNB.GetType(), LNB);
    }
    public T Get<T>() where T : class
    {
        if (LineNetworkBehaviors.TryGetValue(typeof(T), out var Behavior)) return Behavior as T;
        throw new KeyNotFoundException(typeof(T) + " Does not currently exist");
    }
}

public class TrackerHandler
{
    private ElementsDatabase DB;
    public TrackerHandler(ElementsDatabase DB)
    {
        this.DB = DB;
    }
    public Dictionary<Type, ILineNetworkTracker> LineNetworkTrackers = new();
    public void Add(ILineNetworkTracker LNT)
    {
        LineNetworkTrackers.Add(LNT.GetType(), LNT);
    }
    public T Get<T>() where T : class
    {
        if (LineNetworkTrackers.TryGetValue(typeof(T), out var Behavior)) return Behavior as T;
        throw new KeyNotFoundException(typeof(T) + " Does not currently exist");
    }
}

public class ModuleHandler
{
    private ElementsDatabase DB;

    private Dictionary<Type, Object> Modules = new();
    private List<Type> ModuleImplementsILineNetworkTracker = new();
    private List<Type> ModuleImplementsILineNetworkEventListener = new();
    private List<Type> ModuleImplementsILineNetworkBehavior = new();
}