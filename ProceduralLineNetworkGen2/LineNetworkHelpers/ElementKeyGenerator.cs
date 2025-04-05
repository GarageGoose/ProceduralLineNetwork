using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork
{
    public class ElementKeyGenerator
    {
        private uint Keys = 0;
        public uint GenerateKey()
        {
            return Keys++;
        }
    }
}
