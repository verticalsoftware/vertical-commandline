// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using Moq;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;
using Vertical.CommandLine.Conversion;
using Shouldly;
using Xunit;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ArgumentConfigurationTests
    {
        public class MyOptions
        {
            public string Value { get; set; }
        }

        private class MyConfiguration : ArgumentConfiguration<MyOptions, string>
        {
            public MyConfiguration(IParserBuilder<MyOptions, string> parserBuilder)
                : base(parserBuilder)
            {
            }
        }

        private readonly ArgumentConfiguration<MyOptions, string> _instanceUnderTest;
        private readonly Mock<IParserBuilder<MyOptions, string>> _parserBuilderMock =
            new Mock<IParserBuilder<MyOptions, string>>();

        public ArgumentConfigurationTests()
        {
            _instanceUnderTest = new MyConfiguration(_parserBuilderMock.Object);
        }

        [Fact]
        public void MapperSinks()
        {
            _parserBuilderMock.Setup(m => m.Sink(It.IsAny<PropertyMapper<MyOptions, string>>()))
                .Verifiable();
            _instanceUnderTest.Map.ToProperty(opt => opt.Value);
            _parserBuilderMock.Verify(m => m.Sink(It.IsAny<PropertyMapper<MyOptions, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidatorSinks()
        {
            _parserBuilderMock.Setup(m => m.Sink(It.IsAny<IValidator<string>>())).Verifiable();
            _instanceUnderTest.Validate.Less("a");
            _parserBuilderMock.Verify(m => m.Sink(It.IsAny<IValidator<string>>()), Times.Once);
        }

        [Fact]
        public void ConverterSinks()
        {
            _parserBuilderMock.Setup(m => m.Sink(It.IsAny<IValueConverter<string>>())).Verifiable();
            _instanceUnderTest.Convert.Using(s => s);
            _parserBuilderMock.Verify(m => m.Sink(It.IsAny<IValueConverter<string>>()), Times.Once);
        }

        [Fact]
        public void ConfigureReturnsBuilder()
        {
            var builder = ArgumentConfiguration<MyOptions, string>.Configure(
                new ParserBuilder<MyOptions, string>(),
                _ => { });

            builder.ShouldNotBeNull();
        }
    }
}
