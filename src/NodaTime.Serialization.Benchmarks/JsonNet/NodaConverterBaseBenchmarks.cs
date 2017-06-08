// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using System.IO;

namespace NodaTime.Serialization.Benchmarks.JsonNet
{
    public class NodaConverterBaseBenchmarks
    {
        private readonly NodaConverterBase<int> int32Converter = new DummyConverter<int>();
        private readonly NodaConverterBase<string> stringConverter = new DummyConverter<string>();
        private readonly NodaConverterBase<Stream> streamConverter = new DummyConverter<Stream>();

        // Value types
        [Benchmark]
        public bool CanConvert_Int32_Int32() => int32Converter.CanConvert(typeof(int));

        [Benchmark]
        public bool CanConvert_Int32_NullableInt32() => int32Converter.CanConvert(typeof(int?));

        [Benchmark]
        public bool CanConvert_Int32_Object() => int32Converter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_Int32_String() => int32Converter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_Int32_UInt32() => int32Converter.CanConvert(typeof(uint));

        // Sealed classes
        [Benchmark]
        public bool CanConvert_String_String() => stringConverter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_String_Object() => stringConverter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_String_UInt32() => stringConverter.CanConvert(typeof(uint));

        // Unsealed classes
        [Benchmark]
        public bool CanConvert_Stream_Stream() => streamConverter.CanConvert(typeof(Stream));

        [Benchmark]
        public bool CanConvert_Stream_MemoryStream() => streamConverter.CanConvert(typeof(MemoryStream));

        [Benchmark]
        public bool CanConvert_Stream_Object() => streamConverter.CanConvert(typeof(object));

        [Benchmark]
        public bool CanConvert_Stream_String() => streamConverter.CanConvert(typeof(string));

        [Benchmark]
        public bool CanConvert_Stream_UInt32() => streamConverter.CanConvert(typeof(uint));

        private class DummyConverter<T> : NodaConverterBase<T>
        {
            protected override T ReadJsonImpl(JsonReader reader, JsonSerializer serializer) => default(T);

            protected override void WriteJsonImpl(JsonWriter writer, T value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
