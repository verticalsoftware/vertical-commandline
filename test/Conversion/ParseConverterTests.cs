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
    public class ParseConverterTests
    {
        [Fact]
        public void TryCreateReturnsInstanceForNullableType()
        {
            ParseConverter<byte>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<short>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<int>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<long>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<float>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<double>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<decimal>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<DateTime>.TryCreate(out _).ShouldBeTrue();
            ParseConverter<TimeSpan>.TryCreate(out _).ShouldBeTrue();
        }

        [Fact]
        public void ConvertReturnsValueForNonNullInput()
        {
            ParseConverter<int>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert("10").ShouldBe(10);
        }
    }
}
