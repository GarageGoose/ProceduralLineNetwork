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
    public BehaviorHandler(ElementsDatabase DB) { this.DB = DB; }

    public Dictionary<Type, ILineNetworkBehavior> LineNetworkBehaviors = new();
    public void Add(ILineNetworkBehavior LNB)
    {
        LineNetworkBehaviors.Add(LNB.GetType(), LNB);
    }
}

public class TrackerHandler
{
    private ElementsDatabase DB;
    public TrackerHandler(ElementsDatabase DB) { this.DB = DB; }

    public Dictionary<Type, ILineNetworkTracker> LineNetworkTrackers = new();
    public void Add(ILineNetworkTracker LNT)
    {
        LineNetworkTrackers.Add(LNT.GetType(), LNT);
    }
}

public class ModuleHandler
{
    private ElementsDatabase DB;
    public ModuleHandler(ElementsDatabase DB) { this.DB = DB; }
    private Dictionary<Type, Object> Modules = new();
    private List<Type> ModuleImplementsILineNetworkTracker       = new();
    private List<Type> ModuleImplementsILineNetworkEventListener = new();
    private List<Type> ModuleImplementsILineNetworkBehavior      = new();
    private List<Type> ModuleImplementsILineNetworkModulesAccess = new();

    public void Add<T>(T Module) where T : ILineNetworkBehavior, ILineNetworkEventListener, ILineNetworkTracker, ILineNetworkModulesAccess
    {

    }

    public T Get<T>() where T : class
    {
        if(Modules.TryGetValue(typeof(T), out var Module)) return Module as T;
        throw new KeyNotFoundException(typeof(T) + " Does not currently exist");
    }
}