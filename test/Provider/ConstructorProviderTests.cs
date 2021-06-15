// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Provider;
using Xunit;

namespace Vertical.CommandLine.Tests.Provider
{
    public class ConstructorProviderTests
    {
        private class Parameterized
        {
            public Parameterized(string _) { }
            protected Parameterized() { }
        }

        private class SomeType { }

        [Fact]
        public void ThrowsForNoDefaultConstructor()
        {
            Should.Throw<ConfigurationException>(() => ConstructorProvider<Parameterized>.CreateOrThrow());
        }

        [Fact]
        public void CreateOrThrowReturnsInstance()
        {
            ConstructorProvider<SomeType>.CreateOrThrow().ShouldNotBeNull();
        }

        [Fact]
        public void ProviderCreatesInstance()
        {
            ConstructorProvider<SomeType>.CreateOrThrow().GetInstance().ShouldBeOfType<SomeType>();
        }
    }
}
