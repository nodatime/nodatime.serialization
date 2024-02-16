// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// Base class for all the System.Text.Json converters which handle value types (which is most of them).
    /// This deals handles all the boilerplate code dealing with nullity.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public abstract class NodaConverterBase<T> : JsonConverter<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected NodaConverterBase()
        {
        }

        #region CanConvert

        // This code section partially copied from NodaTime.Serialization.JsonNet.NodaConverterBase to support inheritance of types.
        // For now only DateTimeZone uses inheritance 

        // For value types and sealed classes, we can optimize and not call IsAssignableFrom.
        private static readonly bool CheckAssignableFrom =
            !(typeof(T).IsValueType || (typeof(T).IsClass && typeof(T).IsSealed));

        /// <summary>
        /// Returns whether or not this converter supports the given type.
        /// </summary>
        /// <param name="objectType">The type to check for compatibility.</param>
        /// <returns>True if the given type is supported by this converter (not including the nullable form for
        /// value types); false otherwise.</returns>
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(T) || (CheckAssignableFrom && typeof(T).IsAssignableFrom(objectType));

        #endregion

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
                return ReadJsonImpl(ref reader, options);
            }
            catch (Exception ex)
            {
                throw new JsonException($"Cannot convert value to {objectType}", ex);
            }
        }

        /// <summary>
        /// Converts the JSON stored in a reader into the relevant Noda Time type.
        /// </summary>
        /// <param name="reader">The json reader to read data from.</param>
        /// <param name="typeToConvert">The type to convert the JSON to.</param>
        /// <param name="options">A serializer options to use for any embedded deserialization.</param>
        /// <exception cref="InvalidNodaDataException">The JSON was invalid for this converter.</exception>
        /// <returns>The deserialized value.</returns>
        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            try
            {
                // Delegate to the concrete subclass.
                return ReadJsonImpl(ref reader, options);
            }
            catch (Exception ex)
            {
                throw new JsonException($"Cannot convert value to {typeToConvert}", ex);
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

        // Note: currently there's no concrete benefit in delegating to WriteJsonImpl rather than just
        // overriding Write directly, but we *could* put any common code (like the exception handling above)
        // in here in the future.

        /// <summary>
        /// Writes the given value to a Utf8JsonWriter.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="options">The serializer options to use for any embedded serialization.</param>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            WriteJsonImpl(writer, value, options);

        /// <summary>
        /// Writes the value as a string to a Utf8JsonWriter.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="options">The serializer options to use for any embedded serialization.</param>
        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            WriteJsonImpl(writer, value, options, true);

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="options">The serializer options to use for nested serialization</param>
        /// <param name="isProperty">Boolean flag to determine which writer method to invoke</param>
        protected abstract void WriteJsonImpl(Utf8JsonWriter writer, T value, JsonSerializerOptions options, bool isProperty = false);
    }
}
