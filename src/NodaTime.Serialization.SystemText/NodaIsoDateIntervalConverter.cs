// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemText
{
    /// <summary>
    /// System.Text.Json converter for <see cref="DateInterval"/>.
    /// </summary>
    internal sealed class NodaIsoDateIntervalConverter : NodaConverterBase<DateInterval>
    {
        /// <summary>
        /// Reads Start and End properties for the start and end of an interval, converting them to instants
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <returns>The <see cref="Interval"/> identified in the JSON.</returns>
        protected override DateInterval ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidNodaDataException(
                    $"Unexpected token parsing DateInterval. Expected String, got {reader.TokenType}.");
            }
            string text = reader.GetString();
            int slash = text.IndexOf('/');
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; slash was missing.");
            }

            string startText = text.Substring(0, slash);
            if (startText == "")
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; start date was missing.");
            }

            string endText = text.Substring(slash + 1);
            if (endText == "")
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; end date was missing.");
            }

            var pattern = LocalDatePattern.Iso;
            var start = pattern.Parse(startText).Value;
            var end = pattern.Parse(endText).Value;

            return new DateInterval(start, end);
        }

        /// <summary>
        /// Serializes the date interval as start/end dates.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The date interval to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateInterval value, JsonSerializerOptions options)
        {
            var pattern = LocalDatePattern.Iso;
            string text = pattern.Format(value.Start) + "/" + pattern.Format(value.End);
            writer.WriteStringValue(text);
        }
    }
}
