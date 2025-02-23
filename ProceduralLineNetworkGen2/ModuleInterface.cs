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

        //
        HashSet<uint> FindObjects();

        //
        int EstimateAmountOfPoints();

        //
        bool IsElementEligible(uint Key);
    }

    public interface ILineNetworkEventListener
    {
        void OnPointAddition(uint PointKey) { }
        void OnPointUpdate(uint PointKey) { }
        void OnPointRemoval(uint PointKey) { }
        void OnLineAddition(uint LineKey) { }
        void OnLineUpdate(uint LineKey) { }
        void OnLineRemoval(uint LineKey) { }
    }

    public interface ILineNetworkModuleDependency
    {
        T[] InvokeModules<T>() where T : class;
        void GetModules<T>(T[] Modules) where T : class;
    }

    /// <summary>
    /// Used for custom behavior when expanding the line network
    /// </summary>
    public interface ILineNetworkBehavior
    {
        void Database(ref ElementsDatabase DB);
        void CurrElement(uint ElementKey, TypeOfElement Element);
        float ApplyRadian(float CurrRadians);
        float ApplyDist(float CurrDist);

    }

    public enum TypeOfElement
    {
        Point = 0,
        Line = 1
    }
}
