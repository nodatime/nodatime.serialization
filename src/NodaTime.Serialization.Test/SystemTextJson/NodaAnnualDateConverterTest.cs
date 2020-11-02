// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using System.Text.Json;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaAnnualDateConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.AnnualDateConverter },
        };

        [Test]
        public void Serialize_NonNullableType()
        {
            var annualDate = new AnnualDate(07, 01);
            var json = JsonSerializer.Serialize(annualDate, options);
            string expectedJson = "\"07-01\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            AnnualDate? annualDate = new AnnualDate(07, 01);
            var json = JsonSerializer.Serialize(annualDate, options);
            string expectedJson = "\"07-01\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            AnnualDate? annualDate = null;
            var json = JsonSerializer.Serialize(annualDate, options);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"07-01\"";
            var annualDate = JsonSerializer.Deserialize<AnnualDate>(json, options);
            var expectedAnnualDate = new AnnualDate(07, 01);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"07-01\"";
            var annualDate = JsonSerializer.Deserialize<AnnualDate?>(json, options);
            AnnualDate? expectedAnnualDate = new AnnualDate(07, 01);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var annualDate = JsonSerializer.Deserialize<AnnualDate?>(json, options);
            Assert.IsNull(annualDate);
        }
    }
}
