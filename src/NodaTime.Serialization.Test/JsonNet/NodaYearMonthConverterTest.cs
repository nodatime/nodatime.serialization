// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.JsonNet
{
    public class NodaYearMonthConverterTest
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = { NodaConverters.YearMonthConverter },
            DateParseHandling = DateParseHandling.None
        };

        [Test]
        public void Serialize_NonNullableType()
        {
            var yearMonth = new YearMonth(2010, 11);
            var json = JsonConvert.SerializeObject(yearMonth, Formatting.None, settings);
            string expectedJson = "\"2010-11\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            YearMonth? yearMonth = new YearMonth(2010, 11);
            var json = JsonConvert.SerializeObject(yearMonth, Formatting.None, settings);
            string expectedJson = "\"2010-11\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            YearMonth? yearMonth = null;
            var json = JsonConvert.SerializeObject(yearMonth, Formatting.None, settings);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"2010-11\"";
            var yearMonth = JsonConvert.DeserializeObject<YearMonth>(json, settings);
            var expectedYearMonth = new YearMonth(2010, 11);
            Assert.AreEqual(expectedYearMonth, yearMonth);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"2010-11\"";
            var yearMonth = JsonConvert.DeserializeObject<YearMonth?>(json, settings);
            YearMonth? expectedYearMonth = new YearMonth(2010, 11);
            Assert.AreEqual(expectedYearMonth, yearMonth);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var yearMonth = JsonConvert.DeserializeObject<YearMonth?>(json, settings);
            Assert.IsNull(yearMonth);
        }

        [Test]
        public void NodaJsonSettings_ShouldIncludeConverter()
        {
            var jsonSettings = new NodaJsonSettings();
            var configuredOptions = new JsonSerializerSettings().ConfigureForNodaTime(jsonSettings);
            Assert.AreEqual("\"2010-11\"", JsonConvert.SerializeObject(new YearMonth(2010, 11), configuredOptions));
        }
    }
}
