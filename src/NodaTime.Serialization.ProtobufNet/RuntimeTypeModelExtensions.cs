using System;
using System.Collections.Generic;
using ProtoBuf.Meta;

namespace NodaTime.Serialization.ProtobufNet
{
    /// <summary>
    /// Provides an extension method to conveniently register all NodaTime surrogate types with the default <see cref="RuntimeTypeModel"/>.
    /// </summary>
    public static class RuntimeTypeModelExtensions
    {
        private static readonly IDictionary<Type, Type> SurrogateMapping = new Dictionary<Type, Type>
        {
            [typeof(Duration)] = typeof(DurationSurrogate),
            [typeof(Instant)] = typeof(InstantSurrogate),
            [typeof(LocalTime)] = typeof(LocalTimeSurrogate),
            [typeof(LocalDate)] = typeof(LocalDateSurrogate),
            [typeof(IsoDayOfWeek)] = typeof(IsoDayOfWeekSurrogate),
        };

        /// <summary>
        /// Register all NodaTime surrogate types with the protobuf runtime model.
        /// </summary>
        public static RuntimeTypeModel AddNodaTimeSurrogates(this RuntimeTypeModel runtimeTypeModel)
        {
            foreach (var map in SurrogateMapping) 
                runtimeTypeModel.Add(map.Key, false).SetSurrogate(map.Value);
            
            return runtimeTypeModel;
        }
    }
}