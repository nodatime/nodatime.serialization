// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using static NodaTime.Serialization.Test.SystemText.TestHelper;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaAnnualDateConverterTest
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { NodaConverters.AnnualDateConverter },
        };

        private readonly JsonSerializerOptions optionsCamelCase = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { NodaConverters.AnnualDateConverter },
        };

        [Test]
        public void RoundTrip()
        {
            var month = 7;
            var day = 1;
            var annualDate = new AnnualDate(month, day);
            AssertConversions(annualDate, "{\"Month\":7,\"Day\":1}", options);
        }

        [Test]
        public void RoundTrip_CamelCase()
        {
            var month = 7;
            var day = 1;
            var annualDate = new AnnualDate(month, day);
            AssertConversions(annualDate, "{\"month\":7,\"day\":1}", optionsCamelCase);
        }

        [Test]
        public void Serialize_InObject()
        {
            var month = 7;
            var day = 1;
            var annualDate = new AnnualDate(month, day);

            var testObject = new TestObject { AnnualDate = annualDate };

            var json = JsonSerializer.Serialize(testObject, options);

            string expectedJson = "{\"AnnualDate\":{\"Month\":7,\"Day\":1}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_InObject_CamelCase()
        {
            var month = 7;
            var day = 1;
            var annualDate = new AnnualDate(month, day);

            var testObject = new TestObject { AnnualDate = annualDate };

            var json = JsonSerializer.Serialize(testObject, optionsCamelCase);

            string expectedJson = "{\"annualDate\":{\"month\":7,\"day\":1}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"AnnualDate\":{\"Month\":7,\"Day\":1}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, options);

            var annualDate = testObject.AnnualDate;

            var month = 7;
            var day = 1;
            var expectedAnnualDate = new AnnualDate(month, day);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        [Test]
        public void Deserialize_InObject_CamelCase()
        {
            string json = "{\"annualDate\":{\"month\":7,\"day\":1}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, optionsCamelCase);

            var annualDate = testObject.AnnualDate;

            var month = 7;
            var day = 1;
            var expectedAnnualDate = new AnnualDate(month, day);
            Assert.AreEqual(expectedAnnualDate, annualDate);
        }

        public class TestObject
        {
            public AnnualDate AnnualDate { get; set; }
        }
    }
}
