// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaYearMonthConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.YearMonthConverter },
        };

        [Test]
        public void Serialize_NonNullableType()
        {
            YearMonth? yearMonth = new YearMonth(2010, 11);
            var json = JsonSerializer.Serialize(yearMonth, options);
            string expectedJson = "\"2010-11\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            YearMonth? yearMonth = new YearMonth(2010, 11);
            var json = JsonSerializer.Serialize(yearMonth, options);
            string expectedJson = "\"2010-11\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            YearMonth? yearMonth = null;
            var json = JsonSerializer.Serialize(yearMonth, options);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"2010-11\"";
            var yearMonth = JsonSerializer.Deserialize<YearMonth>(json, options);
            var expectedYearMonth = new YearMonth(2010, 11);
            Assert.AreEqual(expectedYearMonth, yearMonth);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"2010-11\"";
            var yearMonth = JsonSerializer.Deserialize<YearMonth?>(json, options);
            YearMonth? expectedYearMonth = new YearMonth(2010, 11);
            Assert.AreEqual(expectedYearMonth, yearMonth);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var yearMonth = JsonSerializer.Deserialize<YearMonth?>(json, options);
            Assert.IsNull(yearMonth);
        }

        [Test]
        public void NodaJsonSettings_ShouldAddConverter()
        {
            var jsonSettings = new NodaJsonSettings();
            var configuredOptions = new JsonSerializerOptions().ConfigureForNodaTime(jsonSettings);
            Assert.AreEqual("\"2010-11\"", JsonSerializer.Serialize(new YearMonth(2010, 11), configuredOptions));
        }
    }
}
