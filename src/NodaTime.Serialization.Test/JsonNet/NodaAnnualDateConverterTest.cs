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
    public class NodaAnnualDateConverterTest
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = { NodaConverters.AnnualDateConverter },
            DateParseHandling = DateParseHandling.None
        };

        [Test]
        public void Serialize_NonNullableType()
        {
            var annualDate = new AnnualDate(07, 01);
            var json = JsonConvert.SerializeObject(annualDate, Formatting.None, settings);
            string expectedJson = "\"07-01\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            AnnualDate? annualDate = new AnnualDate(07, 01);
            var json = JsonConvert.SerializeObject(annualDate, Formatting.None, settings);
            string expectedJson = "\"07-01\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            AnnualDate? instant = null;
            var json = JsonConvert.SerializeObject(instant, Formatting.None, settings);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"07-01\"";
            var annualDate = JsonConvert.DeserializeObject<AnnualDate>(json, settings);
            var expectedAnnualDate = new AnnualDate(07, 01);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"07-01\"";
            var annualDate = JsonConvert.DeserializeObject<AnnualDate?>(json, settings);
            AnnualDate? expectedAnnualDate = new AnnualDate(07, 01);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var annualDate = JsonConvert.DeserializeObject<AnnualDate?>(json, settings);
            Assert.IsNull(annualDate);
        }
    }
}
