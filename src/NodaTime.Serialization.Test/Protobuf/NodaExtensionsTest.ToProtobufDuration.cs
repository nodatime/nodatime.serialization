// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.Protobuf;
using NodaTime.Text;
using NUnit.Framework;
using System;
using ProtoDuration = Google.Protobuf.WellKnownTypes.Duration;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class NodaExtensionsTest
    {
        [Test]
        [TestCase(ProtoDuration.MinSeconds - 1)]
        [TestCase(ProtoDuration.MaxSeconds + 1)]
        public void ToProtobufDuration_OutOfRange(long seconds)
        {
            var nodaDuration = Duration.FromSeconds(seconds);
            Assert.Throws<ArgumentOutOfRangeException>(() => nodaDuration.ToProtobufDuration());
        }

        [Test]
        [TestCase("-3652500:00:00:00.999999999", ProtoDuration.MinSeconds, (int)(-NodaConstants.NanosecondsPerSecond) + 1)]
        [TestCase("3652500:00:00:00.999999999", ProtoDuration.MaxSeconds, (int)(NodaConstants.NanosecondsPerSecond - 1))]
        [TestCase("0:00:00:00", 0, 0)]
        [TestCase("-0:00:00:01", -1, 0)]
        [TestCase("0:00:00:01", 1, 0)]
        [TestCase("0:00:00:00.000000001", 0, 1)]
        [TestCase("-0:00:00:00.000000001", 0, -1)]
        [TestCase("0:00:00:01.5", 1, 500000000)]
        [TestCase("-0:00:00:01.5", -1, -500000000)]
        public void ToProtobufDuration_Valid(string nodaDurationText, long expectedSeconds, int expectedNanos)
        {
            Duration nodaDuration = DurationPattern.Roundtrip.Parse(nodaDurationText).Value;
            ProtoDuration protoDuration = nodaDuration.ToProtobufDuration();
            Assert.AreEqual(expectedSeconds, protoDuration.Seconds);
            Assert.AreEqual(expectedNanos, protoDuration.Nanos);
        }
    }
}
