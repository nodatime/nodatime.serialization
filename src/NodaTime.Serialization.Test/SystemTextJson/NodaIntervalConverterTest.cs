// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using static NodaTime.Serialization.Test.SystemText.TestHelper;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaIntervalConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter },
        };

        private readonly JsonSerializerOptions optionsCaseInsensitive = new JsonSerializerOptions
        {
            Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter },
            PropertyNameCaseInsensitive = true,
        };

        private readonly JsonSerializerOptions optionsCamelCase = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter },
        };

        private readonly JsonSerializerOptions optionsCamelCaseCaseInsensitive = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter },
            PropertyNameCaseInsensitive = true,
        };

        [Test]
        public void RoundTrip()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);
            var interval = new Interval(startInstant, endInstant);
            AssertConversions(interval, "{\"Start\":\"2012-01-02T03:04:05.67Z\",\"End\":\"2013-06-07T08:09:10.123456789Z\"}", options);
        }

        [Test]
        public void RoundTrip_Infinite()
        {
            var instant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);
            AssertConversions(new Interval(null, instant), "{\"End\":\"2013-06-07T08:09:10.123456789Z\"}", options);
            AssertConversions(new Interval(instant, null), "{\"Start\":\"2013-06-07T08:09:10.123456789Z\"}", options);
            AssertConversions(new Interval(null, null), "{}", options);
        }

        [Test]
        public void Serialize_InObject()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            var json = JsonSerializer.Serialize(testObject, options);

            string expectedJson = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_InObject_CamelCase()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            var json = JsonSerializer.Serialize(testObject, optionsCamelCase);

            string expectedJson = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, options);

            var interval = testObject.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_InObject_CamelCase()
        {
            string json = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCase);

            var interval = testObject.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_CaseSensitive()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, options);

            Assert.True(testObject.Interval.HasStart);
            Assert.False(testObject.Interval.HasEnd);
        }

        [Test]
        public void Deserialize_CaseSensitive_CamelCase()
        {
            string json = "{\"interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCase);

            Assert.False(testObject.Interval.HasStart);
            Assert.True(testObject.Interval.HasEnd);
        }

        [Test]
        public void Deserialize_CaseInsensitive()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";

            var testObjectPascalCase = JsonSerializer.Deserialize<TestObject>(json, optionsCaseInsensitive);
            var testObjectCamelCase = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCaseCaseInsensitive);

            var intervalPascalCase = testObjectPascalCase.Interval;
            var intervalCamelCase = testObjectCamelCase.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, intervalPascalCase);
            Assert.AreEqual(expectedInterval, intervalCamelCase);
        }

        [Test]
        public void Deserialize_CaseInsensitive_CamelCase()
        {
            string json = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObjectPascalCase = JsonSerializer.Deserialize<TestObject>(json, optionsCaseInsensitive);
            var testObjectCamelCase = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCaseCaseInsensitive);

            var intervalPascalCase = testObjectPascalCase.Interval;
            var intervalCamelCase = testObjectCamelCase.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, intervalPascalCase);
            Assert.AreEqual(expectedInterval, intervalCamelCase);
        }

        [Test]
        public void CannotUseIntervalAsPropertyName()
        {
            var obj = new Dictionary<Interval, string>
            {
                {  new Interval(NodaConstants.UnixEpoch, NodaConstants.UnixEpoch), "Test" }
            };
            Assert.Throws<JsonException>(() => JsonSerializer.Serialize(obj, options));
        }

        public class TestObject
        {
            public Interval Interval { get; set; }
        }
    }
}
