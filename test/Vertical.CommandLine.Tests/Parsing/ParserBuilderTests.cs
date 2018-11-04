// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Validation;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class ParserBuilderTests : ParserTestsBase<object>
    {
        public sealed class NotConvertible
        {
        }

        private readonly ParserBuilder<object, object> _instanceUnderTest = new ParserBuilder<object, object>();

        public ParserBuilderTests()
        {
            ((IComponentSink<IValueConverter<object>>)_instanceUnderTest).Sink(ConverterMock.Object);
            ((IComponentSink<IMapper<object, object>>)_instanceUnderTest).Sink(MapperMock.Object);
            ((IComponentSink<IValidator<object>>)_instanceUnderTest).Sink(ValidatorMock.Object);

            ValidatorMock.Setup(m => m.Validate(It.IsAny<object>())).Returns(true);

            _instanceUnderTest.PositionArgument(0).ProcessContext(new object(), new ParseContext(new[] { "test" }));
        }

        [Fact]
        public void ParserWithNoMapperThrows()
        {
            var parserBuilder = new ParserBuilder<object, object>();
            Should.Throw<ConfigurationException>(() => parserBuilder.Option(Template.ForOptionOrSwitch("--test")));
        }

        [Fact]
        public void ParserWithNoConverterThrows()
        {
            var parserBuilder = new ParserBuilder<object, NotConvertible>();
            ((IComponentSink<IMapper<object, NotConvertible>>)parserBuilder).Sink(new Mock<IMapper<object, NotConvertible>>().Object);
            Should.Throw<ConfigurationException>(() => parserBuilder.PositionArgument(0));
        }

        [Fact]
        public void ConverterSinks()
        {
            ConverterMock.Verify(m => m.Convert(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void MapperSinks()
        {
            MapperMock.Verify(m => m.MapValue(It.IsAny<object>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void ValidatorSinks()
        {
            ValidatorMock.Verify(m => m.Validate(It.IsAny<object>()), Times.Once);
        }
    }
}