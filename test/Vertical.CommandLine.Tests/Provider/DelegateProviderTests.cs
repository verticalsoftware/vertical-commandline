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
    public class DelegateProviderTests
    {
        [Fact]
        public void ConstructWithNullFunctionThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DelegateProvider<object>(null));
        }

        [Fact]
        public void GetInstanceReturnsDelegateResult()
        {
            var options = new object();

            new DelegateProvider<object>(() => options).GetInstance().ShouldBeSameAs(options);
        }
    }
}
