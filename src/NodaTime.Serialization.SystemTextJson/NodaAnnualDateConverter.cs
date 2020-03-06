// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// System.Text.Json converter for <see cref="AnnualDate"/> using a compound representation. The month and
    /// day aspects of the annual date are represented with separate properties, each parsed and formatted
    /// by the <see cref="int"/> converter for the serializer provided.
    /// </summary>
    internal sealed class NodaAnnualDateConverter : NodaConverterBase<AnnualDate>
    {
        /// <summary>
        /// Reads Month and Day properties for the month and day of an annual date, converting them to integers
        /// using the given serializer.
        /// </summary>
        /// <param name="reader">The JSON reader to fetch data from.</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        /// <returns>The <see cref="AnnualDate"/> identified in the JSON.</returns>
        protected override AnnualDate ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            int? month = null;
            int? day = null;
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

                var monthPropertyName = options.ResolvePropertyName(nameof(AnnualDate.Month));
                if (propertyName == monthPropertyName)
                {
                    month = options.ReadType<int>(ref reader);
                }

                var dayPropertyName = options.ResolvePropertyName(nameof(AnnualDate.Day));
                if (propertyName == dayPropertyName)
                {
                    day = options.ReadType<int>(ref reader);
                }
            }

            if (!month.HasValue)
            {
                throw new InvalidNodaDataException("Expected annual date; month was missing.");
            }

            if (!day.HasValue)
            {
                throw new InvalidNodaDataException("Expected annual date; day was missing.");
            }

            return new AnnualDate(month.Value, day.Value);
        }

        /// <summary>
        /// Serializes the annual date as month and day integers.
        /// </summary>
        /// <param name="writer">The writer to write JSON to</param>
        /// <param name="value">The annual date to serialize</param>
        /// <param name="options">The serializer options for embedded serialization.</param>
        protected override void WriteJsonImpl(Utf8JsonWriter writer, AnnualDate value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var monthPropertyName = options.ResolvePropertyName(nameof(AnnualDate.Month));
            writer.WritePropertyName(monthPropertyName);
            options.WriteType(writer, value.Month);

            var dayPropertyName = options.ResolvePropertyName(nameof(AnnualDate.Day));
            writer.WritePropertyName(dayPropertyName);
            options.WriteType(writer, value.Day);

            writer.WriteEndObject();
        }
    }
}
