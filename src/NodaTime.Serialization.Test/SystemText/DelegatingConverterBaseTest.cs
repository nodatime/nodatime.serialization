// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Serialization.SystemText;
using NodaTime.Text;
using NUnit.Framework;
using NodaConverters = NodaTime.Serialization.SystemText.NodaConverters;

namespace NodaTime.Serialization.Test.SystemText
{
    public class DelegatingConverterBaseTest
    {
        [Test]
        public void Serialize()
        {
            string expected = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var date = new LocalDate(2017, 2, 20);
            var entity = new Entity { ShortDate = date, LongDate = date };
            var actual = JsonSerializer.Serialize(entity, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Deserialize()
        {
            string json = "{'ShortDate':'2017-02-20','LongDate':'20 February 2017'}"
                .Replace("'", "\"");
            var expectedDate = new LocalDate(2017, 2, 20);
            var entity = JsonSerializer.Deserialize<Entity>(json);
            Assert.AreEqual(expectedDate, entity.ShortDate);
            Assert.AreEqual(expectedDate, entity.LongDate);
        }

        public class Entity
        {
            [JsonConverter(typeof(ShortDateConverter))]
            public LocalDate ShortDate { get; set; }

            [JsonConverter(typeof(LongDateConverter))]
            public LocalDate LongDate { get; set; }
        }

        public class ShortDateConverter : DelegatingConverterBase<LocalDate>
        {
            public ShortDateConverter() : base(NodaConverters.LocalDateConverter) { }
        }

        public class LongDateConverter : DelegatingConverterBase<LocalDate>
        {
            // No need to create a new one of these each time...
            private static readonly JsonConverter<LocalDate> converter =
                new Serialization.SystemText.NodaPatternConverter<LocalDate>(LocalDatePattern.CreateWithInvariantCulture("d MMMM yyyy"));

            public LongDateConverter() : base(converter)
            {
            }
        }
    }
}
