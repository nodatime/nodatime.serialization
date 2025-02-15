// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// System.Text.Json converter for <see cref="DateInterval"/> using a compound representation. The start and
    /// end aspects of the date interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="LocalDate"/> converter for the serializer provided.
    /// </summary>
    internal sealed class NodaDateIntervalConverter : NodaConverterBase<DateInterval>
    {
        /// <summary>
        /// LocalDate converter to use, overriding whatever is in JsonSerializerOptions.
        /// </summary>
        private readonly JsonConverter<LocalDate> localDateConverter;

        internal NodaDateIntervalConverter() : this(null)
        {
        }

        internal NodaDateIntervalConverter(JsonConverter<LocalDate> localDateConverter)
        {
            this.localDateConverter = localDateConverter;
        }

        /// <summary>
        /// Reads Start and End properties for the start and end of a date interval, converting them to local dates
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <returns>The <see cref="DateInterval"/> identified in the JSON.</returns>
        protected override DateInterval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            LocalDate? startLocalDate = null;
            LocalDate? endLocalDate = null;
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    break;
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    break;
                }

                var caseSensitivity = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                var startPropertyName = options.ResolvePropertyName(nameof(Interval.Start));
                if (string.Equals(propertyName, startPropertyName, caseSensitivity))
                {
                    startLocalDate = options.ReadType<LocalDate>(localDateConverter, ref reader);
                }

                var endPropertyName = options.ResolvePropertyName(nameof(Interval.End));
                if (string.Equals(propertyName, endPropertyName, caseSensitivity))
                {
                    endLocalDate = options.ReadType<LocalDate>(localDateConverter, ref reader);
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
        /// <param name="options">The serializer options for embedded serialization.</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var startPropertyName = options.ResolvePropertyName(nameof(Interval.Start));
            writer.WritePropertyName(startPropertyName);
            options.WriteType(localDateConverter, writer, value.Start);

            var endPropertyName = options.ResolvePropertyName(nameof(Interval.End));
            writer.WritePropertyName(endPropertyName);
            options.WriteType(localDateConverter, writer, value.End);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Unconditionally throws an exception, as a DateInterval cannot be serialized as a JSON property name.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The date interval to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <exception cref="InvalidOperationException">Always thrown to indicate this is not an appropriate method to call on this type.</exception>
        protected override void WriteJsonPropertyNameImpl(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options) =>
            throw new JsonException("Cannot serialize a DateInterval as a JSON property name using this converter");
    }
}
