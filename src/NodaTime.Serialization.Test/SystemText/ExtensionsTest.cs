// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.IO;
using System.Text.Json;
using NodaTime.Serialization.SystemText;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.SystemText
{
    public class ExtensionsTest
    {
        [Test]
        public void Options_ConfigureForNodaTime_DefaultInterval()
        {
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitOptions = new JsonSerializerOptions
            {
                Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter }
            };
            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));
            Assert.AreEqual(JsonSerializer.Serialize(interval, explicitOptions),
                JsonSerializer.Serialize(interval, configuredOptions));
        }

        [Test]
        public void Options_ConfigureForNodaTime_WithIsoIntervalConverter()
        {
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoIntervalConverter();
            var explicitOptions = new JsonSerializerOptions { Converters = { NodaConverters.IsoIntervalConverter } };
            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));
            Assert.AreEqual(JsonSerializer.Serialize(interval, explicitOptions),
                JsonSerializer.Serialize(interval, configuredOptions));
        }

        [Test]
        public void Options_ConfigureForNodaTime_DefaultDateInterval()
        {
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitOptions = new JsonSerializerOptions
            {
                Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter }
            };
            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));
            Assert.AreEqual(JsonSerializer.Serialize(interval, explicitOptions),
                JsonSerializer.Serialize(interval, configuredOptions));
        }

        [Test]
        public void Options_ConfigureForNodaTime_WithIsoDateIntervalConverter()
        {
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoDateIntervalConverter();
            var explicitOptions = new JsonSerializerOptions { Converters = { NodaConverters.IsoDateIntervalConverter } };
            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));
            Assert.AreEqual(JsonSerializer.Serialize(interval, explicitOptions),
                JsonSerializer.Serialize(interval, configuredOptions));
        }
    }
}
