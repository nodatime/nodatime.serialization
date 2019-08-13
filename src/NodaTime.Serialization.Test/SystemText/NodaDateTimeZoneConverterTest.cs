// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Serialization.SystemText;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaDateTimeZoneConverterTest
    {
        private readonly JsonConverter converter = NodaConverters.CreateDateTimeZoneConverter(DateTimeZoneProviders.Tzdb);

        [Test]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = JsonConvert.SerializeObject(dateTimeZone, Formatting.None, converter);
            string expectedJson = "\"America/Los_Angeles\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"America/Los_Angeles\"";
            var dateTimeZone = JsonConvert.DeserializeObject<DateTimeZone>(json, converter);
            var expectedDateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            Assert.AreEqual(expectedDateTimeZone, dateTimeZone);
        }

        [Test]
        public void Deserialize_TimeZoneNotFound()
        {
            string json = "\"America/DOES_NOT_EXIST\"";
            var exception = Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<DateTimeZone>(json, converter));
            Assert.IsInstanceOf<DateTimeZoneNotFoundException>(exception.InnerException);
        }
    }
}
