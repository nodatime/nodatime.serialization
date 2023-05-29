// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using NUnit.Framework;
using System.Linq;
using System.Text.Json;

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

        [Test]
        public void Options_ConfigureForNodaTime_NodaJsonSettings()
        {
            var jsonSettings = new NodaJsonSettings
            {
                LocalDateConverter = new NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("dd/MM/yyyy")),
                LocalTimeConverter = null
            };
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(jsonSettings);
            Assert.AreEqual("\"28/05/2023\"", JsonSerializer.Serialize(new LocalDate(2023, 5, 28), configuredOptions));
            Assert.AreEqual("\"2023-05-28T18:07:12Z UTC\"", JsonSerializer.Serialize(new LocalDateTime(2023, 5, 28, 18, 07, 12).InUtc(), configuredOptions));
            Assert.False(configuredOptions.Converters.Any(c => c.CanConvert(typeof(LocalTime))));
        }
    }
}
