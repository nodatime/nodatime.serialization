// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using static NodaTime.Serialization.Test.JsonNet.TestHelper;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime.Serialization.JsonNet;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.JsonNet
{
    public class NodaDateIntervalConverterTest
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
            DateParseHandling = DateParseHandling.None
        };

        private readonly JsonSerializerSettings settingsCamelCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { NodaConverters.DateIntervalConverter, NodaConverters.LocalDateConverter },
            DateParseHandling = DateParseHandling.None
        };

        [Test]
        public void RoundTrip()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}", settings);
        }

        [Test]
        public void RoundTrip_CamelCase()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}", settingsCamelCase);
        }

        [Test]
        public void Serialize_InObject()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonConvert.SerializeObject(testObject, Formatting.None, settings);

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

            var json = JsonConvert.SerializeObject(testObject, Formatting.None, settingsCamelCase);

            string expectedJson = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";

            var testObject = JsonConvert.DeserializeObject<TestObject>(json, settings);

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

            var testObject = JsonConvert.DeserializeObject<TestObject>(json, settingsCamelCase);

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
