using System.Runtime.Serialization;
using Google.Type;
using NodaTime.Serialization.Protobuf;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Surrogate for <see cref="LocalTime"/>.
    /// </summary>
    [DataContract]
    public class LocalTimeSurrogate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalTimeSurrogate()
        {
        }

        /// <summary>
        /// Constructor overload. 
        /// </summary>
        public LocalTimeSurrogate(LocalTime value) => Value = value.ToTimeOfDay();

        /// <summary>
        /// Stores the Google Protobuf TimeOfDay value for serialization.
        /// </summary>
        public TimeOfDay Value { get; set; } = default!;

        /// <summary>
        /// Converts the surrogate to a <see cref="LocalTime"/>.
        /// </summary>
        public static implicit operator LocalTime(LocalTimeSurrogate surrogate) => surrogate.Value.ToLocalTime();

        /// <summary>
        ///  Converts the <see cref="LocalTime"/> to a surrogate.
        /// </summary>
        public static implicit operator LocalTimeSurrogate(LocalTime source) => new LocalTimeSurrogate(source);
    }
}