// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Json.NET converter for <see cref="Interval"/> using a compound representation. The start and
    /// end aspects of the interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="Instant"/> converter for the serializer provided.
    /// </summary>   
    internal sealed class NodaIntervalConverter : NodaConverterBase<Interval>
    {
        /// <summary>
        /// Reads Start and End properties for the start and end of an interval, converting them to instants
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        /// <returns>The <see cref="Interval"/> identified in the JSON.</returns>
        protected override Interval ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            Instant? startInstant = null;
            Instant? endInstant = null;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var propertyName = (string)reader.Value;
                // If we haven't got a property value, that's pretty weird. Break out of the loop,
                // and let JSON.NET fail appropriately...
                if (!reader.Read())
                {
                    break;
                }

                var startPropertyName = serializer.ResolvePropertyName(nameof(Interval.Start));
                if (propertyName == startPropertyName)
                {
                    startInstant = serializer.Deserialize<Instant>(reader);
                }

                var endPropertyName = serializer.ResolvePropertyName(nameof(Interval.End));
                if (propertyName == endPropertyName)
                {
                    endInstant = serializer.Deserialize<Instant>(reader);
                }
            }

            return new Interval(startInstant, endInstant);
        }

        /// <summary>
        /// Serializes the interval as start/end instants.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The interval to serialize</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        protected override void WriteJsonImpl(JsonWriter writer, Interval value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.HasStart)
            {
                var startPropertyName = serializer.ResolvePropertyName(nameof(Interval.Start));
                writer.WritePropertyName(startPropertyName);
                serializer.Serialize(writer, value.Start);
            }
            if (value.HasEnd)
            {
                var endPropertyName = serializer.ResolvePropertyName(nameof(Interval.End));
                writer.WritePropertyName(endPropertyName);
                serializer.Serialize(writer, value.End);
            }
            writer.WriteEndObject();
        }
    }
}
