// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Google.Type;
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
        [TestCase(1, 1, 1)]
        [TestCase(9999, 12, 31)]
        [TestCase(2008, 2, 29)]
        public void ToDate_Valid(int year, int month, int day)
        {
            var date = new LocalDate(year, month, day);
            var expectedResult = new Date { Year = year, Month = month, Day = day };
            var actualResult = date.ToDate();
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void ToDate_NonIsoCalendar()
        {
            var date = new LocalDate(100, 1, 1, CalendarSystem.Julian);
            Assert.Throws<ArgumentException>(() => date.ToDate());
        }

        [Test]
        public void ToDate_TooEarly()
        {
            var date = new LocalDate(1, 1, 1).PlusDays(-1);
            Assert.Throws<ArgumentOutOfRangeException>(() => date.ToDate());
        }
    }
}