// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// System.Text.Json converter for <see cref="Interval"/> using a compound representation. The start and
    /// end aspects of the interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="Instant"/> converter for the serializer provided.
    /// </summary>
    internal sealed class NodaIntervalConverter : NodaConverterBase<Interval>
    {
        /// <summary>
        /// Instant converter to use, overriding whatever is in JsonSerializerOptions.
        /// </summary>
        private readonly JsonConverter<Instant> instantConverter;

        internal NodaIntervalConverter() : this(null)
        {
        }

        internal NodaIntervalConverter(JsonConverter<Instant> instantConverter)
        {
            this.instantConverter = instantConverter;
        }

        /// <summary>
        /// Reads Start and End properties for the start and end of an interval, converting them to instants
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <returns>The <see cref="Interval"/> identified in the JSON.</returns>
        protected override Interval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Instant? startInstant = null;
            Instant? endInstant = null;
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    break;
                }

                var propertyName = reader.GetString();
                // If we haven't got a property value, that's pretty weird. Break out of the loop,
                // and let the underlying framework fail appropriately...
                if (!reader.Read())
                {
                    break;
                }

                var caseSensitivity = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                var startPropertyName = options.ResolvePropertyName(nameof(Interval.Start));
                if (string.Equals(propertyName, startPropertyName, caseSensitivity))
                {
                    startInstant = options.ReadType<Instant>(instantConverter, ref reader);
                }

                var endPropertyName = options.ResolvePropertyName(nameof(Interval.End));
                if (string.Equals(propertyName, endPropertyName, caseSensitivity))
                {
                    endInstant = options.ReadType<Instant>(instantConverter, ref reader);
                }
            }

            return new Interval(startInstant, endInstant);
        }

        /// <summary>
        /// Serializes the interval as start/end instants.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The interval to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.HasStart)
            {
                var startPropertyName = options.ResolvePropertyName(nameof(Interval.Start));
                writer.WritePropertyName(startPropertyName);
                options.WriteType(instantConverter, writer, value.Start);
            }
            if (value.HasEnd)
            {
                var endPropertyName = options.ResolvePropertyName(nameof(Interval.End));
                writer.WritePropertyName(endPropertyName);
                options.WriteType(instantConverter, writer, value.End);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Unconditionally throws an exception, as an Interval cannot be serialized as a JSON property name.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The date interval to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <exception cref="InvalidOperationException">Always thrown to indicate this is not an appropriate method to call on this type.</exception>
        protected override void WriteJsonPropertyNameImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options) =>
            throw new JsonException("Cannot serialize an Interval as a JSON property name using this converter");
    }
}
