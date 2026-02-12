// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaConverterBaseTest
    {
        [Test]
        public void Serialize_NonNullValue()
        {
            var options = CreateOptions<TestConverter>();
            JsonSerializer.Serialize(5, options);
        }

        [Test]
        public void Serialize_NullValue()
        {
            var options = CreateOptions<TestConverter>();
            JsonSerializer.Serialize((object)null, options);
        }

        [Test]
        public void Deserialize_NullableType_NullValue()
        {
            var options = CreateOptions<TestConverter>();
            Assert.IsNull(JsonSerializer.Deserialize<int?>("null", options));
        }

        [Test]
        public void Deserialize_ReferenceType_NullValue()
        {
            var options = CreateOptions<TestStringConverter>();
            Assert.IsNull(JsonSerializer.Deserialize<string>("null", options));
        }

        [Test]
        public void Deserialize_NullableType_NonNullValue()
        {
            var options = CreateOptions<TestConverter>();
            Assert.AreEqual(5, JsonSerializer.Deserialize<int?>("\"5\"", options));
        }

        [Test]
        [TestCase("null")]
        [TestCase("\"\"")]
        public void Deserialize_NonNullableType_InvalidValue(string json)
        {
            var options = CreateOptions<TestConverter>();
            var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(json, options));
            StringAssert.StartsWith("The JSON value could not be converted to System.Int32. Path: $ | LineNumber: 0 | BytePositionInLine: ", exception.Message);
        }

        [Test]
        public void Deserialize_NonNullableType_NonNullValue()
        {
            var options = CreateOptions<TestConverter>();
            Assert.AreEqual(5, JsonSerializer.Deserialize<int>("\"5\"", options));
        }

        private static JsonSerializerOptions CreateOptions<TConverter>() where TConverter : JsonConverter, new() =>
            new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { new TConverter() }
            };

        private class TestConverter : NodaConverterBase<int>
        {
            protected override int ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                return int.Parse(reader.GetString());
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString());

            protected override void WriteJsonPropertyNameImpl(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
                writer.WritePropertyName(value.ToString());
        }

        private class TestStringConverter : NodaConverterBase<string>
        {
            protected override string ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                return reader.GetString();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value);

            protected override void WriteJsonPropertyNameImpl(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
                writer.WritePropertyName(value);
        }
    }
}
