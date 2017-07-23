// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Protobuf.WellKnownTypes;
using NodaTime.Serialization.Protobuf;
using NodaTime.Text;
using NUnit.Framework;
using System;
using static NodaTime.Serialization.Protobuf.ProtobufExtensions;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class ProtobufExtensionsTest
    {
        // Bounds are from https://github.com/google/protobuf/blob/master/src/google/protobuf/timestamp.proto

        [Test]
        [TestCase(0, (int) NodaConstants.NanosecondsPerSecond, Description = "Nanos out of range")]
        [TestCase(0, -1, Description = "Nanos out of range")]
        [TestCase(MinValidTimestampSeconds - 1, 0, Description = "Seconds out of range")]
        [TestCase(MaxValidTimestampSeconds + 1, 0, Description = "Seconds out of range")]
        public void ToInstant_InvalidTimestamp(long seconds, int nanos)
        {
            var timestamp = new Timestamp { Seconds = seconds, Nanos = nanos };
            Assert.Throws<ArgumentException>(() => timestamp.ToInstant());
        }
        
        [Test]
        [TestCase(MinValidTimestampSeconds, 0, "0001-01-01T00:00:00.000000000")]
        [TestCase(MaxValidTimestampSeconds, (int) (NodaConstants.NanosecondsPerSecond - 1), "9999-12-31T23:59:59.999999999")]
        [TestCase(0, 0, "1970-01-01T00:00:00.000000000")]
        [TestCase(0, 1, "1970-01-01T00:00:00.000000001")]
        [TestCase(1, 0, "1970-01-01T00:00:01.000000000")]
        [TestCase(-1, 0, "1969-12-31T23:59:59.000000000")]
        [TestCase(1500889025, 123456789, "2017-07-24T09:37:05.123456789")]
        public void ToInstant_Valid(long seconds, int nanos, string expectedResult)
        {
            var pattern = InstantPattern.CreateWithInvariantCulture("uuuu-MM-dd'T'HH:mm:ss.fffffffff");
            var timestamp = new Timestamp { Seconds = seconds, Nanos = nanos };
            var instant = timestamp.ToInstant();
            Assert.AreEqual(expectedResult, pattern.Format(instant));
        }
    }
}
