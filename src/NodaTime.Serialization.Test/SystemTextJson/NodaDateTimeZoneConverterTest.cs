// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Serialization.SystemTextJson;
using NodaTime.TimeZones;
using NodaTime.Utility;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaDateTimeZoneConverterTest
    {
        private readonly JsonConverter<DateTimeZone> converter =
            NodaConverters.CreateDateTimeZoneConverter(DateTimeZoneProviders.Tzdb);

        [Test]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = JsonSerializer.Serialize(dateTimeZone, new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = {converter}
            });
            string expectedJson = "\"America/Los_Angeles\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void SerializeAsObject()
        {
            object dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = JsonSerializer.Serialize(dateTimeZone, new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            });
            string expectedJson = "\"America/Los_Angeles\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"America/Los_Angeles\"";
            var dateTimeZone = JsonSerializer.Deserialize<DateTimeZone>(json, new JsonSerializerOptions
            {
                Converters = {converter}
            });
            var expectedDateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            Assert.AreEqual(expectedDateTimeZone, dateTimeZone);
        }

        [Test]
        public void Deserialize_TimeZoneNotFound()
        {
            string json = "\"America/DOES_NOT_EXIST\"";
            var exception =
                Assert.Throws<JsonException>(() =>
                    JsonSerializer.Deserialize<DateTimeZone>(json, new JsonSerializerOptions
                    {
                        Converters = {converter}
                    }));
            Assert.IsInstanceOf<DateTimeZoneNotFoundException>(exception.InnerException);
        }

        [Test]
        public void Deserialize_InvalidData()
        {
            string json = "0";
            var exception =
                Assert.Throws<JsonException>(() =>
                    JsonSerializer.Deserialize<DateTimeZone>(json, new JsonSerializerOptions
                    {
                        Converters = {converter}
                    }));
            Assert.AreEqual("The JSON value could not be converted to NodaTime.DateTimeZone. Path: $ | LineNumber: 0 | BytePositionInLine: 1.", exception.Message);
            Assert.IsInstanceOf<InvalidNodaDataException>(exception.InnerException);
            Assert.AreEqual("Unexpected token parsing time zone. Expected String, got Number.", exception.InnerException.Message);
        }
    }
}