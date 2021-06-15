// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Conversion;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ConversionConfigurationTests
    {
        private readonly ConversionConfiguration<object, string> _instanceUnderTest;
        private readonly Mock<IComponentSink<IValueConverter<string>>> _converterSink =
            new Mock<IComponentSink<IValueConverter<string>>>();

        public ConversionConfigurationTests()
        {
            _instanceUnderTest = new ConversionConfiguration<object, string>(null, _converterSink.Object);
        }

        [Fact]
        public void UsingInstanceSinksConverter()
        {
            var converter = new Mock<IValueConverter<string>>().Object;
            _converterSink.Setup(m => m.Sink(converter)).Verifiable();
            _instanceUnderTest.Using(converter);
            _converterSink.Verify(m => m.Sink(converter), Times.Once);
        }

        [Fact]
        public void UsingInstanceThrowsForNull()
        {
            Should.Throw<ArgumentNullException>(() => _instanceUnderTest.Using(default(IValueConverter<string>)));
        }

        [Fact]
        public void UsingFunctionSinksConverter()
        {
            _converterSink.Setup(m => m.Sink(It.IsAny<DelegateConverter<string>>())).Verifiable();
            _instanceUnderTest.Using(str => str);
            _converterSink.Verify(m => m.Sink(It.IsAny<DelegateConverter<string>>()), Times.Once);
        }

        [Fact]
        public void UsingValuesSinksConverter()
        {
            _converterSink.Setup(m => m.Sink(It.IsAny<DictionaryConverter<string>>())).Verifiable();
            _instanceUnderTest.UsingValues(new[]
            {
                new KeyValuePair<string, string>("key", "value")
            }, EqualityComparer<string>.Default);
            _converterSink.Verify(m => m.Sink(It.IsAny<DictionaryConverter<string>>()), Times.Once);
        }

        [Fact]
        public void UsingValuesSinksConverterWithDefaultComparer()
        {
            _converterSink.Setup(m => m.Sink(It.IsAny<DictionaryConverter<string>>())).Verifiable();
            _instanceUnderTest.UsingValues(new[]
            {
                new KeyValuePair<string, string>("key", "value")
            });
            _converterSink.Verify(m => m.Sink(It.IsAny<DictionaryConverter<string>>()), Times.Once);
        }
    }
}
