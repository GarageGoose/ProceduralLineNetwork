using System.Runtime.InteropServices;

namespace GarageGoose.ProceduralLineNetwork.Module.Interface
{
    /// <summary>
    /// Used for tracking various stuff on the line network and retrieving elements with specific traits.
    /// </summary>
    public interface ILineNetworkTracker
    {
        //Database of points and lines
        void Database(ElementsDatabase DB);

        //Search for objects
        HashSet<uint> FindObjects();
    }

    public interface ILineNetworkEventListener
    {
        void Database(ElementsDatabase DB);
        void OnPointAddition(uint PointKey) { }
        void OnPointUpdate(uint PointKey) { }
        void OnPointRemoval(uint PointKey) { }
        void OnLineAddition(uint LineKey) { }
        void OnLineUpdate(uint LineKey) { }
        void OnLineRemoval(uint LineKey) { }
    }

    public interface ILineNetworkModulesAccess
    {
        void AccessModules(ModuleHandler MH);
    }

    /// <summary>
    /// Used for custom behavior when expanding the line network
    /// </summary>
    public interface ILineNetworkBehavior
    {
        void Database(ref ElementsDatabase DB);
        void CurrElement(uint ElementKey);
        float ApplyRotation(float Radians);
        float ApplyDist(float Dist);
    }
}
