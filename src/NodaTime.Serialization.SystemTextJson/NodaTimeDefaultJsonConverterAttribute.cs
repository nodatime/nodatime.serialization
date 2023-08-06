// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
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
    /// <summary>
    /// Constructs an instance of the attribute.
    /// </summary>
    public NodaTimeDefaultJsonConverterAttribute()
    {
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert) =>
        NodaTimeDefaultJsonConverterFactory.GetConverter(typeToConvert);
}