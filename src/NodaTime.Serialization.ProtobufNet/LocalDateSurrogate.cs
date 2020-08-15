using System.Runtime.Serialization;
using Google.Type;
using NodaTime.Serialization.Protobuf;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Surrogate for <see cref="LocalDate"/>.
    /// </summary>
    [DataContract]
    public class LocalDateSurrogate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalDateSurrogate()
        {
        }

        /// <summary>
        /// Constructor overload. 
        /// </summary>
        public LocalDateSurrogate(LocalDate value) => Value = value.ToDate();

        /// <summary>
        /// Stores the Google Protobuf TimeOfDay value for serialization.
        /// </summary>
        public Date Value { get; set; } = default!;

        /// <summary>
        /// Converts the surrogate to a <see cref="LocalDate"/>.
        /// </summary>
        public static implicit operator LocalDate(LocalDateSurrogate surrogate) => surrogate.Value.ToLocalDate();

        /// <summary>
        ///  Converts the <see cref="LocalDate"/> to a surrogate.
        /// </summary>
        public static implicit operator LocalDateSurrogate(LocalDate source) => new LocalDateSurrogate(source);
    }
}