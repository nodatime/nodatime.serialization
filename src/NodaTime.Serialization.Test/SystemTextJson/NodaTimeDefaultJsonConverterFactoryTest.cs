// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NodaTime.TimeZones;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.Test.SystemTextJson;

public partial class NodaTimeDefaultJsonConverterFactoryTest
{
    private static readonly List<Type> convertibleTypes = new()
    {
        typeof(AnnualDate),
        typeof(AnnualDate?),
        typeof(DateInterval),
        typeof(DateTimeZone),
        typeof(Duration),
        typeof(Duration?),
        typeof(Instant),
        typeof(Instant?),
        typeof(Interval),
        typeof(Interval?),
        typeof(LocalDate),
        typeof(LocalDate?),
        typeof(LocalTime),
        typeof(LocalTime?),
        typeof(Offset),
        typeof(Offset?),
        typeof(OffsetDate),
        typeof(OffsetDate?),
        typeof(OffsetDateTime),
        typeof(OffsetDateTime?),
        typeof(OffsetTime),
        typeof(OffsetTime?),
        typeof(Period),
        // Note: YearMonth isn't supported yet
        typeof(ZonedDateTime),
        typeof(ZonedDateTime?),
    };

    private static readonly List<Type> nonConvertibleTypes = new()
    {
        typeof(int),
        typeof(int?),
        typeof(PeriodBuilder),
        typeof(ZoneInterval)
    };

    [Test]
    [TestCaseSource(nameof(convertibleTypes))]
    public void ConvertibleTypes(Type type)
    {
        var factory = new NodaTimeDefaultJsonConverterFactory();
        Assert.IsTrue(factory.CanConvert(type));
        var converter = factory.CreateConverter(type, default);
        Assert.NotNull(converter);
        // The converter doesn't "advertise" that it handles nullable value types,
        // unlike the Newtonsoft.Json version.
        var typeToCheckForCanConvert = Nullable.GetUnderlyingType(type) ?? type;
        Assert.IsTrue(converter.CanConvert(typeToCheckForCanConvert));
    }

    [Test]
    [TestCaseSource(nameof(nonConvertibleTypes))]
    public void NonConvertibleTypes(Type type)
    {
        var factory = new NodaTimeDefaultJsonConverterFactory();
        Assert.IsFalse(factory.CanConvert(type));
        var converter = factory.CreateConverter(type, default);
        Assert.IsNull(converter);
    }

    // See https://github.com/nodatime/nodatime.serialization/issues/97 and
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
    [Test]
    public void SourceGenerationCompatibility()
    {
        var sample = new SampleData { Foo = Instant.FromUtc(2023, 8, 6, 12, 40, 12) };
        byte[] utf8Json = JsonSerializer.SerializeToUtf8Bytes(sample, SampleJsonContext.Default.SampleData);
        string actual = Encoding.UTF8.GetString(utf8Json);
        string expected = "{\"Foo\":\"2023-08-06T12:40:12Z\"}";
        Assert.AreEqual(expected, actual);
    }

    public class SampleData
    {
        [JsonConverter(typeof(NodaTimeDefaultJsonConverterFactory))]
        public Instant Foo { get; set; }
    }

    [JsonSerializable(typeof(SampleData))]
    public partial class SampleJsonContext : JsonSerializerContext
    {
    }
}
