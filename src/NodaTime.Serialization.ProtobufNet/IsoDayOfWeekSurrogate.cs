using System.Runtime.Serialization;
using NodaTime.Serialization.Protobuf;
using ProtobufDayOfWeek = Google.Type.DayOfWeek;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Surrogate for <see cref="IsoDayOfWeek"/>.
    /// </summary>
    [DataContract]
    public class IsoDayOfWeekSurrogate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IsoDayOfWeekSurrogate()
        {
        }

        /// <summary>
        /// Constructor overload. 
        /// </summary>
        public IsoDayOfWeekSurrogate(IsoDayOfWeek value) => Value = value.ToProtobufDayOfWeek();

        /// <summary>
        /// Stores the Google Protobuf DayOfWeek value for serialization.
        /// </summary>
        public ProtobufDayOfWeek Value { get; set; }

        /// <summary>
        /// Converts the surrogate to an <see cref="IsoDayOfWeek"/>.
        /// </summary>
        public static implicit operator IsoDayOfWeek(IsoDayOfWeekSurrogate surrogate) => surrogate.Value.ToIsoDayOfWeek();

        /// <summary>
        ///  Converts the <see cref="IsoDayOfWeek"/> to a surrogate.
        /// </summary>
        public static implicit operator IsoDayOfWeekSurrogate(IsoDayOfWeek source) => new IsoDayOfWeekSurrogate(source);
    }
}