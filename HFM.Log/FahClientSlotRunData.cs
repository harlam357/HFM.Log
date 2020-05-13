
namespace HFM.Log
{
    /// <summary>
    /// Represents v7 or newer client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="SlotRun"/> object.
    /// </summary>
    public class FahClientSlotRunData : SlotRunData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientSlotRunData"/> class.
        /// </summary>
        public FahClientSlotRunData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientSlotRunData"/> class.
        /// </summary>
        /// <param name="other">The other instance from which data will be copied.</param>
        public FahClientSlotRunData(FahClientSlotRunData other)
            : base(other)
        {
            
        }
    }
}