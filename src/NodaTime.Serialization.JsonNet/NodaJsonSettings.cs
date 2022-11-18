// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace NodaTime.Serialization.JsonNet;

/// <summary>
/// A collection of converters and related settings for
/// Noda Time JSON parsing. This can be used to configure Newtonsoft.Json
/// serializers using the <see cref="Extensions.ConfigureForNodaTime(JsonSerializer, NodaJsonSettings)"/>
/// and <see cref="Extensions.ConfigureForNodaTime(JsonSerializerSettings, NodaJsonSettings)"/> extension
/// methods.
/// </summary>
/// <remarks>
/// This type does not attempt to ensure any sort of thread safety.
/// The expect use is to create an instance, potentially modify some properties,
/// use it to configure a <see cref="JsonSerializer"/> or <see cref="JsonSerializerSettings"/>,
/// and then discard it.
/// </remarks>
public sealed class NodaJsonSettings
{
    /// <summary>
    /// The converter used for <see cref="Instant"/> values.
    /// </summary>
    public JsonConverter InstantConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="Interval"/> values.
    /// </summary>
    public JsonConverter IntervalConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="LocalDate"/> values.
    /// </summary>
    public JsonConverter LocalDateConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="LocalTime"/> values.
    /// </summary>
    public JsonConverter LocalTimeConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="LocalDateTime"/> values.
    /// </summary>
    public JsonConverter LocalDateTimeConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="AnnualDate"/> values.
    /// </summary>
    public JsonConverter AnnualDateConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="DateInterval"/> values.
    /// </summary>
    public JsonConverter DateIntervalConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="Offset"/> values.
    /// </summary>
    public JsonConverter OffsetConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="DateTimeZone"/> values.
    /// </summary>
    public JsonConverter DateTimeZoneConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="Duration"/> values.
    /// </summary>
    public JsonConverter DurationConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="Period"/> values.
    /// </summary>
    public JsonConverter PeriodConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="OffsetDate"/> values.
    /// </summary>
    public JsonConverter OffsetDateConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="OffsetTime"/> values.
    /// </summary>
    public JsonConverter OffsetTimeConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="OffsetDateTime"/> values.
    /// </summary>
    public JsonConverter OffsetDateTimeConverter { get; set; }

    /// <summary>
    /// The converter used for <see cref="ZonedDateTimeConverter"/> values.
    /// </summary>
    public JsonConverter ZonedDateTimeConverter { get; set; }

    /// <summary>
    /// Creates an instance with the default converters, using <see cref="DateTimeZoneProviders.Tzdb"/> for
    /// time zone conversions.
    /// </summary>
    public NodaJsonSettings() : this(DateTimeZoneProviders.Tzdb)
    {
    }

    /// <summary>
    /// Creates an instance with the default converters, using the specified <see cref="IDateTimeZoneProvider"/>
    /// for time zone conversions.
    /// </summary>
    /// <param name="provider">The time zone provider to use. Must not be null.
    /// </param>
    public NodaJsonSettings(IDateTimeZoneProvider provider)
    {
        Preconditions.CheckNotNull(provider, nameof(provider));
        InstantConverter = NodaConverters.InstantConverter;
        IntervalConverter = NodaConverters.IntervalConverter;
        LocalDateConverter = NodaConverters.LocalDateConverter;
        LocalTimeConverter = NodaConverters.LocalTimeConverter;
        LocalDateTimeConverter = NodaConverters.LocalDateTimeConverter;
        AnnualDateConverter = NodaConverters.AnnualDateConverter;
        DateIntervalConverter = NodaConverters.DateIntervalConverter;
        OffsetConverter = NodaConverters.OffsetConverter;
        DateTimeZoneConverter = NodaConverters.CreateDateTimeZoneConverter(provider);
        DurationConverter = NodaConverters.DurationConverter;
        PeriodConverter = NodaConverters.RoundtripPeriodConverter;
        OffsetDateConverter = NodaConverters.OffsetDateConverter;
        OffsetTimeConverter = NodaConverters.OffsetTimeConverter;
        OffsetDateTimeConverter = NodaConverters.OffsetDateTimeConverter;
        ZonedDateTimeConverter = NodaConverters.CreateZonedDateTimeConverter(provider);
    }

    internal void AddConverters(IList<JsonConverter> converters)
    {
        MaybeAdd(InstantConverter);
        MaybeAdd(IntervalConverter);
        MaybeAdd(LocalDateConverter);
        MaybeAdd(LocalDateTimeConverter);
        MaybeAdd(LocalTimeConverter);
        MaybeAdd(AnnualDateConverter);
        MaybeAdd(DateIntervalConverter);
        MaybeAdd(OffsetConverter);
        MaybeAdd(DateTimeZoneConverter);
        MaybeAdd(DurationConverter);
        MaybeAdd(PeriodConverter);
        MaybeAdd(OffsetDateTimeConverter);
        MaybeAdd(OffsetDateConverter);
        MaybeAdd(OffsetTimeConverter);
        MaybeAdd(ZonedDateTimeConverter);

        void MaybeAdd(JsonConverter converter)
        {
            if (converter is not null)
            {
                converters.Add(converter);
            }
        }
    }
}
