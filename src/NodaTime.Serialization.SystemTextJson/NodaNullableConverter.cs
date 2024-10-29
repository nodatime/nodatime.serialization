// Copyright 2023 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemTextJson;

/// <summary>
/// System.Text.Json converter for <see cref="Nullable{T}"/> value types, wrapping
/// an inner converter.
/// </summary>
/// <typeparam name="T">Value type to be converted.</typeparam>
internal sealed class NodaNullableConverter<T> : JsonConverter<T?> where T : struct
{
    private readonly JsonConverter<T> _innerConverter;

    /// <summary>
    /// Creates a new NodaNullableConverter.
    /// </summary>
    /// <param name="innerConverter">Inner converter for serializing and deserializing when not null.</param>
    public NodaNullableConverter(JsonConverter<T> innerConverter)
    {
        Preconditions.CheckNotNull(innerConverter, nameof(innerConverter));

        _innerConverter = innerConverter;
    }

    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return _innerConverter.Read(ref reader, typeToConvert, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            _innerConverter.Write(writer, value.Value, options);
        }
    }
}
