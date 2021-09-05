
using System;

namespace HFM.Log
{
    /// <summary>
    /// Represents the types of log lines that can be detected by the HFM.Log API.
    /// Once the type has been identified a parser may be assigned to find data in the line text.
    /// </summary>
    public readonly struct LogLineType : IEquatable<LogLineType>
    {
        /// <summary>
        /// Line does not contain any information or indicate any condition of interest.
        /// </summary>
        public const int None = 0;
        /// <summary>
        /// Line is a log opening line containing client start date and time (FahClient and Legacy clients)
        /// </summary>
        public const int LogOpen = 1;
        /// <summary>
        /// Line indicates the client is sending work to server (FahClient and Legacy clients)
        /// </summary>
        public const int ClientSendWorkToServer = 2;
        /// <summary>
        /// Line contains client argument information (Legacy clients only)
        /// </summary>
        public const int ClientArguments = 3;
        /// <summary>
        /// Line indicates the client is attempting to get a work packet (FahClient and Legacy clients)
        /// </summary>
        public const int ClientAttemptGetWorkPacket = 4;
        /// <summary>
        /// Line indicates the client has begun working on a work unit (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitWorking = 5;
        /// <summary>
        /// Line "*------------------------------*" indicates the client core process has begun working on a work unit (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitCoreStart = 6;
        /// <summary>
        /// Line contains core executable version information (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitCoreVersion = 7;
        ///// <summary>
        ///// Line indicates the client core process did not fail to start and is running (FahClient and Legacy clients)
        ///// </summary>
        //public const int WorkUnitRunning = 8;
        /// <summary>
        /// Line contains work unit project information (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitProject = 9;
        /// <summary>
        /// Line contains work unit frame (progress) information (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitFrame = 10;
        /// <summary>
        /// Line contains client core process result string (FahClient and Legacy clients)
        /// </summary>
        public const int WorkUnitCoreShutdown = 11;
        /// <summary>
        /// Line contains the client echo of the core process result string (FahClient clients only)
        /// </summary>
        public const int WorkUnitCoreReturn = 12;
        /// <summary>
        /// Line indicates work unit processing is complete (FahClient clients only)
        /// </summary>
        public const int WorkUnitCleaningUp = 13;
        /// <summary>
        /// Line indicates a client-core communications error (Legacy clients only)
        /// </summary>
        public const int ClientCoreCommunicationsError = 14;
        /// <summary>
        /// Line indicates a client-core communications error which caused the client to shutdown (Legacy clients only)
        /// </summary>
        public const int ClientCoreCommunicationsErrorShutdown = 15;
        /// <summary>
        /// Line indicates the client has encountered too many EARLY_UNIT_END results from client core processes and will pause activity for 24 hours (Legacy clients only)
        /// </summary>
        public const int ClientEuePauseState = 16;
        /// <summary>
        /// Line indicates the client has been shutdown (Legacy clients only)
        /// </summary>
        public const int ClientShutdown = 17;
        /// <summary>
        /// Line indicates the client detected too many failures to run the same work unit (FahClient clients only)
        /// </summary>
        public const int WorkUnitTooManyErrors = 18;

        private readonly int _value;

        private LogLineType(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(LogLineType other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is LogLineType other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return _value;
        }

#pragma warning disable 1591
        public static bool operator ==(LogLineType left, LogLineType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LogLineType left, LogLineType right)
        {
            return !left.Equals(right);
        }

        public static implicit operator int(LogLineType logLineType)
        {
            return logLineType._value;
        }

        public static implicit operator LogLineType(int value)
        {
            return new LogLineType(value);
        }
#pragma warning restore 1591
    }
}
