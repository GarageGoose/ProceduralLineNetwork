using GarageGoose.ProceduralLineNetwork.Component.Interface;
using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork
{
    public class ElementsDatabase
    {
        public readonly ElementDict<Point> Points;
        public readonly ElementDict<Line> Lines;

        public ElementsDatabase(ObserverManager Observer, ElementKeyGenerator keyGenerator)
        {
            Points = new(Observer, keyGenerator, Type.Point);
            Lines = new(Observer, keyGenerator, Type.Line);
        }

        public Type CheckElementType(uint Key)
        {
            if (Points.ContainsKey(Key)) return Type.Point;
            if (Lines.ContainsKey(Key)) return Type.Line;
            return Type.Unknown;
        }

        /// <summary>
        /// Ignore this, only used internally. Custom dictionary is used to track addition, removal, and modification of an element
        /// </summary>
        public class ElementDict<TElement> : Dictionary<uint, TElement>
        {
            ObserverManager Observer;
            ElementKeyGenerator keyGenerator;

            private readonly UpdateType Addition;
            private readonly UpdateType ModificationBefore;
            private readonly UpdateType ModificationAfter;
            private readonly UpdateType Removal;

            public ElementDict(ObserverManager Observer, ElementKeyGenerator keyGenerator, Type ElementType)
            {
                this.Observer = Observer;
                this.keyGenerator = keyGenerator;

                Addition = (ElementType == Type.Point) ? UpdateType.OnPointAddition : UpdateType.OnLineAddition;
                ModificationBefore = (ElementType == Type.Point) ? UpdateType.OnPointModificationBefore : UpdateType.OnLineModificationBefore;
                ModificationAfter = (ElementType == Type.Point) ? UpdateType.OnPointModificationAfter : UpdateType.OnLineModificationAfter;
                Removal = (ElementType == Type.Point) ? UpdateType.OnPointRemoval : UpdateType.OnLineRemoval;
            }
            public uint AddAuto(TElement NewElement)
            {
                uint Key = keyGenerator.GenerateKey();
                base.Add(Key, NewElement);
                ElementAddition(Key);
                return Key;
            }
            public new void Add(uint Key, TElement NewElement)
            {
                base.Add(Key, NewElement);
                ElementAddition(Key);
            }
            public new bool TryAdd(uint Key, TElement NewElement)
            {
                bool Success = base.TryAdd(Key, NewElement);
                if (Success)
                {
                    ElementAddition(Key);
                }
                return Success;
            }
            public new void Remove(uint Key)
            {
                ElementRemoval(Key);
                base.Remove(Key);
            }
            public new void Remove(uint Key, out TElement? Element)
            {
                ElementRemoval(Key);
                base.Remove(Key, out Element);
            }

            public new TElement this[uint Key]
            {
                get => base[Key];
                set
                {
                    Observer.NotifyObservers(ModificationBefore, Key);
                    base[Key] = value;
                    Observer.NotifyObservers(ModificationAfter, Key);
                }
            }
            private void ElementAddition(uint Key)
            {
                Observer.NotifyObservers(Addition, Key);
            }
            private void ElementRemoval(uint Key)
            {
                Observer.NotifyObservers(Removal, Key);
            }
        }
        public enum Type
        {
            Point, Line, Unknown
        }
    }
}
