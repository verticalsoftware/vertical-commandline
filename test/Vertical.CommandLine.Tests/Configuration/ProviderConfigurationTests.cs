// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ProviderConfigurationTests
    {
        private readonly Mock<IComponentSink<IProvider<object>>> _mockSink =
            new Mock<IComponentSink<IProvider<object>>>();
        private readonly ProviderConfiguration<object> _instanceUnderTest;

        public ProviderConfigurationTests()
        {
            _instanceUnderTest = new ProviderConfiguration<object>(null, _mockSink.Object);
        }

        [Fact]
        public void UseInstanceSinksProvider()
        {
            var instance = new object();
            _mockSink.Setup(m => m.Sink(It.IsAny<InstanceProvider<object>>())).Verifiable();
            _instanceUnderTest.UseInstance(new object());
            _mockSink.Verify(m => m.Sink(It.IsAny<InstanceProvider<object>>()), Times.Once);
        }

        [Fact]
        public void UseFactorySinksProvider()
        {
            _mockSink.Setup(m => m.Sink(It.IsAny<DelegateProvider<object>>())).Verifiable();
            _instanceUnderTest.UseFactory(() => new object());
            _mockSink.Verify(m => m.Sink(It.IsAny<DelegateProvider<object>>()), Times.Once);
        }

        [Fact]
        public void UseDefaultSinksProvider()
        {
            _mockSink.Setup(m => m.Sink(It.IsAny<ConstructorProvider<object>>())).Verifiable();
            _instanceUnderTest.UseDefault();
            _mockSink.Verify(m => m.Sink(It.IsAny<ConstructorProvider<object>>()), Times.Once);
        }
    }
}
