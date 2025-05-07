using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Expand the line network on the selected point with flexibility using additional components.
    /// </summary>
    public class AddLinesOnPoint
    {
        /// <summary>
        /// Add a new line for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="AngularBiasComponents">Components to determine the angular bias.</param>
        /// <param name="LineLengthBiasComponents">Components to determine the line length bias.</param>
        /// <param name="MaxLength">Maximum length of the line</param>
        public void Modify(IEnumerable<uint> TargetPoints, IPointAngleBias[] AngularBiasComponents, ILineLengthBias[] LineLengthBiasComponents, float MaxLength)
        {

        }

        /// <summary>
        /// Add a new line with pre-determined angles for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="AngularBiasComponents">Components to determine the angular bias.</param>
        /// <param name="Length">Length of the line</param>
        public void Modify(IEnumerable<uint> TargetPoints, IPointAngleBias[] AngularBiasComponents, float Length)
        {

        }

        /// <summary>
        /// Add a new line with pre-determined length for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="LineLengthBiasComponents">Components to determine the line length bias.</param>
        /// <param name="Angle">Angle of the point</param>
        public void Modify(IEnumerable<uint> TargetPoints, ILineLengthBias[] LineLengthBiasComponents, float Angle)
        {

        }
    }

    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for angular bias.
    /// </summary>
    public interface IPointAngleBias
    {
        /// <param name="pointKey">Current point's key</param>
        /// <returns>Determined angular bias</returns>
        public NewLineBias GetLineAngularBias(uint pointKey, Point targetPoint);
    }

    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for line length bias.
    /// </summary>
    public interface ILineLengthBias
    {
        /// <param name="pointKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public NewLineBias GetLineLengthBias(uint lineKey, Line TargetLine, float angle);
    }

    /// <summary>
    /// Class for holding information about angular bias for <code>LineLengthAngularBias</code>.
    /// </summary>
    public class NewLineBias
    {
        /// <summary>
        /// Bias range starting point.
        /// </summary>
        public float from;

        /// <summary>
        /// Bias range endpoint.
        /// </summary>
        public float to;

        /// <summary>
        /// Bias intensity from -1 to 1. 
        /// from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float bias;

        /// <param name="from">Bias range starting point.</param>
        /// <param name="to">Bias range endpoint.</param>
        /// <param name="bias">
        /// Bias from -1 to 1. from the value being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public NewLineBias(float from, float to, float bias)
        {
            this.from = from;
            this.to = to;
            this.bias = bias;
        }
    }
}
