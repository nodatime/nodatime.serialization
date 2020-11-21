// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Reflection;
using Newtonsoft.Json;
using NodaTime.Utility;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Base class for all the Json.NET converters which handle value types (which is most of them).
    /// This handles all the boilerplate code dealing with nullity.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public abstract class NodaConverterBase<T> : JsonConverter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected NodaConverterBase()
        {
        }

        // For value types and sealed classes, we can optimize and not call IsAssignableFrom.
        private static readonly bool CheckAssignableFrom =
            !(typeof(T).IsValueType || (typeof(T).IsClass && typeof(T).IsSealed));

        private static readonly Type NullableT = typeof(T).IsValueType 
            ? typeof(Nullable<>).MakeGenericType(typeof(T)) : typeof(T);

        // TODO: It's not clear whether we *should* support inheritance here. The Json.NET docs
        // aren't clear on when this is used - is it for reading or writing? If it's for both, that's
        // a problem: our "writer" may be okay for subclasses, but that doesn't mean the "reader" is.
        // This may well only be an issue for DateTimeZone, as everything else uses a sealed type (e.g. Period)
        // or a value type.

        /// <summary>
        /// Returns whether or not this converter supports the given type.
        /// </summary>
        /// <param name="objectType">The type to check for compatibility.</param>
        /// <returns>True if the given type is supported by this converter (including the nullable form for
        /// value types); false otherwise.</returns>
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(T) || objectType == NullableT ||
            (CheckAssignableFrom && typeof(T).IsAssignableFrom(objectType.GetTypeInfo()));

        /// <summary>
        /// Converts the JSON stored in a reader into the relevant Noda Time type.
        /// </summary>
        /// <param name="reader">The Json.NET reader to read data from.</param>
        /// <param name="objectType">The type to convert the JSON to.</param>
        /// <param name="existingValue">An existing value; ignored by this converter.</param>
        /// <param name="serializer">A serializer to use for any embedded deserialization.</param>
        /// <exception cref="InvalidNodaDataException">The JSON was invalid for this converter.</exception>
        /// <returns>The deserialized value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                Preconditions.CheckData(objectType == NullableT,
                    "Cannot convert null value to {0}",
                    objectType);
                return null;
            }

            // Handle empty strings automatically
            if (reader.TokenType == JsonToken.String)
            {
                string value = (string) reader.Value;
                if (value == "")
                {
                    Preconditions.CheckData(objectType == NullableT,
                        "Cannot convert null value to {0}",
                        objectType);
                    return null;
                }
            }

            try
            {
                // Delegate to the concrete subclass. At this point we know that we don't want to return null, so we
                // can ask the subclass to return a T, which we will box. That will be valid even if objectType is
                // T? because the boxed form of a non-null T? value is just the boxed value itself.

                // Note that we don't currently pass existingValue down; we could change this if we ever found a use for it.
                return ReadJsonImpl(reader, serializer);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Cannot convert value to {objectType}", ex);
            }
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
        /// a value of type T.
        /// </summary>
        /// <param name="reader">The JSON reader to pull data from</param>
        /// <param name="serializer">The serializer to use for nested serialization</param>
        /// <returns>The deserialized value of type T.</returns>
        protected abstract T ReadJsonImpl(JsonReader reader, JsonSerializer serializer);

        /// <summary>
        /// Writes the given value to a Json.NET writer.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The serializer to use for any embedded serialization.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Json.NET should prevent this happening, but let's validate...
            Preconditions.CheckNotNull(value, nameof(value));

            // Note: don't need to worry about value is T? due to the way boxing works.
            // Again, Json.NET probably prevents us from needing to check this, really.
            if (value is T castValue)
            {
                WriteJsonImpl(writer, castValue, serializer);
                return;
            }
            throw new ArgumentException($"Unexpected value when converting. Expected {typeof(T).FullName}, got {value.GetType().FullName}.");
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="serializer">The serializer to use for nested serialization</param>
        protected abstract void WriteJsonImpl(JsonWriter writer, T value, JsonSerializer serializer);
    }
}
