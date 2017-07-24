// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;
using ProtobufDayOfWeek = Google.Type.DayOfWeek;
using NodaTime.Serialization.Protobuf;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class NodaExtensionsTest
    {
        // Might as well just list everything...
        [Test]
        [TestCase(IsoDayOfWeek.None, ProtobufDayOfWeek.Unspecified)]
        [TestCase(IsoDayOfWeek.Sunday, ProtobufDayOfWeek.Sunday)]
        [TestCase(IsoDayOfWeek.Monday, ProtobufDayOfWeek.Monday)]
        [TestCase(IsoDayOfWeek.Tuesday, ProtobufDayOfWeek.Tuesday)]
        [TestCase(IsoDayOfWeek.Wednesday, ProtobufDayOfWeek.Wednesday)]
        [TestCase(IsoDayOfWeek.Thursday, ProtobufDayOfWeek.Thursday)]
        [TestCase(IsoDayOfWeek.Friday, ProtobufDayOfWeek.Friday)]
        [TestCase(IsoDayOfWeek.Saturday, ProtobufDayOfWeek.Saturday)]
        public void Valid(IsoDayOfWeek input, ProtobufDayOfWeek expectedOutput) =>
            Assert.AreEqual(expectedOutput, input.ToProtobufDayOfWeek());

        [Test]
        [TestCase((IsoDayOfWeek)(-1))]
        [TestCase((IsoDayOfWeek)8)]
        public void OutOfRange(IsoDayOfWeek value) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => value.ToProtobufDayOfWeek());

    }
}
