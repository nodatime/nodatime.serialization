// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.Protobuf;
using NUnit.Framework;
using System;

using ProtoDayOfWeek = Google.Type.DayOfWeek;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class ProtobufExtensionsTest
    {
        // Might as well just list everything...
        [Test]
        [TestCase(ProtoDayOfWeek.Unspecified, IsoDayOfWeek.None)]
        [TestCase(ProtoDayOfWeek.Sunday, IsoDayOfWeek.Sunday)]
        [TestCase(ProtoDayOfWeek.Monday, IsoDayOfWeek.Monday)]
        [TestCase(ProtoDayOfWeek.Tuesday, IsoDayOfWeek.Tuesday)]
        [TestCase(ProtoDayOfWeek.Wednesday, IsoDayOfWeek.Wednesday)]
        [TestCase(ProtoDayOfWeek.Thursday, IsoDayOfWeek.Thursday)]
        [TestCase(ProtoDayOfWeek.Friday, IsoDayOfWeek.Friday)]
        [TestCase(ProtoDayOfWeek.Saturday, IsoDayOfWeek.Saturday)]
        public void Valid(ProtoDayOfWeek input, IsoDayOfWeek expectedOutput) =>
            Assert.AreEqual(expectedOutput, input.ToIsoDayOfWeek());

        [Test]
        [TestCase((ProtoDayOfWeek) (-1))]
        [TestCase((ProtoDayOfWeek) 8)]
        public void OutOfRange(ProtoDayOfWeek value) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => value.ToIsoDayOfWeek());
    }
}
