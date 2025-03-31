using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;

namespace GarageGoose.ProceduralLineNetwork
{
    public class LineNetwork
    {
        /// <summary>
        /// Handles managing data for points and lines.
        /// </summary>
        public ElementsDatabase DB;

        /// <summary>
        /// Handles managing behavior of the line network when modifying it
        /// </summary>
        public ModificationManager Behavior;

        /// <summary>
        /// Handles data related to the networks' elements.
        /// </summary>
        public TrackerManager Tracker = new();

        public LineNetwork()
        {
            DB = new(Tracker);
            Behavior = new(DB, Tracker);
        }
    }

    public class ElementsDatabase
    {
        private TrackerManager TM;
        public PointDict Points;
        public LineDict Lines;

        public ElementsDatabase(TrackerManager TM)
        {
            this.TM = TM;
            Points = new(TM);
            Lines = new(TM);
        }

        public class PointDict : Dictionary<uint, Point>
        {
            TrackerManager TM;
            public PointDict(TrackerManager TM)
            {
                this.TM = TM;
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
                    base[Key] = value;
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnPointModification))
                    {
                        TrackerComponent.OnPointModification(Key);
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
            public LineDict(TrackerManager TM)
            {
                this.TM = TM;
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
                LineDeleted(Key);
                base.Remove(Key);
            }
            public new Line this[uint Key]
            {
                get
                { 
                    return base[Key]; 
                }
                set
                {
                    base[Key] = value;
                    foreach (ILineNetworkTracker TrackerComponent in TM.GetTrackerByMethod(TrackerManager.InterfaceMethodNames.OnLineModification))
                    {
                        TrackerComponent.OnLineModification(Key);
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
            private void LineDeleted(uint Key)
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
                    "OnPointAddition", "OnPointModification", "OnPointRemoval",

                    //Line update
                    "OnLineAddition", "OnLineModification", "OnLineRemoval",

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
            OnPointAddition, OnPointModification, OnPointRemoval,

            //Line update
            OnLineAddition, OnLineModification, OnLineRemoval,

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


