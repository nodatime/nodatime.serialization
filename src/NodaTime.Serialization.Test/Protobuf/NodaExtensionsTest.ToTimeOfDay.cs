// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.Protobuf;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class NodaExtensionsTest
    {
        [Test]
        [TestCase("00:00:00.000000000", 0, 0, 0, 0)]
        [TestCase("00:00:00.999999999", 0, 0, 0, (int)NodaConstants.NanosecondsPerSecond - 1)]
        [TestCase("23:59:59.999999999", 23, 59, 59, (int)NodaConstants.NanosecondsPerSecond - 1)]
        // Just a non-extreme value
        [TestCase("12:45:23.000500000", 12, 45, 23, 500000)]
        public void ToTimeOfDay_Valid(string localTimeText, int expectedHours, int expectedMinutes, int expectedSeconds, int expectedNanos)
        {
            var pattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm:ss.fffffffff");
            var localTime = pattern.Parse(localTimeText).Value;
            var timeOfDay = localTime.ToTimeOfDay();
            Assert.AreEqual(expectedHours, timeOfDay.Hours);
            Assert.AreEqual(expectedMinutes, timeOfDay.Minutes);
            Assert.AreEqual(expectedSeconds, timeOfDay.Seconds);
            Assert.AreEqual(expectedNanos, timeOfDay.Nanos);
        }
    }
}
