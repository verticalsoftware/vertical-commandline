// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class SwitchParserTests : ParserTestsBase<bool>
    {
        private readonly SwitchParser<object, bool> _instanceUnderTest;
        private const string TemplateValue = "-t";
        private readonly Template _template = Template.ForOptionOrSwitch(TemplateValue);

        public SwitchParserTests()
        {
            _instanceUnderTest = new SwitchParser<object, bool>(_template,
                ConverterMock.Object, ValidatorMock.Object, MapperMock.Object);
        }

        [Fact]
        public void ProcessContextConsumesSwitch()
        {
            var context = new ParseContext(new[] { TemplateValue });
            _instanceUnderTest.ProcessContext(new object(), context);
            context.Ready.ShouldBeFalse();
        }

        [Fact]
        public void ProcessContextReturnsArgumentForMatch()
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { TemplateValue })).ShouldBe(ContextResult.Argument);
        }

        [Theory]
        [InlineData("arg")]
        [InlineData("-h")]
        [InlineData("--test")]
        public void ProcessContextReturnsNoMatch(string arg)
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { arg })).ShouldBe(ContextResult.NoMatch);
        }

        [Fact]
        public void ProcessContextMapsAcceptedValue()
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { TemplateValue }));
            VerifyMocks();
        }
    }
}
