namespace ProceduralLineNetwork.CoreComponents.LineNetworkModification
{
    /// <summary>
    /// Expand the line network on the selected point using additional components.
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
        public void Modify(IEnumerable<uint> TargetPoints, IPointAngularBias[] AngularBiasComponents, ILineLengthBias[] LineLengthBiasComponents, float MaxLength)
        {

        }

        /// <summary>
        /// Add a new line with pre-determined angles for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="AngularBiasComponents">Components to determine the angular bias.</param>
        /// <param name="Length">Length of the line</param>
        public void Modify(IEnumerable<uint> TargetPoints, IPointAngularBias[] AngularBiasComponents, float Length)
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
    public interface IPointAngularBias
    {
        /// <param name="pointKey">Current point's key</param>
        /// <returns>Determined angular bias</returns>
        public AngularBias GetLineAngularBias(uint pointKey);
    }

    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for line length bias.
    /// </summary>
    public interface ILineLengthBias
    {
        /// <param name="pointKey">Current point's key</param>
        /// <param name="angle">Chosen angle</param>
        /// <returns>Determinedd line length bias</returns>
        public LengthBias GetLineLengthBias(uint pointKey, float angle);
    }

    /// <summary>
    /// Class for holding information about angular bias for <code>LineLengthAngularBias</code>
    /// </summary>
    public class AngularBias
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
        /// from the angle being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float bias;

        /// <param name="from">Bias range starting point.</param>
        /// <param name="to">Bias range endpoint.</param>
        /// <param name="bias">
        /// Bias angle from -1 to 1. from the angle being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public AngularBias(float from, float to, float bias)
        {
            this.from = from;
            this.to = to;
            this.bias = bias;
        }
    }

    /// <summary>
    /// Class for holding information about line legnth bias for <code>LineLengthAngularBias</code>
    /// </summary>
    public class LengthBias
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
        /// from the length being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </summary>
        public float intensity;

        /// <param name="from">Bias range starting point.</param>
        /// <param name="to">Bias range endpoint.</param>
        /// <param name="bias">
        /// Bias intensity from -1 to 1. from the length being inside the bias range at 1, being twice as likely to be in the bias range at 0.5, to being outside the bias range at -1.
        /// </param>
        public LengthBias(float from, float to, float bias)
        {
            this.from = from;
            this.to = to;
        }
    }
}
