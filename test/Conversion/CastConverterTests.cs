// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Conversion;
using Xunit;

namespace Vertical.CommandLine.Tests.Conversion
{
    public class Explicit
    {
        internal string Value { get; set; }
        public static explicit operator Explicit(string value) => new Explicit { Value = value };
    }

    public class Implicit
    {
        internal string Value { get; set; }
        public static explicit operator Implicit(string value) => new Implicit { Value = value };
    }

    public class CastConverterTests
    {
        private const string Value = "string";
        
        [Fact]
        public void TryCreateTypeWithExplicitConverterReturnsTrue()
        {
            CastConverter<Explicit>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(Value).Value.ShouldBe(Value);
        }

        [Fact]
        public void TryCreateTypeWithImplicitConverterReturnsTrue()
        {
            CastConverter<Implicit>.TryCreate(out var converter).ShouldBeTrue();
            converter.Convert(Value).Value.ShouldBe(Value);
        }

        [Fact]
        public void TryCreateIncompatibleTypeReturnsFalse()
        {
            CastConverter<CastConverterTests>.TryCreate(out _).ShouldBeFalse();
        }
    }
}
