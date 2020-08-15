using System.Runtime.Serialization;
using Google.Protobuf.WellKnownTypes;
using NodaTime.Serialization.Protobuf;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Surrogate for <see cref="Instant"/>.
    /// </summary>
    [DataContract]
    public class InstantSurrogate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstantSurrogate()
        {
        }

        /// <summary>
        /// Constructor overload. 
        /// </summary>
        public InstantSurrogate(Instant value) => Value = value.ToTimestamp();

        /// <summary>
        /// Stores the Google Protobuf Timestamp value for serialization.
        /// </summary>
        public Timestamp Value { get; set; } = default!;

        /// <summary>
        /// Converts the surrogate to an <see cref="Instant"/>.
        /// </summary>
        public static implicit operator Instant(InstantSurrogate surrogate) => surrogate.Value.ToInstant();

        /// <summary>
        ///  Converts the <see cref="Instant"/> to a surrogate.
        /// </summary>
        public static implicit operator InstantSurrogate(Instant source) => new InstantSurrogate(source);
    }
}