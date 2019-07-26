// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemText
{
    /// <summary>
    /// Base class for all the System.Text.Json converters which handle value types (which is most of them).
    /// This deals handles all the boilerplate code dealing with nullity.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public abstract class NodaConverterBase<T> : JsonConverter<T>
    {
        /// <summary>
        /// Converts the JSON stored in a reader into the relevant Noda Time type.
        /// </summary>
        /// <param name="reader">The json reader to read data from.</param>
        /// <param name="objectType">The type to convert the JSON to.</param>
        /// <param name="options">A serializer options to use for any embedded deserialization.</param>
        /// <exception cref="InvalidNodaDataException">The JSON was invalid for this converter.</exception>
        /// <returns>The deserialized value.</returns>
        public override T Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
        {
            try
            {
                // Delegate to the concrete subclass.
                // Note that we don't currently pass existingValue down; we could change this if we ever found a use for it.
                return ReadJsonImpl(ref reader, options);
            }
            catch (Exception ex)
            {
                throw new JsonException($"Cannot convert value to {objectType}", ex);
            }
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
        /// a value of type T.
        /// </summary>
        /// <param name="reader">The JSON reader to pull data from</param>
        /// <param name="options">The serializer options to use for nested serialization</param>
        /// <returns>The deserialized value of type T.</returns>
        protected abstract T ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options);

        /// <summary>
        /// Writes the given value to a Json.NET writer.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="options">The serializer options to use for any embedded serialization.</param>
        public override void Write(Utf8JsonWriter writer,
            T value, JsonSerializerOptions options)
        {
            WriteJsonImpl(writer, value, options);
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="options">The serializer options to use for nested serialization</param>
        protected abstract void WriteJsonImpl(Utf8JsonWriter writer,
            T value, JsonSerializerOptions options);
    }
}
