// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson;

/// <summary>
/// Provides JSON default converters for Noda Time types, as if using serializer
/// options configured by <see cref="Extensions.ConfigureForNodaTime(JsonSerializerOptions, IDateTimeZoneProvider)"/>
/// with a provider of <see cref="DateTimeZoneProviders.Tzdb"/>.
/// </summary>
/// <remarks>
/// This is a factory equivalent of <see cref="NodaTimeDefaultJsonConverterAttribute"/>.
/// </remarks>
public sealed class NodaTimeDefaultJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// A dictionary of default converters, keyed by type. This includes nullable types.
    /// </summary>
    private static readonly Dictionary<Type, JsonConverter> converters = CreateConverterDictionary();

    /// <summary>
    /// Constructs an instance of the factory.
    /// </summary>
    public NodaTimeDefaultJsonConverterFactory()
    {
    }

    private static Dictionary<Type, JsonConverter> CreateConverterDictionary()
    {
        var converters = new Dictionary<Type, JsonConverter>();

        // Value types first, using the local function below to handle nullability.
        Add(NodaConverters.AnnualDateConverter);
        Add(NodaConverters.DurationConverterImpl);
        Add(NodaConverters.InstantConverter);
        Add(new NodaIntervalConverter(NodaConverters.InstantConverter));
        Add(NodaConverters.LocalDateConverter);
        Add(NodaConverters.LocalDateTimeConverter);
        Add(NodaConverters.LocalTimeConverter);
        Add(NodaConverters.OffsetConverter);
        Add(NodaConverters.OffsetDateConverter);
        Add(NodaConverters.OffsetDateTimeConverter);
        Add(NodaConverters.OffsetTimeConverter);
        Add(NodaConverters.CreateZonedDateTimeConverter(DateTimeZoneProviders.Tzdb));

        // Reference types
        converters[typeof(DateInterval)] = new NodaDateIntervalConverter(NodaConverters.LocalDateConverter);
        converters[typeof(DateTimeZone)] = NodaConverters.CreateDateTimeZoneConverter(DateTimeZoneProviders.Tzdb);
        converters[typeof(Period)] = NodaConverters.RoundtripPeriodConverter;
        return converters;

        // Adds the converter for a value type to the dictionary,
        // and a nullable converter for the corresponding nullable value type.
        void Add<T>(JsonConverter<T> converter) where T : struct
        {
            converters[typeof(T)] = converter;
            converters[typeof(T?)] = new NodaNullableConverter<T>(converter);
        }
    }

    /// <summary>
    /// Returns a converter for the given type, or null if no such converter is available.
    /// </summary>
    /// <param name="typeToConvert">The type to retrieve a converter for. This may
    /// be a nullable value type.</param>
    internal static JsonConverter GetConverter(Type typeToConvert) =>
        converters.TryGetValue(typeToConvert, out var converter) ? converter : null;

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => GetConverter(typeToConvert) is not null;

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        GetConverter(typeToConvert);
}