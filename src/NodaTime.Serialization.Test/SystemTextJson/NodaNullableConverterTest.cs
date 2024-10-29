// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using NUnit.Framework;
using static NodaTime.Serialization.Test.SystemText.TestHelper;

namespace NodaTime.Serialization.Test.SystemText
{
    /// <summary>
    /// Tests for the converters exposed in NodaConverters.
    /// </summary>
    public class NodaNullableConverterTest
    {
        [Test]
        public void InstantConverter_NotNull()
        {
            Instant? value = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            string json = "\"2012-01-02T03:04:05Z\"";
            var converter = new NodaTimeDefaultJsonConverterFactory().CreateConverter(typeof(Instant?), new JsonSerializerOptions());
            AssertConversions(value, json, converter);
        }

        [Test]
        public void InstantConverter_Null()
        {
            Instant? value = null;
            string json = "null";
            var converter = new NodaTimeDefaultJsonConverterFactory().CreateConverter(typeof(Instant?), new JsonSerializerOptions());
            AssertConversions(value, json, converter);
        }
    }
}
