// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Protobuf.WellKnownTypes;
using Google.Type;
using System;
using NodaDuration = NodaTime.Duration;
using ProtobufDuration = Google.Protobuf.WellKnownTypes.Duration;
using ProtobufDayOfWeek = Google.Type.DayOfWeek;

namespace NodaTime.Serialization.Protobuf
{
    /// <summary>
    /// Extension methods on the Google.Protobuf time-related types to convert them to Noda Time types.
    /// </summary>
    public static class NodaExtensions
    {
        private static readonly NodaDuration minProtobufDuration =
            NodaDuration.FromSeconds(ProtobufDuration.MinSeconds - 1) + NodaDuration.FromNanoseconds(1);

        private static readonly NodaDuration maxProtobufDuration =
            NodaDuration.FromSeconds(ProtobufDuration.MaxSeconds + 1) - NodaDuration.FromNanoseconds(1);

        /// <summary>
        /// Converts a Noda Time <see cref="NodaDuration"/> to a Protobuf <see cref="ProtobufDuration"/>.
        /// </summary>
        /// <remarks>
        /// Noda Time has a wider range of valid durations than Protobuf; durations of more than around 10,000
        /// years (positive or negative) cannot be represented.
        /// </remarks>
        /// <param name="duration">The duration to convert. Must not be null.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> represents a duration
        /// which is invalid in <see cref="ProtobufDuration"/>.</exception>
        /// <returns>The Protobuf representation.</returns>
        public static ProtobufDuration ToProtobufDuration(this NodaDuration duration)
        {
            if (duration < minProtobufDuration || duration > maxProtobufDuration)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), "Duration is outside the range of valid Protobuf durations.");
            }
            // Deliberately long to keep the later arithmetic in 64-bit.
            long days = duration.Days;
            long nanoOfDay = duration.NanosecondOfDay;
            long secondOfDay = nanoOfDay / NodaConstants.NanosecondsPerSecond;
            int nanos = duration.SubsecondNanoseconds;
            return new ProtobufDuration { Seconds = days * NodaConstants.SecondsPerDay + secondOfDay, Nanos = nanos };
        }

        /// <summary>
        /// Converts a Noda Time <see cref="Instant"/> to a Protobuf <see cref="Timestamp"/>.
        /// </summary>
        /// <remarks>
        /// Noda Time has a wider range of valid instants than Protobuf timestamps; instants before 0001-01-01 CE
        /// are out of range.
        /// </remarks>
        /// <param name="instant">The instant to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="instant"/> represents an instant
        /// which is invalid in <see cref="Timestamp"/>.</exception>
        /// <returns>The Protobuf representation.</returns>
        public static Timestamp ToTimestamp(this Instant instant)
        {
            if (instant < NodaConstants.BclEpoch)
            {
                throw new ArgumentOutOfRangeException(nameof(instant), "Instant is outside the range of Valid Protobuf timestamps");
            }
            // Truncated towards the start of time, which is what we want...
            var seconds = instant.ToUnixTimeSeconds();
            var remainder = instant - Instant.FromUnixTimeSeconds(seconds);
            // NanosecondOfDay is probably the most efficient way of turning a small, subsecond, non-negative duration
            // into a number of nanoseconds...
            return new Timestamp { Seconds = seconds, Nanos = (int) remainder.NanosecondOfDay };
        }

        /// <summary>
        /// Converts a Noda Time <see cref="LocalTime"/> to a Protobuf <see cref="TimeOfDay"/>.
        /// </summary>
        /// <remarks>
        /// Every valid Noda Time local time can be represented in Protobuf without loss of information.
        /// </remarks>
        /// <param name="localTime">The local time.</param>
        /// <returns>The Protobuf representation.</returns>
        public static TimeOfDay ToTimeOfDay(this LocalTime localTime) =>
            new TimeOfDay
            {
                Hours = localTime.Hour,
                Minutes = localTime.Minute,
                Seconds = localTime.Second,
                Nanos = localTime.NanosecondOfSecond
            };

        /// <summary>
        /// Converts a Noda Time <see cref="IsoDayOfWeek"/> to a Protobuf <see cref="ProtobufDayOfWeek"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IsoDayOfWeek.None"/> value maps to <see cref="ProtobufDayOfWeek.Unspecified"/>.
        /// </remarks>
        /// <param name="isoDayOfWeek">The ISO day-of-week value to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="isoDayOfWeek"/> is neither None nor
        /// a valid ISO day-of-week value.</exception>
        /// <returns>The Protobuf representation.</returns>
        public static ProtobufDayOfWeek ToProtobufDayOfWeek(this IsoDayOfWeek isoDayOfWeek)
        {
            // Preconditions doesn't have an enum version.
            if (isoDayOfWeek < 0 || isoDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(isoDayOfWeek), isoDayOfWeek,
                    "Only valid day-of-week values (or None) may be converted");
            }
            // Handily, Noda Time and Protobuf use the same numbers.
            return (ProtobufDayOfWeek) isoDayOfWeek;
        }

        /// <summary>
        /// Converts a Noda Time <see cref="LocalDate"/> to a Protobuf <see cref="Date"/>.
        /// </summary>
        /// <remarks>
        /// Only dates in the ISO calendar can be converted, and only those in the year range of 1-9999.
        /// </remarks>
        /// <param name="date">The date to convert.</param>
        /// <returns>The Protobuf representation.</returns>
        public static Date ToDate(this LocalDate date)
        {
            Preconditions.CheckArgument(date.Calendar == CalendarSystem.Iso, nameof(date),
                "Non-ISO dates cannot be converted to Protobuf Date messages. Actual calendar ID: {0}", date.Calendar.Id);
            if (date.Year < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(date),
                    $"Dates earlier than 1AD cannot be converted to Protobuf Date messages. Year: {date.Year}");
            }
            return new Date { Year = date.Year, Month = date.Month, Day = date.Day };
        }
    }
}
