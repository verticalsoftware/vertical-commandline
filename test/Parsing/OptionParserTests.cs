// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
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
    public class OptionParserTests
    {
        public class MyOptions
        {
            public string Value { get; set; }
        }

        private readonly MyOptions _options = new MyOptions();

        private readonly IArgumentParser<MyOptions> _instanceUnderTest =
            new OptionParser<MyOptions, string>(Template.ForOptionOrSwitch("--option"), ConverterFactory.CreateOrThrow<string>(),
                null,
                PropertyMapper<MyOptions, string>.Create(opt => opt.Value));

        [Fact]
        public void ProcessContextAcceptsArgument()
        {
            _instanceUnderTest.ProcessContext(_options, new ParseContext(new[] {"--option", "test"}))
                .ShouldBe(ContextResult.Argument);
            _options.Value.ShouldBe("test");
        }

        [Fact]
        public void ProcessContextAcceptsArgumentWithDash()
        {
            _instanceUnderTest.ProcessContext(_options, new ParseContext(new[] {"--option=-test"}))
                .ShouldBe(ContextResult.Argument);
            _options.Value.ShouldBe("-test");
        }

        [Fact]
        public void ProcessCompositeOptionWithEquals()
        {
            _instanceUnderTest.ProcessContext(_options, new ParseContext(new[] { "--option=value" }));
            _options.Value.ShouldBe("value");
        }
        
        [Fact]
        public void ProcessCompositeOptionWithColon()
        {
            _instanceUnderTest.ProcessContext(_options, new ParseContext(new[] { "--option:value" }));
            _options.Value.ShouldBe("value");
        }

        [Fact]
        public void ProcessContextNoMatchForNoOption()
        {
            _instanceUnderTest.ProcessContext(_options, new ParseContext(new[]{"option", "test"}))
                .ShouldBe(ContextResult.NoMatch);
        }

        [Fact]
        public void ProcessContextWithShortOptions()
        {
            var uut =
                new OptionParser<MyOptions, string>(Template.ForOptionOrSwitch("-o"), ConverterFactory.CreateOrThrow<string>(),
                    null,
                    PropertyMapper<MyOptions, string>.Create(opt => opt.Value));
            uut.ProcessContext(_options, new ParseContext(new[] { "-o", "value" }));
            _options.Value.ShouldBe("value");
        }
        
        [Fact]
        public void ProcessContextWithCapitalizedShortOptions()
        {
            var uut =
                new OptionParser<MyOptions, string>(Template.ForOptionOrSwitch("-S"), ConverterFactory.CreateOrThrow<string>(),
                    null,
                    PropertyMapper<MyOptions, string>.Create(opt => opt.Value));
            uut.ProcessContext(_options, new ParseContext(new[] { "-S", "value" }));
            _options.Value.ShouldBe("value");
        }
        
        [Fact]
        public void ProcessContextWithCapitalizedShortAndLongOptionsUsingShortOption()
        {
            var uut =
                new OptionParser<MyOptions, string>(Template.ForOptionOrSwitch("-S|--long-option"), ConverterFactory.CreateOrThrow<string>(),
                    null,
                    PropertyMapper<MyOptions, string>.Create(opt => opt.Value));
            uut.ProcessContext(_options, new ParseContext(new[] { "-S", "value" }));
            _options.Value.ShouldBe("value");
        }
        
        [Fact]
        public void ProcessContextWithCapitalizedShortAndLongOptionsUsingLongOption()
        {
            var uut =
                new OptionParser<MyOptions, string>(Template.ForOptionOrSwitch("-S|--long-option"), ConverterFactory.CreateOrThrow<string>(),
                    null,
                    PropertyMapper<MyOptions, string>.Create(opt => opt.Value));
            uut.ProcessContext(_options, new ParseContext(new[] { "--long-option", "value" }));
            _options.Value.ShouldBe("value");
        }

        [Theory]
        [InlineData("--")]
        [InlineData("arg")]
        [InlineData("-@")]
        [InlineData("--w@rd")]
        public void ConstructWithInvalidTokenThrows(string template)
        {
            Should.Throw<ConfigurationException>(() => new OptionParser<object, string>(
               Template.ForOptionOrSwitch(template), 
                new Mock<IValueConverter<string>>().Object, 
                new Mock<IValidator<string>>().Object,
                new Mock<IMapper<object, string>>().Object));
        }
        
        [Fact]
        public void ProcessContextNoOperandThrows()
        {
            Should.Throw<UsageException>(() => _instanceUnderTest.ProcessContext(_options,
                new ParseContext(new[] {"--option"})));
        }
    }
}