
namespace HFM.Log
{
    /// <summary>
    /// Represents v7 or newer client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="UnitRun"/> object.
    /// </summary>
    public class FahClientUnitRunData : UnitRunData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientUnitRunData"/> class.
        /// </summary>
        public FahClientUnitRunData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientUnitRunData"/> class.
        /// </summary>
        /// <param name="other">The other instance from which data will be copied.</param>
        public FahClientUnitRunData(FahClientUnitRunData other)
            : base(other)
        {
            
        }
    }
}