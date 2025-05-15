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
        public void Modify(IEnumerable<uint> TargetPoints, IBiasSegmentComponent[] AngularBiasComponents, IBiasSegmentComponent[] LineLengthBiasComponents, float MaxLength)
        {

        }

        /// <summary>
        /// Add a new line with pre-determined angles for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="AngularBiasComponents">Components to determine the angular bias.</param>
        /// <param name="Length">Length of the line</param>
        public void ModifyPresetAngle(IEnumerable<uint> TargetPoints, IBiasSegmentComponent[] AngularBiasComponents, float Length)
        {

        }

        /// <summary>
        /// Add a new line with pre-determined length for each selected points.
        /// </summary>
        /// <param name="TargetPoints">Keys of the point where to add lines.</param>
        /// <param name="LineLengthBiasComponents">Components to determine the line length bias.</param>
        /// <param name="Angle">Angle of the point</param>
        public void ModifyPresetLength(IEnumerable<uint> TargetPoints, IBiasSegmentComponent[] LineLengthBiasComponents, float Angle)
        {

        }
    }






}
