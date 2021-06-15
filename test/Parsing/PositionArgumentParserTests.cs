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
    public class PositionArgumentParserTests : ParserTestsBase<string>
    {
        private readonly PositionArgumentParser<object, string> _instanceUnderTest;

        public PositionArgumentParserTests()
        {
            _instanceUnderTest = new PositionArgumentParser<object, string>(0,
                ConverterMock.Object,
                ValidatorMock.Object,
                MapperMock.Object);
        }

        [Fact]
        public void ParserTypeReturnsPositionArgument()
        {
            _instanceUnderTest.ParserType.ShouldBe(ParserType.PositionArgument);
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("-abc")]
        public void ProcessContextReturnsNoMatchForNonStringArguments(string arg)
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { arg })).ShouldBe(ContextResult.NoMatch);
        }

        [Fact]
        public void ProcessContextConsumesStringArg()
        {
            var context = new ParseContext(new[] { "arg" });
            _instanceUnderTest.ProcessContext(new object(), context);
            context.Ready.ShouldBeFalse();
        }

        [Fact]
        public void ProcessContextMapsAcceptedArguments()
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { "arg" }));
            VerifyMocks();
        }

        [Fact]
        public void ProcessContextReturnsArgumentOnMatch()
        {
            _instanceUnderTest.ProcessContext(new object(), new ParseContext(new[] { "arg" })).ShouldBe(ContextResult.Argument);
        }
    }
}
