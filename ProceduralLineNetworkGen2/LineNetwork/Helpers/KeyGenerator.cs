using GarageGoose.ProceduralLineNetwork.Component.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Manager
{
    public interface ILineNetworkKeyGen
    {
        public uint GenerateKey();
    }

    /// <summary>
    /// Fastest key generator. No duplicate checks or overflow protection.
    /// </summary>
    public class FastKeyGen : ILineNetworkKeyGen
    {
        private uint Keys = 0;
        public uint GenerateKey() => Keys++;
    }

    /// <summary>
    /// Fastest key generator for a multithreaded line network. No duplicate checks or overflow protection.
    /// </summary>
    public class FastMultithreadKeyGen : ILineNetworkKeyGen
    {
        private uint Keys = 0;
        public uint GenerateKey() => Interlocked.Increment(ref Keys);
    }
}
