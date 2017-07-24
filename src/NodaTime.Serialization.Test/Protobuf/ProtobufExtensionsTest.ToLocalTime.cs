// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Type;
using NodaTime.Serialization.Protobuf;
using NodaTime.Text;
using NUnit.Framework;
using System;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class ProtobufExtensionsTest
    {
        /// <summary>
        /// These are times of day which are invalid in Protobuf.
        /// </summary>
        [Test]
        [TestCase(-1, 0, 0, 0)]
        [TestCase(25, 0, 0, 0)] // 24 is handled below
        [TestCase(0, -1, 0, 0)]
        [TestCase(0, 60, 0, 0)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, 61, 0)] // 60 is handled below
        [TestCase(0, 0, 0, -1)]
        [TestCase(0, 0, 0, (int) NodaConstants.NanosecondsPerSecond)]
        public void ToLocalTime_InvalidTimeOfDay(int hours, int minutes, int seconds, int nanos)
        {
            var timeOfDay = new TimeOfDay { Hours = hours, Minutes = minutes, Seconds = seconds, Nanos = nanos };
            Assert.Throws<ArgumentException>(() => timeOfDay.ToLocalTime());
        }

        /// <summary>
        /// These are times of day which are valid in Protobuf,
        /// but not supported by Noda Time.
        /// </summary>
        [Test]
        [TestCase(25, 0, 0, 0, Description = "End of day, 24:00")]
        [TestCase(0, 0, 60, 0, Description = "Leap second")]
        public void ToLocalTime_UnhandledTimeOfDay(int hours, int minutes, int seconds, int nanos)
        {
            var timeOfDay = new TimeOfDay { Hours = hours, Minutes = minutes, Seconds = seconds, Nanos = nanos };
            Assert.Throws<ArgumentException>(() => timeOfDay.ToLocalTime());
        }

        [Test]
        [TestCase(0, 0, 0, 0, "00:00:00.000000000")]
        [TestCase(0, 0, 0, (int) NodaConstants.NanosecondsPerSecond - 1, "00:00:00.999999999")]
        [TestCase(23, 59, 59, (int)NodaConstants.NanosecondsPerSecond - 1, "23:59:59.999999999")]
        // Just a non-extreme value
        [TestCase(12, 45, 23, 500000, "12:45:23.000500000")]
        public void ToLocalTime_Valid(int hours, int minutes, int seconds, int nanos, string expectedResult)
        {
            var pattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm:ss.fffffffff");
            var timeOfDay = new TimeOfDay { Hours = hours, Minutes = minutes, Seconds = seconds, Nanos = nanos };
            var localTime = timeOfDay.ToLocalTime();
            Assert.AreEqual(expectedResult, pattern.Format(localTime));
        }
    }
}
