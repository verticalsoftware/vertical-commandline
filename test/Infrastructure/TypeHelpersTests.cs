// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
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
    }
}