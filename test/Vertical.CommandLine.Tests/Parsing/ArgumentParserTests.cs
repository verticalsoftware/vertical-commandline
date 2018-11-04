// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;
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
    public class ArgumentParserTests
    {
        private class MyParser : ArgumentParser<object, string>
        {
            public MyParser(Template template, ParserType parserType,
                IValueConverter<string> converter,
                IValidator<string> validator,
                IMapper<object, string> mapper) :
                base(template, parserType, converter, validator, mapper)
            {
            }

            protected override string Context => "context";

            public override ContextResult ProcessContext(object options, ParseContext parseContext)
            {
                AcceptArgumentValue(options, new Token(TokenType.NonTemplateValue, "test"));
                return ContextResult.NoMatch;
            }
        }

        private readonly Mock<IValueConverter<string>> _mockConverter = new Mock<IValueConverter<string>>();
        private readonly Mock<IMapper<object, string>> _mockMapper =
            new Mock<IMapper<object, string>>();
        private readonly Mock<IValidator<string>> _mockValidator = new Mock<IValidator<string>>();

        [Fact]
        public void ConstructWithNullConverterThrows()
        {
            Should.Throw<ArgumentNullException>(() => new MyParser(null,
                ParserType.Command,
                null,
                null,
                _mockMapper.Object));
        }

        [Fact]
        public void ConstructWithNullMapperThrows()
        {
            Should.Throw<ArgumentNullException>(() => new MyParser(null,
                ParserType.Command,
                _mockConverter.Object,
                null,
                null));
        }

        [Fact]
        public void MultiValuedReturnsMapperValue()
        {
            _mockMapper.SetupGet(m => m.MultiValued).Returns(true);
            
            var parser = new MyParser(null, ParserType.PositionArgument,
                _mockConverter.Object,
                null,
                _mockMapper.Object);

            parser.MultiValued.ShouldBeTrue();
        }

        [Fact]
        public void AcceptArgumentValueInvokesConverter()
        {
            _mockConverter.Setup(m => m.Convert("test")).Verifiable();
            
            var parser = new MyParser(null, ParserType.Command, _mockConverter.Object,
                null, _mockMapper.Object);
            parser.ProcessContext(new object(), new ParseContext(Enumerable.Empty<string>()));
            
            _mockConverter.Verify(m => m.Convert("test"), Times.Once());
        }

        [Fact]
        public void AcceptArgumentValueInvokesValidator()
        {
            _mockValidator.Setup(m => m.Validate("test")).Returns(true).Verifiable();
            
            var parser = new MyParser(null, ParserType.Command, ConverterFactory.CreateOrThrow<string>(),
                _mockValidator.Object, _mockMapper.Object);
            parser.ProcessContext(new object(), new ParseContext(Enumerable.Empty<string>()));
            
            _mockValidator.Verify(m => m.Validate("test"), Times.Once);
        }

        [Fact]
        public void AcceptArgumentValueInvokesMapper()
        {
            var options = new object();
            
            var parser = new MyParser(null, ParserType.Command, ConverterFactory.CreateOrThrow<string>(),
                null,
                _mockMapper.Object);
            
            _mockMapper.Setup(m => m.MapValue(options, "test")).Verifiable();
            parser.ProcessContext(options, new ParseContext(Enumerable.Empty<string>()));
            
            _mockMapper.Verify(m => m.MapValue(options, "test"), Times.Once);
        }

        [Fact]
        public void ProcessContextThrowsWhenConversionLogicThrows()
        {
            var converterMock = new Mock<IValueConverter<string>>();
            converterMock.Setup(m => m.Convert(It.IsAny<string>())).Throws<Exception>();
            var parser = new MyParser(null, ParserType.Command, converterMock.Object, null, _mockMapper.Object);
            Should.Throw<ConversionException>(() => parser.ProcessContext(null, null));
        }

        [Fact]
        public void ProcessContextThrowsWhenValidationThrows()
        {
            var validatorMock = new Mock<IValidator<string>>();
            validatorMock.Setup(m => m.Validate(It.IsAny<string>())).Throws<Exception>();
            var parser = new MyParser(null, ParserType.Command, _mockConverter.Object,
                validatorMock.Object,
                _mockMapper.Object);
            Should.Throw<ConfigurationException>(() => parser.ProcessContext(null, null));
        }
    }
}