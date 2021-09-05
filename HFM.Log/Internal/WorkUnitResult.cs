
namespace HFM.Log.Internal
{
   internal static class WorkUnitResult
   {
      internal const string None = null;
      internal const string FINISHED_UNIT = "FINISHED_UNIT";
      internal const string EARLY_UNIT_END = "EARLY_UNIT_END";
      internal const string UNSTABLE_MACHINE = "UNSTABLE_MACHINE";
      internal const string INTERRUPTED = "INTERRUPTED";
      internal const string BAD_WORK_UNIT = "BAD_WORK_UNIT";
      internal const string UNKNOWN_ENUM = "UNKNOWN_ENUM";
      internal const string CORE_RESTART = "CORE_RESTART";
      internal const string BAD_FRAME_CHECKSUM = "BAD_FRAME_CHECKSUM";

      //internal const string CORE_OUTDATED = "CORE_OUTDATED";
      //internal const string GPU_MEMTEST_ERROR = "GPU_MEMTEST_ERROR";
   }
}
