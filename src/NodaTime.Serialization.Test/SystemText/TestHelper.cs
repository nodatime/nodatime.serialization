// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Runtime.Serialization;
using System.Text.Json;
using NodaTime.Utility;
using NUnit.Framework;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NodaTime.Serialization.Test.SystemText
{
    internal static class TestHelper
    {
        internal static void AssertConversions<T>(T value, string expectedJson, JsonConverter converter)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { converter },
            };
            AssertConversions(value, expectedJson, options);
        }

        internal static void AssertConversions<T>(T value, string expectedJson, JsonSerializerOptions options)
        {
            var actualJson = JsonSerializer.Serialize(value, options);
            Assert.AreEqual(expectedJson, actualJson);

            var deserializedValue = JsonSerializer.Deserialize<T>(expectedJson, options);
            Assert.AreEqual(value, deserializedValue);
        }

        internal static void AssertInvalidJson<T>(string json, JsonSerializerOptions options)
        {
            var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<T>(json, options));
            Assert.IsInstanceOf<InvalidNodaDataException>(exception.InnerException);
        }
    }
}
