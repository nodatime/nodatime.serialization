// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Utility;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Json.NET converter for <see cref="DateInterval"/> using a compound representation. The start and
    /// end aspects of the date interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="DateInstant"/> converter for the serializer provided.
    /// </summary>   
    internal sealed class NodaDateIntervalConverter : NodaConverterBase<DateInterval>
    {
        /// <summary>
        /// Reads Start and End properties for the start and end of a date interval, converting them to local dates
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        /// <returns>The <see cref="DateInterval"/> identified in the JSON.</returns>
        protected override DateInterval ReadJsonImpl(JsonReader reader, JsonSerializer serializer)
        {
            LocalDate? startLocalDate = null;
            LocalDate? endLocalDate = null;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var propertyName = (string)reader.Value;
                if (!reader.Read())
                {
                    break;
                }

                if (propertyName == "Start")
                {
                    startLocalDate = serializer.Deserialize<LocalDate>(reader);
                }

                if (propertyName == "End")
                {
                    endLocalDate = serializer.Deserialize<LocalDate>(reader);
                }
            }

            if (!startLocalDate.HasValue)
            {
                throw new InvalidNodaDataException("Expected date interval; start date was missing.");
            }

            if (!endLocalDate.HasValue)
            {
                throw new InvalidNodaDataException("Expected date interval; end date was missing.");
            }

            return new DateInterval(startLocalDate.Value, endLocalDate.Value);
        }

        /// <summary>
        /// Serializes the date interval as start/end local dates.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The date interval to serialize</param>
        /// <param name="serializer">The serializer for embedded serialization.</param>
        protected override void WriteJsonImpl(JsonWriter writer, DateInterval value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Start");
            serializer.Serialize(writer, value.Start);

            writer.WritePropertyName("End");
            serializer.Serialize(writer, value.End);

            writer.WriteEndObject();
        }
    }
}
