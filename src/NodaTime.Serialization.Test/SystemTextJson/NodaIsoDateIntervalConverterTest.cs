// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using static NodaTime.Serialization.Test.SystemText.TestHelper;

namespace NodaTime.Serialization.Test.SystemText
{
    /// <summary>
    /// The same tests as NodaDateIntervalConverterTest, but using the ISO-based interval converter.
    /// </summary>
    public class NodaIsoDateIntervalConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.IsoDateIntervalConverter, NodaConverters.LocalDateConverter },
        };

        [Test]
        public void RoundTrip()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "\"2012-01-02/2013-06-07\"", options);
        }

        [Test]
        [TestCase("\"2012-01-022013-06-07\"")]
        public void InvalidJson(string json)
        {
            AssertInvalidJson<DateInterval>(json, options);
        }

        [Test]
        public void Serialize_InObject()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.Serialize(testObject, options);

            string expectedJson = "{\"Interval\":\"2012-01-02/2013-06-07\"}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"Interval\":\"2012-01-02/2013-06-07\"}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, options);

            var interval = testObject.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.AreEqual(expectedInterval, interval);
        }

        public class TestObject
        {
            public DateInterval Interval { get; set; }
        }
    }
}
