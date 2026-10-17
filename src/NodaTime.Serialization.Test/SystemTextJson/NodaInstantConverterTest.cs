// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaInstantConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.InstantConverter },
        };

        [Test]
        public void Serialize_NonNullableType()
        {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonSerializer.Serialize(instant, options);
            string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            Instant? instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonSerializer.Serialize(instant, options);
            string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            Instant? instant = null;
            var json = JsonSerializer.Serialize(instant, options);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonSerializer.Deserialize<Instant>(json, options);
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonSerializer.Deserialize<Instant?>(json, options);
            Instant? expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var instant = JsonSerializer.Deserialize<Instant?>(json, options);
            Assert.IsNull(instant);
        }

        [Test]
        public void Serialize_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);
            var jsonDateTime = JsonSerializer.Serialize(dateTime, options);
            var jsonInstant = JsonSerializer.Serialize(instant, options);
            Assert.AreEqual(jsonDateTime, jsonInstant);
        }

        private static readonly Instant[] ExtendedIsoEdgeCases =
        {
            Instant.FromUtc(2012, 1, 2, 3, 4, 5),
            Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(500),
            Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromTicks(1_234_500),
            Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromTicks(1),
            Instant.FromUnixTimeTicks(0),
            Instant.FromUnixTimeTicks(-1),
            Instant.FromUtc(1, 1, 1, 0, 0),
            Instant.FromUtc(9999, 12, 31, 23, 59, 59) + Duration.FromTicks(9_999_999),
            Instant.FromUnixTimeTicks(0) + Duration.FromNanoseconds(1),
            Instant.FromUnixTimeTicks(-1) + Duration.FromNanoseconds(50),
            Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromNanoseconds(123_456_789),
            Instant.FromUtc(9999, 12, 31, 23, 59, 59) + Duration.FromNanoseconds(999_999_999),
            Instant.FromUtc(0, 12, 31, 23, 59, 59),
            Instant.FromUtc(-5, 1, 1, 0, 0),
            Instant.MinValue,
            Instant.MaxValue,
        };

        [Test]
        [TestCaseSource(nameof(ExtendedIsoEdgeCases))]
        public void Serialize_MatchesExtendedIsoPattern(Instant instant)
        {
            AssertMatchesExtendedIsoPattern(instant);
        }

        [Test]
        public void Serialize_MatchesExtendedIsoPattern_RandomValues()
        {
            var random = new Random(42);
            var start = Instant.FromUtc(1, 1, 1, 0, 0);
            long seconds = (long) (Instant.FromUtc(9999, 12, 31, 23, 59, 59) - start).TotalSeconds;
            for (int i = 0; i < 10_000; i++)
            {
                var whole = start + Duration.FromSeconds((long) (random.NextDouble() * seconds));
                var fraction = (i % 4) switch
                {
                    0 => Duration.Zero,
                    1 => Duration.FromMilliseconds(random.Next(1_000)),
                    2 => Duration.FromTicks(random.Next(10_000_000)),
                    _ => Duration.FromNanoseconds(random.Next(1_000_000_000)),
                };
                AssertMatchesExtendedIsoPattern(whole + fraction);
            }
        }

        [Test]
        [TestCaseSource(nameof(ExtendedIsoEdgeCases))]
        public void Serialize_AsPropertyName_MatchesExtendedIsoPattern(Instant instant)
        {
            var json = JsonSerializer.Serialize(new Dictionary<Instant, int> { [instant] = 1 }, options);
            Assert.AreEqual($"{{\"{InstantPattern.ExtendedIso.Format(instant)}\":1}}", json);
        }

        [Test]
        public void Serialize_CustomPattern_UsesPattern()
        {
            var customOptions = new JsonSerializerOptions
            {
                Converters = { new NodaPatternConverter<Instant>(InstantPattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'")) },
            };
            var json = JsonSerializer.Serialize(Instant.FromUtc(2012, 1, 2, 3, 4, 5), customOptions);
            Assert.AreEqual("\"2012-01-02T03:04:05.000Z\"", json);
        }

        [Test]
        public void Serialize_ValueLongerThanCachedBuffer()
        {
            var longLiteral = new string('x', 1_000);
            var customOptions = new JsonSerializerOptions
            {
                Converters = { new NodaPatternConverter<Instant>(InstantPattern.CreateWithInvariantCulture($"'{longLiteral}'uuuu")) },
            };
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual($"\"{longLiteral}2012\"", JsonSerializer.Serialize(instant, customOptions));
                Assert.AreEqual("\"2012-01-02T03:04:05Z\"", JsonSerializer.Serialize(instant, options));
            }
        }

        private void AssertMatchesExtendedIsoPattern(Instant instant)
        {
            var expected = $"\"{InstantPattern.ExtendedIso.Format(instant)}\"";
            Assert.AreEqual(expected, JsonSerializer.Serialize(instant, options));
            Assert.AreEqual(expected, JsonSerializer.Serialize<Instant?>(instant, options));
        }
    }
}
