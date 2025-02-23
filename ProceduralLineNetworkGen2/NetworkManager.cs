using System.Numerics;
using GarageGoose.ProceduralLineNetwork.Elements;
using GarageGoose.ProceduralLineNetwork.Module.Interface;

public class ElementsDatabase
{
    Dictionary<uint, Point> Points = new();
    Dictionary<uint, Line> Lines = new();
}

public class ModuleHandler
{
    private Dictionary<Type, Object> Modules = new();
    private List<Type> ModuleImplementsILineNetworkTracker          = new();
    private List<Type> ModuleImplementsILineNetworkEventListener    = new();
    private List<Type> ModuleImplementsILineNetworkBehavior         = new();
    private List<Type> ModuleImplementsILineNetworkModuleDependency = new();

    public void Add<T>(T Module) where T : ILineNetworkBehavior, ILineNetworkEventListener, ILineNetworkTracker, ILineNetworkModuleDependency
    {
        Type ModuleType = typeof(T);
        Modules.Add(ModuleType, Module);
        if (Module is ILineNetworkBehavior) ModuleImplementsILineNetworkBehavior.Add(ModuleType);
        if (Module is ILineNetworkEventListener) ModuleImplementsILineNetworkEventListener.Add(ModuleType);
        if (Module is ILineNetworkTracker) ModuleImplementsILineNetworkTracker.Add(ModuleType);
        if (Module is ILineNetworkModuleDependency)
        {
            ModuleImplementsILineNetworkModuleDependency.Add(ModuleType);
            List<T> ModuleDependencies = new();
            ILineNetworkModuleDependency ModuleWithDependency = Module;
            foreach(T ModuleDependency in ModuleWithDependency.InvokeModules<T>())
            {

            }
        }
    }

    public T Get<T>() where T : class
    {
        if(Modules.TryGetValue(typeof(T), out var Module)) return Module as T;
        throw new KeyNotFoundException(typeof(T) + " Does not currently exist");
    }

    public List<Type> GetBehaviorModules()
    {
        return ModuleImplementsILineNetworkBehavior;
    }

    public List<Type> GetTrackerModules()
    {
        return ModuleImplementsILineNetworkTracker;
    }

    public List<Type> GetEventListenerModules()
    {
        return ModuleImplementsILineNetworkBehavior;
    }

    public List<Type> GetModuleWithDependency()
    {
        return ModuleImplementsILineNetworkModuleDependency;
    }
}