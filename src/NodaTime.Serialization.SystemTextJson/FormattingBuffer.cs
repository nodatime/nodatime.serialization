// Copyright 2026 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;

namespace NodaTime.Serialization.SystemTextJson;

/// <summary>
/// Per-thread buffers used to format values and write them to a <see cref="System.Text.Json.Utf8JsonWriter"/>
/// without allocating a string per value.
/// </summary>
internal static class FormattingBuffer
{
    // Larger buffers are released rather than cached, so an unusually long value doesn't keep memory alive.
    private const int MaxCachedCapacity = 256;

    [ThreadStatic]
    private static StringBuilder cachedBuilder;

    [ThreadStatic]
    private static char[] cachedChars;

    /// <summary>
    /// Returns an empty builder. The cached builder is handed out at most once at a time, so nested use
    /// on the same thread gets a new builder instead of sharing it.
    /// </summary>
    internal static StringBuilder AcquireBuilder()
    {
        var builder = cachedBuilder;
        if (builder is null)
        {
            return new StringBuilder(64);
        }
        cachedBuilder = null;
        return builder;
    }

    internal static void ReleaseBuilder(StringBuilder builder)
    {
        if (cachedChars is char[] chars)
        {
            Array.Clear(chars, 0, chars.Length);
        }
        if (builder.Capacity <= MaxCachedCapacity)
        {
            builder.Clear();
            cachedBuilder = builder;
        }
    }

    /// <summary>
    /// Copies the contents of <paramref name="builder"/> into a per-thread buffer and returns them as a span.
    /// The span is only valid until the next call on the same thread.
    /// </summary>
    internal static ReadOnlySpan<char> AsSpan(StringBuilder builder)
    {
        int length = builder.Length;
        if (length > MaxCachedCapacity)
        {
            // Not caching large builders, so just convert to string and return as a span.
            return builder.ToString().AsSpan();
        }
        var chars = cachedChars ??= new char[MaxCachedCapacity];
        builder.CopyTo(0, chars, 0, length);
        return new ReadOnlySpan<char>(chars, 0, length);
    }
}
