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
    public class DelegateConverterTests
    {
        [Fact]
        public void ConstructWithNullDelegateThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DelegateConverter<string>(null));
        }

        [Fact]
        public void ConstructAssignsDelegateAndConverts()
        {
            new DelegateConverter<int>(int.Parse).Convert("10").ShouldBe(10);
        }
    }
}
