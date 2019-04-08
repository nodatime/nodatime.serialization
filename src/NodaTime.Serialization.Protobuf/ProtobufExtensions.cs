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
    public static class ProtobufExtensions
    {
        // These correspond to 0001-01-01T00:00:00 and 9999-12-31T23:59:59 respectively.
        // They are checked in ProtobufExtensiosnTest.cs.
        internal const long MinValidTimestampSeconds = -62135596800L;
        internal const long MaxValidTimestampSeconds = 253402300799;

        /// <summary>
        /// Converts a Protobuf <see cref="ProtobufDuration"/> to a Noda Time <see cref="NodaDuration"/>.
        /// </summary>
        /// <remarks>
        /// Every valid Protobuf duration can be represented in Noda Time without loss of information.
        /// </remarks>
        /// <param name="duration">The duration to convert. Must not be null.</param>
        /// <exception cref="ArgumentException"><paramref name="duration"/> represents an invalid duration.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="duration"/> is null.</exception>
        /// <returns>The Noda Time representation.</returns>
        public static NodaDuration ToNodaDuration(this ProtobufDuration duration)
        {
            Preconditions.CheckNotNull(duration, nameof(duration));
            long seconds = duration.Seconds;
            long nanos = duration.Nanos;
            Preconditions.CheckArgument(
                seconds >= ProtobufDuration.MinSeconds &&
                seconds <= ProtobufDuration.MaxSeconds,
                nameof(duration),
                "duration.Seconds out of range: {0}",
                seconds);
            Preconditions.CheckArgument(
                nanos > -ProtobufDuration.NanosecondsPerSecond &&
                nanos < ProtobufDuration.NanosecondsPerSecond,
                nameof(duration),
                "duration.Nanos out of range: {0}",
                nanos);
            // If either sign is 0, we're fine. Otherwise, they should be the same. Multiplying them
            // together seems the easiest way to check that.
            Preconditions.CheckArgument(Math.Sign(seconds) * Math.Sign(nanos) != -1,
                nameof(duration),
                "duration.Seconds and duration.Nanos have different signs: {0}s {1}ns",
                seconds, nanos);
            return NodaDuration.FromSeconds(seconds) + NodaDuration.FromNanoseconds(nanos);
        }

        /// <summary>
        /// Converts a Protobuf <see cref="Timestamp"/> to a Noda Time <see cref="Instant"/>.
        /// </summary>
        /// <remarks>
        /// Every valid Protobuf timestamp can be represented in Noda Time without loss of information.
        /// </remarks>
        /// <param name="timestamp">The timestamp to convert. Must not be null.</param>
        /// <exception cref="ArgumentException"><paramref name="timestamp"/> represents an invalid timestamp.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="timestamp"/> is null.</exception>
        /// <returns>The Noda Time representation.</returns>
        public static Instant ToInstant(this Timestamp timestamp)
        {
            Preconditions.CheckNotNull(timestamp, nameof(timestamp));
            long seconds = timestamp.Seconds;
            long nanos = timestamp.Nanos;
            Preconditions.CheckArgument(
                seconds >= MinValidTimestampSeconds &&
                seconds <= MaxValidTimestampSeconds,
                nameof(timestamp),
                "timestamp.Seconds out of range {0}",
                seconds);
            Preconditions.CheckArgument(
                nanos >= 0 &&
                nanos < ProtobufDuration.NanosecondsPerSecond,
                nameof(timestamp),
                "timestamp.Nanos out of range: {0}",
                nanos);
            return Instant.FromUnixTimeSeconds(seconds).PlusNanoseconds(nanos);
        }

        /// <summary>
        /// Converts a Protobuf <see cref="TimeOfDay"/> to a Noda Time <see cref="LocalTime"/>.
        /// </summary>
        /// <param name="timeOfDay">The time of day to convert. Must not be null</param>
        /// <exception cref="ArgumentException">The time of day is invalid, uses a leap second, or indicates 24:00.</exception>
        /// <returns>The Noda Time representation.</returns>
        public static LocalTime ToLocalTime(this TimeOfDay timeOfDay)
        {
            Preconditions.CheckNotNull(timeOfDay, nameof(timeOfDay));
            int hours = timeOfDay.Hours;
            int minutes = timeOfDay.Minutes;
            int seconds = timeOfDay.Seconds;
            int nanos = timeOfDay.Nanos;
            Preconditions.CheckArgument(hours >= 0 && hours < 24, nameof(timeOfDay),
                "duration.hours out of range: {0}", hours);
            Preconditions.CheckArgument(minutes >= 0 && minutes < 60, nameof(timeOfDay),
                "duration.minutes out of range: {0}", minutes);
            Preconditions.CheckArgument(seconds >= 0 && seconds < 60, nameof(timeOfDay),
                "duration.seconds out of range: {0}", seconds);
            Preconditions.CheckArgument(nanos >= 0 && nanos < NodaConstants.NanosecondsPerSecond, nameof(timeOfDay),
                "duration.nanos out of range: {0}", nanos);
            return new LocalTime(hours, minutes, seconds).PlusNanoseconds(nanos);
        }

        /// <summary>
        /// Converts a Protobuf <see cref="ProtobufDayOfWeek"/> to a Noda Time <see cref="IsoDayOfWeek"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ProtobufDayOfWeek.Unspecified"/> value maps to <see cref="IsoDayOfWeek.None"/>.
        /// </remarks>
        /// <param name="dayOfWeek">The day-of-week value to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="dayOfWeek"/> is neither Unspecified nor
        /// a valid day-of-week value.</exception>
        /// <returns>The Noda Time representation.</returns>
        public static IsoDayOfWeek ToIsoDayOfWeek(this ProtobufDayOfWeek dayOfWeek)
        {
            // Preconditions doesn't have an enum version.
            if (dayOfWeek < 0 || dayOfWeek > ProtobufDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek,
                    "Only valid day-of-week values (or Unspecified) may be converted");
            }
            // Handily, Noda Time and Protobuf use the same numbers.
            return (IsoDayOfWeek) dayOfWeek;
        }

        /// <summary>
        /// Converts a Protobuf <see cref="Date"/> to a Noda Time <see cref="LocalDate"/>.
        /// </summary>
        /// <remarks>
        /// The resulting date is always in the ISO calendar. The input date must be completely
        /// specified; values with a 0 year, month or day are not supported.
        /// </remarks>
        /// <param name="date">The date to convert. Must not be null.</param>
        /// <returns></returns>
        public static LocalDate ToLocalDate(this Date date)
        {
            Preconditions.CheckNotNull(date, nameof(date));
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            Preconditions.CheckArgument(year != 0 && month != 0 && day != 0, nameof(date),
                "Date messages must be fully-specified (no zero values) to convert to LocalDate.");
            Preconditions.CheckArgument(year >= 1 && year <= 9999, nameof(date),
                "Date.Year must be in the range [1, 9999]. Actual value: {0}", year);
            Preconditions.CheckArgument(month >= 1 && month <= 12, nameof(date),
                "Date.Month must be in the range [1, 12]. Actual value: {0}", month);
            Preconditions.CheckArgument(day >= 1 && day <= CalendarSystem.Iso.GetDaysInMonth(year, month), nameof(date),
                "Date.Day out of range for Year/Month value. Actual value: {0}. Year/month: {1}/{2}", day, year, month);
            return new LocalDate(date.Year, date.Month, date.Day);
        }
    }
}
