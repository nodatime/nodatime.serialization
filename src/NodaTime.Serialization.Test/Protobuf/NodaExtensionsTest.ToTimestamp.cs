// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.Protobuf;
using NodaTime.Text;
using NUnit.Framework;
using System;
using static NodaTime.Serialization.Protobuf.ProtobufExtensions;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class NodaExtensionsTest
    {
        [Test]
        public void ToTimestamp_OutOfRange()
        {
            var instant = NodaConstants.BclEpoch.PlusNanoseconds(-1);
            Assert.Throws<ArgumentOutOfRangeException>(() => instant.ToTimestamp());
        }

        [Test]
        [TestCase("0001-01-01T00:00:00.000000000", MinValidTimestampSeconds, 0)]
        [TestCase("9999-12-31T23:59:59.999999999", MaxValidTimestampSeconds, (int)(NodaConstants.NanosecondsPerSecond - 1))]
        [TestCase("1970-01-01T00:00:00.000000000", 0, 0)]
        [TestCase("1970-01-01T00:00:00.000000001", 0, 1)]
        [TestCase("1970-01-01T00:00:01.000000000", 1, 0)]
        [TestCase("1969-12-31T23:59:59.000000000", -1, 0)]
        [TestCase("2017-07-24T09:37:05.123456789", 1500889025, 123456789)]
        public void ToTimestamp_Valid(string instantText, long expectedSeconds, int expectedNanos)
        {
            var pattern = InstantPattern.CreateWithInvariantCulture("uuuu-MM-dd'T'HH:mm:ss.fffffffff");
            var instant = pattern.Parse(instantText).Value;
            var timestamp = instant.ToTimestamp();
            Assert.AreEqual(expectedSeconds, timestamp.Seconds);
            Assert.AreEqual(expectedNanos, timestamp.Nanos);
        }

    }
}
