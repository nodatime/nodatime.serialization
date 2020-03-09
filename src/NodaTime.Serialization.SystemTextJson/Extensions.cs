// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// Static class containing extension methods to configure System.Text.Json for Noda Time types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Configures System.Text.Json with everything required to properly serialize and deserialize NodaTime data types.
        /// </summary>
        /// <param name="options">The existing options to add Noda Time converters to.</param>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions ConfigureForNodaTime(this JsonSerializerOptions options, IDateTimeZoneProvider provider)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            // Add our converters
            AddDefaultConverters(options.Converters, provider);

            // return to allow fluent chaining if desired
            return options;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaConverters.IsoIntervalConverter"/>.
        /// Any other converters which can convert <see cref="Interval"/> are removed from the serializer.
        /// </summary>
        /// <param name="options">The existing serializer settings to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions WithIsoIntervalConverter(this JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            ReplaceExistingConverters<Interval>(options.Converters, NodaConverters.IsoIntervalConverter);
            return options;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaConverters.IsoDateIntervalConverter"/>.
        /// Any other converters which can convert <see cref="DateInterval"/> are removed from the serializer.
        /// </summary>
        /// <param name="options">The existing serializer settings to add Noda Time converters to.</param>
        /// <returns>The original <paramref name="options"/> value, for further chaining.</returns>
        public static JsonSerializerOptions WithIsoDateIntervalConverter(this JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            ReplaceExistingConverters<DateInterval>(options.Converters, NodaConverters.IsoDateIntervalConverter);
            return options;
        }

        private static void AddDefaultConverters(IList<JsonConverter> converters, IDateTimeZoneProvider provider)
        {
            converters.Add(NodaConverters.InstantConverter);
            converters.Add(NodaConverters.IntervalConverter);
            converters.Add(NodaConverters.LocalDateConverter);
            converters.Add(NodaConverters.LocalDateTimeConverter);
            converters.Add(NodaConverters.LocalTimeConverter);
            converters.Add(NodaConverters.AnnualDateConverter);
            converters.Add(NodaConverters.DateIntervalConverter);
            converters.Add(NodaConverters.OffsetConverter);
            converters.Add(NodaConverters.CreateDateTimeZoneConverter(provider));
            converters.Add(NodaConverters.DurationConverter);
            converters.Add(NodaConverters.RoundtripPeriodConverter);
            converters.Add(NodaConverters.OffsetDateTimeConverter);
            converters.Add(NodaConverters.OffsetDateConverter);
            converters.Add(NodaConverters.OffsetTimeConverter);
            converters.Add(NodaConverters.CreateZonedDateTimeConverter(provider));
        }

        private static void ReplaceExistingConverters<T>(IList<JsonConverter> converters, JsonConverter newConverter)
        {
            for (int i = converters.Count - 1; i >= 0; i--)
            {
                if (converters[i].CanConvert(typeof(T)))
                {
                    converters.RemoveAt(i);
                }
            }
            converters.Add(newConverter);
        }

        /// <summary>
        /// Resolves property name according <see cref="JsonSerializerOptions.PropertyNamingPolicy"/>.
        /// <para>If <see cref="JsonSerializerOptions.PropertyNamingPolicy"/> is not specified then original <paramref name="propertyName"/> returns.</para>
        /// </summary>
        /// <param name="serializerOptions">The serializer options to use name resolve.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Resolved or original property name.</returns>
        internal static string ResolvePropertyName(this JsonSerializerOptions serializerOptions, string propertyName) =>
            (serializerOptions.PropertyNamingPolicy)?.ConvertName(propertyName) ?? propertyName;

        /// <summary>
        /// Retrieves the <see cref="JsonConverter"/> from <paramref name="serializerOptions"/> and deserializes the object as <typeparamref name="T"/>.
        /// </summary>
        /// <param name="serializerOptions">The serializer options to use.</param>
        /// <param name="reader">Json reader.</param>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        internal static T ReadType<T>(this JsonSerializerOptions serializerOptions, ref Utf8JsonReader reader)
        {
            var converter = (JsonConverter<T>)serializerOptions.GetConverter(typeof(T));
            return converter.Read(ref reader, typeof(T), serializerOptions);
        }

        /// <summary>
        /// Retrieves the <see cref="JsonConverter"/> from <paramref name="serializerOptions"/> and serializes the object as <typeparamref name="T"/>.
        /// </summary>
        /// <param name="serializerOptions">The serializer options to use.</param>
        /// <param name="writer">Json writer.</param>
        /// <param name="value">The value to serialize</param>
        /// <typeparam name="T">The type of object to write.</typeparam>
        internal static void WriteType<T>(this JsonSerializerOptions serializerOptions, Utf8JsonWriter writer, T value)
        {
            var converter = (JsonConverter<T>)serializerOptions.GetConverter(typeof(T));
            converter.Write(writer, value, serializerOptions);
        }
    }
}