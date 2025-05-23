using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetModifier
    {
        void Modify();
    }
    public interface ILineNetModifier<T1>
    {
        void Modify(T1 param1);
    }
    public interface ILineNetModifier<T1, T2>
    {
        void Modify(T1 param1, T2 param2);
    }
    public interface ILineNetModifier<T1, T2, T3>
    {
        void Modify(T1 param1, T2 param2, T3 param3);
    }
    public interface ILineNetModifier<T1, T2, T3, T4>
    {
        void Modify(T1 param1, T2 param2, T3 param3, T4 param4);
    }
    public interface ILineNetModifier<T1, T2, T3, T4, T5>
    {
        void Modify(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);
    }
}
