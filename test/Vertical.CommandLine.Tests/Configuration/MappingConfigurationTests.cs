// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Mapping;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class MappingConfigurationTests
    {
        public class MyOptions
        {
            public string Value { get; set; }
        }

        private readonly MappingConfiguration<MyOptions, string> _instanceUnderTest;
        private readonly Mock<IComponentSink<IMapper<MyOptions, string>>> _sinkMock =
            new Mock<IComponentSink<IMapper<MyOptions, string>>>();

        public MappingConfigurationTests()
        {
            _instanceUnderTest = new MappingConfiguration<MyOptions, string>(null, _sinkMock.Object);
        }

        [Fact]
        public void UsingSinksInstance()
        {
            var mapper = new Mock<IMapper<MyOptions, string>>().Object;
            _sinkMock.Setup(m => m.Sink(mapper)).Verifiable();
            _instanceUnderTest.Using(mapper);
            _sinkMock.Verify(m => m.Sink(mapper), Times.Once);
        }

        [Fact]
        public void UsingActionSinksDelegateMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<DelegateMapper<MyOptions, string>>())).Verifiable();
            _instanceUnderTest.Using((_, __) => { });
            _sinkMock.Verify(m => m.Sink(It.IsAny<DelegateMapper<MyOptions, string>>()), Times.Once);
        }

        [Fact]
        public void ToPropertySinksMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<PropertyMapper<MyOptions, string>>())).Verifiable();
            _instanceUnderTest.ToProperty(opt => opt.Value);
            _sinkMock.Verify(m => m.Sink(It.IsAny<PropertyMapper<MyOptions, string>>()), Times.Once);
        }
    }
}
