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
/// This attribute allows JSON conversion to be easily specified for properties without
/// having to configure a specific options object.
/// </remarks>
public sealed class NodaTimeDefaultJsonConverterAttribute : JsonConverterAttribute
{
    private static readonly Dictionary<Type, JsonConverter> converters;

    /// <summary>
    /// Constructs an instance of the attribute.
    /// </summary>
    public NodaTimeDefaultJsonConverterAttribute()
    {
    }

    static NodaTimeDefaultJsonConverterAttribute()
    {
        converters = new()
        {
            { typeof(AnnualDate), NodaConverters.AnnualDateConverter },
            { typeof(DateInterval), NodaConverters.DateIntervalConverter },
            { typeof(DateTimeZone), NodaConverters.CreateDateTimeZoneConverter(DateTimeZoneProviders.Tzdb) },
            { typeof(Duration), NodaConverters.DurationConverter },
            { typeof(Instant), NodaConverters.InstantConverter },
            { typeof(Interval), NodaConverters.IntervalConverter },
            { typeof(LocalDate), NodaConverters.LocalDateConverter },
            { typeof(LocalDateTime), NodaConverters.LocalDateTimeConverter },
            { typeof(LocalTime), NodaConverters.LocalTimeConverter },
            { typeof(Offset), NodaConverters.OffsetConverter },
            { typeof(OffsetDate), NodaConverters.OffsetDateConverter },
            { typeof(OffsetDateTime), NodaConverters.OffsetDateTimeConverter },
            { typeof(OffsetTime), NodaConverters.OffsetTimeConverter },
            { typeof(Period), NodaConverters.RoundtripPeriodConverter },
            { typeof(ZonedDateTime), NodaConverters.CreateZonedDateTimeConverter(DateTimeZoneProviders.Tzdb) }
        };
        // Use the same converter for Nullable<T> as T.
        foreach (var entry in converters.Where(pair => pair.Key.IsValueType).ToList())
        {
            converters[typeof(Nullable<>).MakeGenericType(entry.Key)] = entry.Value;
        }
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert) =>
        converters.TryGetValue(typeToConvert, out var converter) ? converter : null;
}