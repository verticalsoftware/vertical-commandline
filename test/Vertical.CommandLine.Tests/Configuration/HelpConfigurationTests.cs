// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using Moq;
using Vertical.CommandLine.Provider;
using Vertical.CommandLine.Help;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class HelpConfigurationTests
    {
        private readonly HelpConfiguration<object> _instanceUnderTest;
        private readonly Mock<IComponentSink<IProvider<IReadOnlyCollection<string>>>> _sinkMock =
            new Mock<IComponentSink<IProvider<IReadOnlyCollection<string>>>>();

        public HelpConfigurationTests()
        {
            _instanceUnderTest = new HelpConfiguration<object>(null, _sinkMock.Object);
        }

        [Fact]
        public void UsingProviderSinksInstance()
        {
            var provider = new Mock<IProvider<IReadOnlyCollection<string>>>().Object;
            _sinkMock.Setup(m => m.Sink(provider)).Verifiable();
            _instanceUnderTest.Using(provider);
            _sinkMock.Verify(m => m.Sink(provider), Times.Once);
        }

        [Fact]
        public void UsingProviderThrowsForNull()
        {
            Should.Throw<ArgumentNullException>(() => _instanceUnderTest.Using(null));
        }

        [Fact]
        public void UsingFileSinksFileProvider()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<IProvider<IReadOnlyCollection<string>>>()))
                .Callback<IProvider<IReadOnlyCollection<string>>>(provider =>
                provider.ShouldBeOfType<FileHelpContentProvider>());
            _instanceUnderTest.UseFile("file:///help.txt");
        }

        [Fact]
        public void UsingContentSinksProvider()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<IProvider<IReadOnlyCollection<string>>>()))
                .Callback<IProvider<IReadOnlyCollection<string>>>(provider =>
                provider.ShouldBeOfType<InstanceProvider<IReadOnlyCollection<string>>>());
            _instanceUnderTest.UseContent(new[] { "content" });
        }
    }
}
