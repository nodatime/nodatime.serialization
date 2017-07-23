// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using static NodaTime.Serialization.Protobuf.ProtobufExtensions;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class ProtobufExtensionsTest
    {
        [Test]
        public void MinMaxValidTimestampSeconds()
        {
            // These are useful to have as compile-time constants, but let's validate them.
            Assert.AreEqual(MinValidTimestampSeconds, Instant.FromUtc(1, 1, 1, 0, 0).ToUnixTimeSeconds());
            Assert.AreEqual(MaxValidTimestampSeconds, Instant.FromUtc(9999, 12, 31, 23, 59, 59).ToUnixTimeSeconds());
        }
    }
}
