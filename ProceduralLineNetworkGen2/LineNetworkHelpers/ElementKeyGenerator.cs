using GarageGoose.ProceduralLineNetwork.Component.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Manager
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
