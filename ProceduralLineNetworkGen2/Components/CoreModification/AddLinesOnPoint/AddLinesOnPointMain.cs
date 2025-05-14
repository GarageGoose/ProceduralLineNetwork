using GarageGoose.ProceduralLineNetwork.Elements;

namespace GarageGoose.ProceduralLineNetwork.Component.Core
{
    /// <summary>
    /// Expand the line network on the selected point with flexibility using additional components.
    /// </summary>
    public class AddLinesOnPointMain
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
    /// 
    /// </summary>
    public enum BiasSegmentCollisionAction
    {
        /// <summary>
        /// Make a new segment at the collision area that blends the values of both the current and the collider bias points.
        /// </summary>
        Blend,

        /// <summary>
        /// 
        /// </summary>
        PrioritizeThis,

        /// <summary>
        /// 
        /// </summary>
        PrioritizeColliding,

        /// <summary>
        /// 
        /// </summary>
        AdjustToMidpoint,

        /// <summary>
        /// 
        /// </summary>
        CombineBlend,

        /// <summary>
        /// 
        /// </summary>
        CombinePrioritizeThis,

        /// <summary>
        /// 
        /// </summary>
        CombinePrioritizeColliding
    }

    /// <summary>
    /// Interface for <code>AddLinesOnPoint</code> components for angular bias.
    /// </summary>
    public interface IPointAngleBias
    {
        /// <param name="pointKey">Current point's key</param>
        /// <returns>Determined angular bias</returns>
        public LineLengthBias GetLineAngularBias(uint pointKey, Point targetPoint);
    }


}
