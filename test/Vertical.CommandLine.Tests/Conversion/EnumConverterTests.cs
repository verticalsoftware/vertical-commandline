// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Vertical.CommandLine.Conversion;
using Xunit;

namespace Vertical.CommandLine.Tests.Conversion
{
    public class EnumConverterTests
    {
        [Fact]
        public void TryCreateReturnsInstanceForEnumType()
        {
            EnumConverter<DayOfWeek>.TryCreate(out _).ShouldBeTrue();
        }

        [Fact]
        public void ConvertConvertsString()
        {
            EnumConverter<DayOfWeek>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(DayOfWeek.Sunday.ToString()).ShouldBe(DayOfWeek.Sunday);
        }

        [Fact]
        public void TryCreateReturnsFalseForNonEnumTypeReturnsFalse()
        {
            EnumConverter<int>.TryCreate(out _).ShouldBeFalse();
        }

        [Fact]
        public void ConvertWithInvalidEnumValueThrows()
        {
            EnumConverter<DayOfWeek>.TryCreate(out var converter);
            Should.Throw<ArgumentException>(() => converter.Convert("NoDay"));
        }
    }
}
