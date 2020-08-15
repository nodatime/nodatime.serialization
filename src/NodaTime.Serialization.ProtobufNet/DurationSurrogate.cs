using System.Runtime.Serialization;
using NodaTime.Serialization.Protobuf;
using NodaDuration = NodaTime.Duration;
using ProtobufDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Surrogate for <see cref="NodaTime.Duration"/>.
    /// </summary>
    [DataContract]
    public class DurationSurrogate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DurationSurrogate()
        {
        }

        /// <summary>
        /// Constructor overload. 
        /// </summary>
        public DurationSurrogate(NodaDuration value) => Value = value.ToProtobufDuration();

        /// <summary>
        /// Stores the Google Protobuf Duration value for serialization.
        /// </summary>
        public ProtobufDuration Value { get; set; } = default!;

        /// <summary>
        /// Converts the surrogate to a <see cref="NodaTime.Duration"/>.
        /// </summary>
        public static implicit operator NodaDuration(DurationSurrogate surrogate) => surrogate.Value.ToNodaDuration();

        /// <summary>
        ///  Converts the <see cref="NodaTime.Duration"/> to a surrogate.
        /// </summary>
        public static implicit operator DurationSurrogate(NodaDuration source) => new DurationSurrogate(source);
    }
}