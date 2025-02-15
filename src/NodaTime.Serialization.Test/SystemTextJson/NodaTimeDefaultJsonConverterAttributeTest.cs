// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using NUnit.Framework;
using System.Text.Json;

namespace NodaTime.Serialization.Test.SystemTextJson;

public class NodaTimeDefaultJsonConverterAttributeTest
{
    [Test]
    public void Roundtrip()
    {
        var options = new JsonSerializerOptions
        {
            // Converter that only serializes to the minute. Won't round-trip our sample value, and
            // the result can't be parsed with the default converter.
            // The fact that we *do* roundtrip shows that the custom options are ignored in favor of the
            // attribute saying "use the default".
            Converters = { new NodaPatternConverter<Instant>(InstantPattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm'Z'")) },
        };
        AssertRoundtrip(null, null);
        AssertRoundtrip(null, options);
        AssertRoundtrip(options, null);
        AssertRoundtrip(options, options);
    }

    private void AssertRoundtrip(JsonSerializerOptions serializationOptions, JsonSerializerOptions deserializationOptions)
    {
        var options = new JsonSerializerOptions
        {
            // Converter that only serializes to the minute. Won't round-trip our sample value, and
            // the result can't be parsed with the default converter.
            // The fact that we *do* roundtrip shows that the custom options are ignored in favor of the
            // attribute saying "use the default".
            Converters = { new NodaPatternConverter<Instant>(InstantPattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm'Z'")) },
        };

        var obj = new SampleClass
        {
            NonNullableInstant = Instant.FromUtc(2023, 5, 29, 14, 11, 23),
            NullableInstant1 = Instant.FromUtc(2025, 1, 2, 3, 4, 5),
            NullableInstant2 = null,
            ZonedDateTime = Instant.FromUtc(2023, 5, 29, 14, 11, 23).InZone(DateTimeZoneProviders.Tzdb["Europe/London"]),
            Interval = new(Instant.FromUtc(2023, 5, 29, 14, 11, 23), Instant.FromUtc(2025, 1, 2, 3, 4, 5)),
            DateInterval = new(new LocalDate(2025, 2, 13), new LocalDate(2025, 2, 15))
        };

        string json = JsonSerializer.Serialize(obj, serializationOptions);
        var result = JsonSerializer.Deserialize<SampleClass>(json, deserializationOptions);

        Assert.AreEqual(obj.NonNullableInstant, result.NonNullableInstant);
        Assert.AreEqual(obj.NullableInstant1, result.NullableInstant1);
        Assert.Null(result.NullableInstant2);
        Assert.AreEqual(obj.ZonedDateTime, result.ZonedDateTime);
        Assert.AreEqual(obj.Interval, result.Interval);
        Assert.AreEqual(obj.DateInterval, result.DateInterval);
    }

    // Just enough properties to check that the custom converters are being used.
    public class SampleClass
    {
        [NodaTimeDefaultJsonConverter]
        public Instant NonNullableInstant { get; set; }
        [NodaTimeDefaultJsonConverter]
        public Instant? NullableInstant1 { get; set; }
        [NodaTimeDefaultJsonConverter]
        public Instant? NullableInstant2 { get; set; }
        [NodaTimeDefaultJsonConverter]
        public ZonedDateTime ZonedDateTime { get; set; }
        [NodaTimeDefaultJsonConverter]
        public Interval Interval { get; set; }
        [NodaTimeDefaultJsonConverter]
        public DateInterval DateInterval { get; set; }
    }
}
