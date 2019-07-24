// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.TimeZones;

namespace NodaTime.Serialization.SystemText
{
    /// <summary>
    /// Json.NET converter for <see cref="DateTimeZone"/>.
    /// </summary>
    internal sealed class NodaDateTimeZoneConverter : NodaConverterBase<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider provider;

        /// <param name="provider">Provides the <see cref="DateTimeZone"/> that corresponds to each time zone ID in the JSON string.</param>
        public NodaDateTimeZoneConverter(IDateTimeZoneProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Reads the time zone ID (which must be a string) from the reader, and converts it to a time zone.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serialization options to use.</param>
        /// <exception cref="DateTimeZoneNotFoundException">The provider does not support a time zone with the given ID.</exception>
        /// <returns>The <see cref="DateTimeZone"/> identified in the JSON, or null.</returns>
        protected override DateTimeZone ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Preconditions.CheckData(reader.TokenType == JsonTokenType.String,
                "Unexpected token parsing instant. Expected String, got {0}.",
                reader.TokenType);

            var timeZoneId = reader.GetString();
            return provider[timeZoneId];
        }

        /// <summary>
        /// Writes the time zone ID to the writer.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="options">The serialization options to use</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }
}
