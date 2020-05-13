
namespace HFM.Log
{
    /// <summary>
    /// Represents v7 or newer client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="ClientRun"/> object.
    /// </summary>
    public class FahClientClientRunData : ClientRunData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientClientRunData"/> class.
        /// </summary>
        public FahClientClientRunData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientClientRunData"/> class.
        /// </summary>
        /// <param name="other">The other instance from which data will be copied.</param>
        public FahClientClientRunData(FahClientClientRunData other)
            : base(other)
        {
            //if (other == null) return;
        }
    }
}