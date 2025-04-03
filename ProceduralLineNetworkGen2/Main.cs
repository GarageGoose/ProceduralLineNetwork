using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;

using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        /// <summary>
        /// Handles essential managing data for points and lines.
        /// More complex data of these elements is handled by tracker components.
        /// </summary>
        public readonly ElementsDatabase DB;

        /// <summary>
        /// Handles the modification of the line network.
        /// </summary>
        public readonly ModificationManager Modification;

        /// <summary>
        /// Handles data related to the networks' elements.
        /// </summary>
        public readonly TrackerManager Tracker = new();

        public LineNetwork()
        {
            DB = new(Tracker);
            Modification = new(DB, Tracker);
        }
    }

    /// <summary>
    /// Handles essential managing data for points and lines.
    /// More complex data of these elements is handled by tracker components.
    /// </summary>
    public class ElementsDatabase
    {
        TrackerManager TM;
        public PointDict Points;
        public LineDict Lines;
        private uint UniqueElementKey = 0;

        public uint GetNewUniqueKey()
        {
            return Interlocked.Increment(ref UniqueElementKey);
        }

        public ElementsDatabase(TrackerManager TM)
        {
            this.TM = TM;
            Points = new(TM, this);
            Lines = new(TM, this);
        }

        public class PointDict : Dictionary<uint, Point>
        {
            TrackerManager TM;
            ElementsDatabase ED;
            public PointDict(TrackerManager TM, ElementsDatabase ED)
            {
                this.TM = TM;
                this.ED = ED;
            }
            public uint AddAuto(Point NewPoint)
            {
                uint Key = ED.GetNewUniqueKey();
                base.Add(Key, NewPoint);
                PointAdded(Key);
                return Key;
            }
            public new void Add(uint Key, Point NewPoint)
            {
                base.Add(Key, NewPoint);
                PointAdded(Key);
            }
            public new bool TryAdd(uint Key, Point NewPoint)
            {
                bool Success = base.TryAdd(Key, NewPoint);
                if (Success)
                {
                    PointAdded(Key);
                }
                return Success;
            }
            public new void Remove(uint Key)
            {
                PointRemoval(Key);
                base.Remove(Key);
            }
            public new void Remove(uint Key, out Point Point)
            {
                PointRemoval(Key);
                base.Remove(Key, out Point);
            }

            public new Point this[uint Key]
            {
                get => base[Key];
                set
                {
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointModificationBefore))
                    {
                        TrackerComponent.OnPointModificationBefore(Key);
                    }
                    base[Key] = value;
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointModificationAfter))
                    {
                        TrackerComponent.OnPointModificationAfter(Key);
                    }
                }
            }
            private void PointAdded(uint Key)
            {
                foreach(ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointAddition))
                {
                    TrackerComponent.OnPointAddition(Key);
                }
            }
            private void PointRemoval(uint Key)
            {
                foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointRemoval))
                {
                    TrackerComponent.OnPointRemoval(Key);
                }
            }
        }

        public class LineDict : Dictionary<uint, Line>
        {
            TrackerManager TM;
            ElementsDatabase ED;
            public LineDict(TrackerManager TM, ElementsDatabase ED)
            {
                this.TM = TM;
                this.ED = ED;
            }
            public uint AddAuto(Line NewLine)
            {
                uint Key = ED.GetNewUniqueKey();
                base.Add(Key, NewLine);
                LineAdded(Key);
                return Key;
            }
            public new void Add(uint Key, Line NewLine)
            {
                base.Add(Key, NewLine);
                LineAdded(Key);
            }
            public new bool TryAdd(uint Key, Line NewLine)
            {
                bool Success = base.TryAdd(Key, NewLine);
                if (Success)
                {
                    LineAdded(Key);
                }
                return Success;
            }
            public new void Remove(uint Key)
            {
                LineRemoval(Key);
                base.Remove(Key);
            }
            public new void Remove(uint Key, out Line Line)
            {
                LineRemoval(Key);
                base.Remove(Key, out Line);
            }
            public new Line this[uint Key]
            {
                get
                { 
                    return base[Key]; 
                }
                set
                {
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnLineModificationBefore))
                    {
                        TrackerComponent.OnLineModificationBefore(Key);
                    }
                    base[Key] = value;
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnLineModificationAfter))
                    {
                        TrackerComponent.OnLineModificationAfter(Key);
                    }
                }
            }
            private void LineAdded(uint Key)
            {
                foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnLineAddition))
                {
                    TrackerComponent.OnLineAddition(Key);
                }
            }
            private void LineRemoval(uint Key)
            {
                foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointRemoval))
                {
                    TrackerComponent.OnLineRemoval(Key);
                }
            }
        }
    }


    public class TrackerManager
    {
        /// <summary>
        /// Components used for tracking various stuff in the line network.
        /// </summary>
        public ObservableCollection<ILineNetworkTracker> Components = new();

        private ComponentsByInterfaceMethod CBIM;

        public TrackerManager()
        {
            CBIM = new(Components);
            Components.CollectionChanged += ComponentSetup;
        }

        private void ComponentSetup(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach(ILineNetworkTracker Component in e.NewItems)
                {

                }
            }
        }

        /// <summary>
        /// Search for eligible elements determined by all modules.
        /// </summary>
        /// <param name="Intersect">Qualifies elements that is only eligible in all modules.</param>
        /// <param name="Multithread">Make thread-safe modules run simultaneously.</param>
        /// <returns></returns>
        public HashSet<uint> SearchAll(bool Intersect, bool Multithread)
        {
            return new HashSet<uint>();
        }

        /// <summary>
        /// Search for eligible elements determined by specified modules.
        /// </summary>
        /// <param name="Components">List of components that will be used.</param>
        /// <param name="Intersect">Qualifies elements that is only eligible in all modules.</param>
        /// <param name="Multithread">Make thread-safe components run simultaneously.</param>
        /// <returns></returns>
        public HashSet<uint> Search(Type[] Components, bool Intersect, bool Multithread)
        {
            return new HashSet<uint>();
        }

        private class ComponentsByInterfaceMethod
        {
            /// <summary>
            /// 
            /// </summary>
            public IReadOnlyDictionary<InterfaceMethodNames, HashSet<ILineNetworkTracker>> ComponentsByMethod;


            private readonly Dictionary<InterfaceMethodNames, HashSet<ILineNetworkTracker>> MethodsInternal;

            private ObservableCollection<ILineNetworkTracker> Components;

            private readonly string[] InterfaceMethodNamesString;


            public ComponentsByInterfaceMethod(ObservableCollection<ILineNetworkTracker> Components)
            {
                this.Components = Components;
                Components.CollectionChanged += ComponentsCollectionChanged;
                MethodsInternal = new();
                ComponentsByMethod = MethodsInternal;

                InterfaceMethodNamesString = new string[] 
                {
                    //Point update
                    "OnPointAddition", "OnPointModificationBefore", "OnPointModificationAfter", "OnPointRemoval",

                    //Line update
                    "OnLineAddition", "OnLineModificationBefore", "OnLineModificationAfter", "OnLineRemoval",

                    //Modification update
                    "ModificationStart", "ModificationFinished", "ModificationComponentStart", "ModificationComponentFinished",

                    //General
                    "RefreshData", "Search"
                };

                foreach(string InterfaceMethodName in InterfaceMethodNamesString)
                {
                    MethodsInternal.Add(StringToEnum(InterfaceMethodName), new());
                }
            }

            private void ComponentsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
                {
                    foreach(ILineNetworkTracker Component in e.NewItems)
                    {
                        MethodInfo[] ComponentMethods = Component.GetType().GetMethods();
                        foreach(MethodInfo ComponentMethod in ComponentMethods)
                        {
                            InterfaceMethodNames CurrMethod = StringToEnum(ComponentMethod.Name);
                            if (CurrMethod != InterfaceMethodNames.Unknown) MethodsInternal[CurrMethod].Add(Component);
                        }
                    }
                }

                if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
                {
                    foreach (ILineNetworkTracker Component in e.OldItems)
                    {
                        MethodInfo[] ComponentMethods = Component.GetType().GetMethods();
                        foreach (MethodInfo ComponentMethod in ComponentMethods)
                        {
                            InterfaceMethodNames CurrMethod = StringToEnum(ComponentMethod.Name);
                            if (CurrMethod != InterfaceMethodNames.Unknown) MethodsInternal[CurrMethod].Remove(Component);
                        }
                    }
                }
            }

            private InterfaceMethodNames StringToEnum (string MethodName)
            {
                for(int i = 0; i < InterfaceMethodNamesString.Length; i++)
                {
                    if (MethodName == InterfaceMethodNamesString[i]) return (InterfaceMethodNames)i;
                }
                return InterfaceMethodNames.Unknown;
            }
        }
        public enum InterfaceMethodNames
        {
            //Point update
            OnPointAddition, OnPointModificationBefore, OnPointModificationAfter, OnPointRemoval,

            //Line update
            OnLineAddition, OnLineModificationBefore, OnLineModificationAfter, OnLineRemoval,

            //Modification update
            ModificationStart, ModificationFinished, ModificationComponentStart, ModificationComponentFinished,

            //General
            RefreshData, Search,

            //Methods or functions in a class that isnt from the interface
            Unknown
        }
        public HashSet<ILineNetworkTracker> GetTrackerByMethod(InterfaceMethodNames MethodName)
        {
            return CBIM.ComponentsByMethod[MethodName];
        }
    }

    /// <summary>
    /// Handles the modification of the line network.
    /// </summary>
    public class ModificationManager
    {
        private ElementsDatabase Database;
        private TrackerManager Tracker;
        public ModificationManager(ElementsDatabase Database, TrackerManager Tracker)
        {
            this.Database = Database;
            this.Tracker = Tracker;
        }
        public void Execute(ILineNetworkModification[] UsedComponents)
        {
            foreach(ILineNetworkTracker TrackerComponent in Tracker.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.ModificationStart))
            {
                TrackerComponent.ModificationStart();
            }
            foreach(ILineNetworkModification ModificationComponent in UsedComponents)
            {
                foreach(ILineNetworkTracker TrackerComponent in Tracker.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.ModificationComponentStart))
                {
                    TrackerComponent.ModificationComponentStart(ModificationComponent);
                }
                ModificationComponent.Execute();
                foreach (ILineNetworkTracker TrackerComponent in Tracker.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.ModificationComponentFinished))
                {
                    TrackerComponent.ModificationComponentFinished(ModificationComponent);
                }
            }
            foreach (ILineNetworkTracker TrackerComponent in Tracker.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.ModificationFinished))
            {
                TrackerComponent.ModificationFinished();
            }
        }
    }
}

namespace GarageGoose.ProceduralLineNetwork.Elements
{
    public struct Point
    {
        public Vector2 Location;
    }

    public struct Line
    {
        public uint PointKey1;
        public uint PointKey2;
    }
}



interface Worker
{
    void work() { } //Exists but isn't forced to be implemented
    void eat() { } //This too
}