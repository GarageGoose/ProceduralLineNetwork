namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify();
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam param);
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam1, TParam2>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam1 param1, TParam2 param2);
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam1, TParam2, TParam3>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam1, TParam2, TParam3, TParam4>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }

    /// <summary>
    /// Base class for line network modifier components.
    /// </summary>
    public abstract class LineNetworkModifier<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        /// <summary>
        /// Modify the line network.
        /// </summary>
        public abstract void Modify(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6);
    }
}
