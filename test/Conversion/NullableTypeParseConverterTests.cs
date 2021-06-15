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
    public class NullableTypeParseConverterTests
    {
        [Fact]
        public void TryCreateReturnsInstanceForNullableType()
        {
            NullableTypeParseConverter<byte?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<short?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<int?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<long?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<float?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<double?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<decimal?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<DateTime?>.TryCreate(out _).ShouldBeTrue();
            NullableTypeParseConverter<TimeSpan?>.TryCreate(out _).ShouldBeTrue();
        }

        [Fact]
        public void ConvertReturnsValueForNonNullInput()
        {
            NullableTypeParseConverter<int?>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert("10").ShouldBe(10);
        }

        [Fact]
        public void ConvertReturnsNullForNullInput()
        {
            NullableTypeParseConverter<int?>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(null).ShouldBe(null);
        }

        [Fact]
        public void ConvertReturnsNullForEmptyInput()
        {
            NullableTypeParseConverter<int?>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(string.Empty).ShouldBe(null);
        }

        [Fact]
        public void ConvertReturnsNullForWhiteSpaceInput()
        {
            NullableTypeParseConverter<int?>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert("  ").ShouldBe(null);
        }
    }
}
