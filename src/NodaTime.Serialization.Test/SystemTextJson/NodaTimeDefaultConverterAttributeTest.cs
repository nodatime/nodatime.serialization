// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using System.Text.Json;

namespace NodaTime.Serialization.Test.SystemTextJson;

public class NodaTimeDefaultConverterAttributeTest
{
    [Test]
    public void Roundtrip()
    {
        var obj = new SampleClass
        {
            NonNullableInstant = Instant.FromUtc(2023, 5, 29, 14, 11, 23),
            NullableInstant1 = Instant.FromUtc(2025, 1, 2, 3, 4, 5),
            NullableInstant2 = null,
            ZonedDateTime = Instant.FromUtc(2023, 5, 29, 14, 11, 23).InZone(DateTimeZoneProviders.Tzdb["Europe/London"])
        };

        string json = JsonSerializer.Serialize(obj);
        var result = JsonSerializer.Deserialize<SampleClass>(json);

        Assert.AreEqual(obj.NonNullableInstant, result.NonNullableInstant);
        Assert.AreEqual(obj.NullableInstant1, result.NullableInstant1);
        Assert.Null(result.NullableInstant2);
        Assert.AreEqual(obj.ZonedDateTime, result.ZonedDateTime);
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
    }
}
