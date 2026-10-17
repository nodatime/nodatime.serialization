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

        [Test]
        public void Serialize_VaryingLengths_ReusesBufferCorrectly()
        {
            var longer = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromNanoseconds(123_456_789);
            var shorter = Instant.FromUtc(2012, 1, 2, 3, 4, 5);

            var arrayJson = JsonSerializer.Serialize(new[] { longer, shorter }, options);
            Assert.AreEqual("[\"2012-01-02T03:04:05.123456789Z\",\"2012-01-02T03:04:05Z\"]", arrayJson);

            var dictionaryJson = JsonSerializer.Serialize(new Dictionary<Instant, int> { [longer] = 1, [shorter] = 2 }, options);
            Assert.AreEqual("{\"2012-01-02T03:04:05.123456789Z\":1,\"2012-01-02T03:04:05Z\":2}", dictionaryJson);
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
    }
}
