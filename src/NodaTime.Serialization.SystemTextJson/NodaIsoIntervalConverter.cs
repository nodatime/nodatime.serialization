// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// System.Text.Json converter for <see cref="Interval"/>.
    /// </summary>
    internal sealed class NodaIsoIntervalConverter : NodaConverterBase<Interval>
    {
        /// <summary>
        /// Reads Start and End properties for the start and end of an interval, converting them to instants
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <returns>The <see cref="Interval"/> identified in the JSON.</returns>
        protected override Interval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidNodaDataException(
                    $"Unexpected token parsing Interval. Expected String, got {reader.TokenType}.");
            }
            string text = reader.GetString();
            int slash = text.IndexOf('/');
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing.");
            }

            string startText = text.Substring(0, slash);
            string endText = text.Substring(slash + 1);
            var pattern = InstantPattern.ExtendedIso;
            var start = startText == "" ? (Instant?) null : pattern.Parse(startText).Value;
            var end = endText == "" ? (Instant?) null : pattern.Parse(endText).Value;

            return new Interval(start, end);
        }

        /// <summary>
        /// Serializes the interval as start/end instants.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The interval to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options)
        {
            var pattern = InstantPattern.ExtendedIso;
            var text = $"{(value.HasStart ? pattern.Format(value.Start) : "")}/{(value.HasEnd ? pattern.Format(value.End) : "")}";
            writer.WriteStringValue(text);
        }

        protected override void WriteJsonPropertyNameImpl(Utf8JsonWriter writer, Interval value, JsonSerializerOptions options)
        {
            var pattern = InstantPattern.ExtendedIso;
            var text = $"{(value.HasStart ? pattern.Format(value.Start) : "")}/{(value.HasEnd ? pattern.Format(value.End) : "")}";
            writer.WritePropertyName(text);
        }
    }
}
