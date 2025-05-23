using GarageGoose.ProceduralLineNetwork.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Interface for AddLinesOnPoint components for line length bias.
    /// </summary>
    public interface ILengthBiasComponent
    {
        /// <param name="lineKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public IBiasSegmentContainer GetBias(uint lineKey, Line targetLine, float angle);
    }

    /// <summary>
    /// Interface for AddLinesOnPoint components for angular bias.
    /// </summary>
    public interface IAngleBiasComponent
    {
        public IBiasSegmentContainer GetBias(uint pointKey, Point targetPoint);
    }
}
