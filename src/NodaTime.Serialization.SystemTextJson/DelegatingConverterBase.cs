// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson
{
    /// <summary>
    /// Converter which does nothing but delegate to another one for all operations.
    /// </summary>
    /// <remarks>
    /// Nothing in this class is specific to Noda Time. Its purpose is to make it easy
    /// to reuse other converter instances with <see cref="LocalDate"/>,
    /// which can only identify a converter by type.
    /// </remarks>
    /// <example>
    /// <para>
    /// If you had some <see cref="JsonConverterAttribute"/> properties which needed one converter,
    /// but others that needed another, you might want to have different types implementing
    /// those converters. Each type would just derive from this, passing the right converter
    /// into the base constructor.
    /// </para>
    /// <code>
    /// public sealed class ShortDateConverter : DelegatingConverterBase
    /// {
    ///     public ShortDateConverter() : base(NodaConverters.LocalDateConverter) {}
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
    public abstract class DelegatingConverterBase<T> : JsonConverter<T>
    {
        private readonly JsonConverter<T> original;

        /// <summary>
        /// Constructs a converter delegating to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The converter to delegate to. Must not be null.</param>
        protected DelegatingConverterBase(JsonConverter<T> original) =>
            this.original = original ?? throw new ArgumentNullException(nameof(original));

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            original.Write(writer, value, options);

        /// <inheritdoc />
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            original.Read(ref reader, typeToConvert, options);

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => original.CanConvert(objectType);
    }
}
