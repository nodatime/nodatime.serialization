// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using System.Text.Json;
using static NodaTime.Serialization.Test.SystemText.TestHelper;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaDateIntervalConverterTest
    {
        private readonly JsonSerializerOptions options = new()
        {
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
        };

        private readonly JsonSerializerOptions optionsCaseInsensitive = new()
        {
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
            PropertyNameCaseInsensitive = true,
        };

        private readonly JsonSerializerOptions optionsCamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
        };

        private readonly JsonSerializerOptions optionsCamelCaseCaseInsensitive = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
            PropertyNameCaseInsensitive = true,
        };

        [Test]
        public void RoundTrip()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}", options);
        }

        [Test]
        public void RoundTrip_CamelCase()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}", optionsCamelCase);
        }

        [Test]
        public void Serialize_InObject()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.Serialize(testObject, options);

            string expectedJson = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_InObject_CamelCase()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.Serialize(testObject, optionsCamelCase);

            string expectedJson = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, options);

            var interval = testObject.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_InObject_CamelCase()
        {
            string json = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCase);

            var interval = testObject.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_CaseSensitive()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(json, options));
        }

        [Test]
        public void Deserialize_CaseSensitive_CamelCase()
        {
            string json = "{\"interval\":{\"Start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(json, optionsCamelCase));
        }

        [Test]
        public void Deserialize_CaseInsensitive()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";

            var testObjectPascalCase = JsonSerializer.Deserialize<TestObject>(json, optionsCaseInsensitive);;
            var testObjectCamelCase = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCaseCaseInsensitive);

            var intervalPascalCase = testObjectPascalCase.Interval;
            var intervalCamelCase = testObjectCamelCase.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.AreEqual(expectedInterval, intervalPascalCase);
            Assert.AreEqual(expectedInterval, intervalCamelCase);
        }

        [Test]
        public void Deserialize_CaseInsensitive_CamelCase()
        {
            string json = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            var testObjectPascalCase = JsonSerializer.Deserialize<TestObject>(json, optionsCaseInsensitive);
            var testObjectCamelCase = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCaseCaseInsensitive);

            var intervalPascalCase = testObjectPascalCase.Interval;
            var intervalCamelCase = testObjectCamelCase.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.AreEqual(expectedInterval, intervalPascalCase);
            Assert.AreEqual(expectedInterval, intervalCamelCase);
        }

        public class TestObject
        {
            public DateInterval Interval { get; set; }
        }
    }
}
