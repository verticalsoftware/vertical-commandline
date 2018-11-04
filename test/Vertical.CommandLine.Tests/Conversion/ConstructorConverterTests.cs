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
    public class ConstructorConverterTests
    {
        public class ConstructableFromString
        {
            public string Value { get; }

            public ConstructableFromString(string str) => Value = str;
        }

        [Fact]
        public void TryCreateReturnsTrueForCompatibleType()
        {
            ConstructorConverter<ConstructableFromString>.TryCreate(out _).ShouldBeTrue();
        }

        [Fact]
        public void InternalFunctionInvokesConstructor()
        {
            ConstructorConverter<ConstructableFromString>.TryCreate(out var converter);

            var expected = Guid.NewGuid().ToString();
            var instance = converter.Convert(expected);
            instance.Value.ShouldBe(expected);
        }

        [Fact]
        public void TryCreateReturnsFalseForIncompatibleType()
        {
            ConstructorConverter<int>.TryCreate(out _).ShouldBeFalse();
        }
    }
}