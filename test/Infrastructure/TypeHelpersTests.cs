// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Shouldly;
using Vertical.CommandLine.Infrastructure;
using Xunit;

namespace Vertical.CommandLine.Tests.Infrastructure
{
    public class TypeHelpersTests
    {
        [Fact]
        public void GetKnownMethodInfoThrowsForInvalidMethod()
        {
            Should.Throw<MissingMethodException>(() => typeof(string).GetKnownMethodInfo("NotAStringMethod",
                Array.Empty<Type>(),
                typeof(void)));
        }

        [Theory]
        [InlineData(typeof(string), "String")]
        [InlineData(typeof(int), "Int32")]
        [InlineData(typeof(int?), "Nullable<Int32>")]
        [InlineData(typeof(Dictionary<int?, string>), "Dictionary<Nullable<Int32>,String>")]
        public void GetFriendlyNameReturnsExpected(Type type, string expected)
        {
            TypeHelpers.GetFriendlyDisplayName(type).ShouldBe(expected);
        }
    }
}