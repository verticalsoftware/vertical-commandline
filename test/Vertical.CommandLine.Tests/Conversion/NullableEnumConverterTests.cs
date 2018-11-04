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
    public class NullableEnumConverterTests
    {
        [Fact]
        public void TryCreateReturnsInstanceForEnumType()
        {
            NullableEnumConverter<DayOfWeek?>.TryCreate(out _).ShouldBeTrue();
        }

        [Fact]
        public void ConvertConvertsString()
        {
            NullableEnumConverter<DayOfWeek?>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(nameof(DayOfWeek.Sunday)).ShouldBe(DayOfWeek.Sunday);
        }

        [Fact]
        public void TryCreateReturnsFalseForNonNullableTypeReturnsFalse()
        {
            NullableEnumConverter<int>.TryCreate(out _).ShouldBeFalse();
        }

        [Fact]
        public void TryCreateReturnsFalseForNonEnumUnderlyingType()
        {
            NullableEnumConverter<int?>.TryCreate(out _).ShouldBeFalse();
        }

        [Fact]
        public void ConvertWithInvalidEnumValueThrows()
        {
            NullableEnumConverter<DayOfWeek?>.TryCreate(out var converter);
            Should.Throw<ArgumentException>(() => converter.Convert("NoDay"));
        }
    }
}
