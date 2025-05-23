using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Interface
{
    public interface ILineNetSearcher
    {
        HashSet<uint> Search();
    }
    public interface ILineNetSearcher<T1>
    {
        HashSet<uint> Search(T1 param1);
    }
    public interface ILineNetSearcher<T1, T2>
    {
        HashSet<uint> Search(T1 param1, T2 param2);
    }
    public interface ILineNetSearcher<T1, T2, T3>
    {
        HashSet<uint> Search(T1 param1, T2 param2, T3 param3);
    }
    public interface ILineNetSearcher<T1, T2, T3, T4>
    {
        HashSet<uint> Search(T1 param1, T2 param2, T3 param3, T4 param4);
    }
    public interface ILineNetSearcher<T1, T2, T3, T4, T5>
    {
        HashSet<uint> Search(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);
    }
}
