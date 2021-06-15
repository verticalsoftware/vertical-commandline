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
    public class ConversionExceptionTests
    {
        private const string Context = "context";
        private const string Value = "arg";
        private static readonly Type Type = typeof(object);

        private readonly ConversionException _instanceUnderTest = new ConversionException(Context,
            Type, Value, null);

        [Fact]
        public void ConstructAssignsContext()
        {
            _instanceUnderTest.Context.ShouldBe(Context);
        }

        [Fact]
        public void ConstructAssignsValue()
        {
            _instanceUnderTest.ArgumentValue.ShouldBe(Value);
        }

        [Fact]
        public void ConstructAssignsType()
        {
            _instanceUnderTest.TargetType.ShouldBe(Type);
        }
    }
}