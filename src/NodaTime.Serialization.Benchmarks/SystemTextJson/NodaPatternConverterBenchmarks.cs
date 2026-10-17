// Copyright 2026 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Serialization.SystemTextJson;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.Benchmarks.SystemTextJson
{
    /// <summary>
    /// Benchmarks for writing values with the pattern-based System.Text.Json converters.
    /// Each benchmark writes <see cref="Count"/> values into a reused buffer.
    /// </summary>
    [MemoryDiagnoser]
    public class NodaPatternConverterBenchmarks
    {
        private const int Count = 1000;

        private static readonly Instant Start = Instant.FromUtc(2026, 10, 1, 0, 0);

        private readonly ArrayBufferWriter<byte> buffer = new ArrayBufferWriter<byte>(1 << 16);
        private readonly JsonSerializerOptions options = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        private readonly Instant[] wholeSecondInstants = Enumerable.Range(0, Count).Select(i => Start + Duration.FromMinutes(i)).ToArray();
        private readonly Instant[] fractionalInstants = Enumerable.Range(0, Count).Select(i => Start + Duration.FromTicks(i * 1_234_567L)).ToArray();
        private readonly Instant[] nanosecondInstants = Enumerable.Range(0, Count).Select(i => Start + Duration.FromNanoseconds(i * 123_456_789L)).ToArray();
        private readonly LocalDate[] localDates = Enumerable.Range(0, Count).Select(i => new LocalDate(2026, 1, 1).PlusDays(i)).ToArray();
        private readonly OffsetDateTime[] offsetDateTimes = Enumerable.Range(0, Count).Select(i => (Start + Duration.FromMinutes(i)).WithOffset(Offset.FromHours(2))).ToArray();
        private readonly ZonedDateTime[] zonedDateTimes = Enumerable.Range(0, Count).Select(i => (Start + Duration.FromMinutes(i)).InZone(DateTimeZoneProviders.Tzdb["Europe/Oslo"])).ToArray();
        private readonly Dictionary<Instant, int> instantKeyedDictionary = Enumerable.Range(0, Count).ToDictionary(i => Start + Duration.FromMinutes(i), i => i);

        private readonly JsonConverter<Instant> instantConverter = NodaConverters.InstantConverter;
        private readonly JsonConverter<LocalDate> localDateConverter = NodaConverters.LocalDateConverter;
        private readonly JsonConverter<OffsetDateTime> offsetDateTimeConverter = NodaConverters.OffsetDateTimeConverter;
        private readonly JsonConverter<ZonedDateTime> zonedDateTimeConverter = NodaConverters.CreateZonedDateTimeConverter(DateTimeZoneProviders.Tzdb);

        [Benchmark]
        public int Write_Instant_WholeSeconds() => WriteAll(instantConverter, wholeSecondInstants);

        [Benchmark]
        public int Write_Instant_FractionalTicks() => WriteAll(instantConverter, fractionalInstants);

        [Benchmark]
        public int Write_Instant_Nanoseconds() => WriteAll(instantConverter, nanosecondInstants);

        [Benchmark]
        public int Write_LocalDate() => WriteAll(localDateConverter, localDates);

        [Benchmark]
        public int Write_OffsetDateTime() => WriteAll(offsetDateTimeConverter, offsetDateTimes);

        [Benchmark]
        public int Write_ZonedDateTime() => WriteAll(zonedDateTimeConverter, zonedDateTimes);

        [Benchmark]
        public int Serialize_InstantArray()
        {
            buffer.Clear();
            using var writer = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(writer, wholeSecondInstants, options);
            return buffer.WrittenCount;
        }

        [Benchmark]
        public int Serialize_InstantKeyedDictionary()
        {
            buffer.Clear();
            using var writer = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(writer, instantKeyedDictionary, options);
            return buffer.WrittenCount;
        }

        private int WriteAll<T>(JsonConverter<T> converter, T[] values)
        {
            buffer.Clear();
            using var writer = new Utf8JsonWriter(buffer);
            writer.WriteStartArray();
            foreach (var value in values)
            {
                converter.Write(writer, value, options);
            }
            writer.WriteEndArray();
            writer.Flush();
            return buffer.WrittenCount;
        }
    }
}
