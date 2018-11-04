// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Mapping;
using System.Collections.Generic;
using System.Reflection;
using Shouldly;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class MultiValueMappingConfigurationTests
    {
        public class MyOptions
        {
            public ICollection<string> StringCollection { get; set; } = new List<string>();
            public Stack<string> StringStack { get; set; } = new Stack<string>();
            public Queue<string> StringQueue { get; set; } = new Queue<string>();
            public ISet<string> StringSet { get; set; } = new HashSet<string>();
        }

        private readonly Mock<IComponentSink<IMapper<MyOptions, string>>> _sinkMock =
            new Mock<IComponentSink<IMapper<MyOptions, string>>>();
        private readonly MultiValueMappingConfiguration<MyOptions, string> _instanceUnderTest;

        public MultiValueMappingConfigurationTests()
        {
            _instanceUnderTest = new MultiValueMappingConfiguration<MyOptions, string>(null,
                _sinkMock.Object);
        }

        [Fact]
        public void ToCollectionSinksMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<CollectionMapper<MyOptions, string>>()))
                .Verifiable();
            _instanceUnderTest.ToCollection(opt => opt.StringCollection);
            _sinkMock.Verify(m => m.Sink(It.IsAny<CollectionMapper<MyOptions, string>>()),
                Times.Once);
        }

        [Fact]
        public void ToStackSinksMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<StackMapper<MyOptions, string>>()))
                .Verifiable();
            _instanceUnderTest.ToStack(opt => opt.StringStack);
            _sinkMock.Verify(m => m.Sink(It.IsAny<StackMapper<MyOptions, string>>()),
                Times.Once);
        }

        [Fact]
        public void ToQueueSinksMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<QueueMapper<MyOptions, string>>()))
                .Verifiable();
            _instanceUnderTest.ToQueue(opt => opt.StringQueue);
            _sinkMock.Verify(m => m.Sink(It.IsAny<QueueMapper<MyOptions, string>>()),
                Times.Once);
        }

        [Fact]
        public void ToSetSinksMapper()
        {
            _sinkMock.Setup(m => m.Sink(It.IsAny<SetMapper<MyOptions, string>>()))
                .Verifiable();
            _instanceUnderTest.ToSet(opt => opt.StringSet);
            _sinkMock.Verify(m => m.Sink(It.IsAny<SetMapper<MyOptions, string>>()),
                Times.Once);
        }

        [Fact]
        public void MultiValuedReturnsTrue()
        {
            _instanceUnderTest
                .GetType()
                .GetProperty("MultiValued", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance)
                .GetValue(_instanceUnderTest)
                .ShouldBe(true);
        }
    }
}
