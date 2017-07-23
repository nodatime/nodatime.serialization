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
    public partial class ProtobufExtensionsTest
    {
        // Bounds are from https://github.com/google/protobuf/blob/master/src/google/protobuf/duration.proto

        [Test]
        [TestCase(ProtoDuration.MinSeconds - 1, 0)]
        [TestCase(ProtoDuration.MaxSeconds + 1, 0)]
        [TestCase(0, (int) (-NodaConstants.NanosecondsPerSecond))]
        [TestCase(0, (int) NodaConstants.NanosecondsPerSecond)]
        [TestCase(-1, 1, Description = "Different sign")]
        [TestCase(1, -1, Description = "Different sign")]
        public void ToNodaDuration_Invalid(long seconds, int nanos)
        {
            var input = new ProtoDuration { Seconds = seconds, Nanos = nanos };
            Assert.Throws<ArgumentException>(() => input.ToNodaDuration());
        }

        [TestCase(ProtoDuration.MinSeconds, (int)(-NodaConstants.NanosecondsPerSecond) + 1, "-3652500:00:00:00.999999999")]
        [TestCase(ProtoDuration.MaxSeconds, (int) (NodaConstants.NanosecondsPerSecond - 1), "3652500:00:00:00.999999999")]
        [TestCase(0, 0, "0:00:00:00")]
        [TestCase(-1, 0, "-0:00:00:01")]
        [TestCase(1, 0, "0:00:00:01")]
        [TestCase(0, 1, "0:00:00:00.000000001")]
        [TestCase(0, -1, "-0:00:00:00.000000001")]
        [TestCase(1, 500000000, "0:00:00:01.5")]
        [TestCase(-1, -500000000, "-0:00:00:01.5")]
        public void ToNodaDuration_Valid(long seconds, int nanos, string expectedResult)
        {
            var input = new ProtoDuration { Seconds = seconds, Nanos = nanos };
            var nodaDuration = input.ToNodaDuration();
            Assert.AreEqual(expectedResult, DurationPattern.Roundtrip.Format(nodaDuration));
        }
    }
}
