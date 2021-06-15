// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Vertical.CommandLine.Provider;
using Xunit;

namespace Vertical.CommandLine.Tests.Provider
{
    public class InstanceProviderTests
    {
        [Fact]
        public void ConstructWithNullIntsanceThrows()
        {
            Should.Throw<ArgumentNullException>(() => new InstanceProvider<object>(null));
        }

        [Fact]
        public void GetInstanceReturnsOptions()
        {
            var options = new object();
            new InstanceProvider<object>(options).GetInstance().ShouldBeSameAs(options);
        }
    }
}
