// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Type;
using NodaTime.Serialization.Protobuf;
using NUnit.Framework;
using System;

using ProtoDayOfWeek = Google.Type.DayOfWeek;

namespace NodaTime.Serialization.Test.Protobuf
{
    public partial class ProtobufExtensionsTest
    {

        [Test]
        [TestCase(1, 1, 1)]
        [TestCase(9999, 12, 31)]
        [TestCase(2019, 4, 8)]
        [TestCase(2008, 2, 29)]
        public void ToLocalDate_Valid(int year, int month, int day)
        {
            var date = new Date { Year = year, Month = month, Day = day };
            var actualResult = date.ToLocalDate();
            var expectedResult = new LocalDate(year, month, day);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        [TestCase(0, 1, 1)]
        [TestCase(-1, 1, 1)]
        [TestCase(1, 0, 1)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 0, 0)]
        [TestCase(10000, 1, 1)]
        [TestCase(2007, 2, 29)]
        [TestCase(1, 13, 1)]
        [TestCase(1, 1, 32)]
        public void ToLocalDate_Invalid(int year, int month, int day)
        {
            var date = new Date { Year = year, Month = month, Day = day };
            Assert.Throws<ArgumentException>(() => date.ToLocalDate());
        }

        [Test]
        public void ToLocalDate_Null() =>
            Assert.Throws<ArgumentNullException>(() => ((Date) null).ToLocalDate());
    }
}
