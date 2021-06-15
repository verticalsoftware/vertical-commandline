// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Moq;
using Shouldly;
using Shouldly.ShouldlyExtensionMethods;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class ParserTests
    {
        public class MyOptions
        {
        
        }

        private readonly MyOptions _myOptions = new MyOptions();

        [Fact]
        public void MapDoesNotInvokedUnmatchedParser()
        {
            var parserMock = CreateParserMock(ParserType.Command, false, _ => { }, ContextResult.Command);
            Parser<MyOptions>.Map(_myOptions, new[] {parserMock.Object}, new ParseContext(new[] {"test"}),
                ParserType.Help);
            parserMock.Verify(m => m.ProcessContext(It.IsAny<MyOptions>(), It.IsAny<ParseContext>()), Times.Never);
        }

        [Fact]
        public void MapInvokesMatchedParser()
        {
            var parserMock = CreateParserMock(ParserType.Command, false, context => context.TryTakeStringValue(out _),
                ContextResult.Command);
            Parser<MyOptions>.Map(_myOptions, new[] {parserMock.Object}, new ParseContext(new[] {"test"}),
                ParserType.Command);
            parserMock.Verify(m => m.ProcessContext(It.IsAny<MyOptions>(), It.IsAny<ParseContext>()), Times.Once);
        }
        
        [Theory]
        [InlineData(ContextResult.Argument)]
        [InlineData(ContextResult.Command)]
        [InlineData(ContextResult.Help)]
        public void MapReturnsProcessContextValue(ContextResult result)
        {
            var parserMock = CreateParserMock(ParserType.Command, false, context => context.TryTakeStringValue(out _),
                result);
            Parser<MyOptions>
                .Map(_myOptions, new[] {parserMock.Object}, new ParseContext(new[] {"test"}), ParserType.Command)
                .ShouldBe(result);
        }

        [Fact]
        public void MapAggregatesResultsToFlags()
        {
            var result = Parser<MyOptions>.Map(_myOptions, new[]
                {
                    CreateParserMock(ParserType.Option, false, context => context.TryTakeTemplate(Template.ForOptionOrSwitch("--help")),
                        ContextResult.Help).Object,
                    CreateParserMock(ParserType.Option, false, context => context.TryTakeTemplate(Template.ForOptionOrSwitch("--test")),
                        ContextResult.Argument).Object
                },
                new ParseContext(new[] {"test"}),
                ParserType.Option);
            
            result.ShouldHaveFlag(ContextResult.Argument);
            result.ShouldHaveFlag(ContextResult.Help);
        }
        
        [Fact]
        public void MapIteratesOnceForSingleValuedParser()
        {
            var parserMock = CreateParserMock(ParserType.PositionArgument, false,
                context => context.TryTakeStringValue(out _),
                ContextResult.Argument);
            var parseContext = new ParseContext(new[] {"value1", "value2"});
            Parser<MyOptions>.Map(_myOptions, new[] {parserMock.Object}, parseContext, ParserType.PositionArgument);
            parserMock.Verify(m => m.ProcessContext(_myOptions, parseContext), Times.Once);
        }

        [Fact]
        public void MapIterateRepeatsOnMultiValuedParser()
        {
            var parserMock = CreateParserMock(ParserType.PositionArgument, true,
                context => context.TryTakeStringValue(out _),
                ContextResult.Argument);
            var parseContext = new ParseContext(new[] {"value1", "value2"});
            Parser<MyOptions>.Map(_myOptions, new[] {parserMock.Object}, parseContext, ParserType.PositionArgument);
            parserMock.Verify(m => m.ProcessContext(_myOptions, parseContext), Times.Exactly(2));
        }
        
        private static Mock<IArgumentParser<MyOptions>> CreateParserMock(ParserType parserType, bool multiValued,
            Action<ParseContext> parseAction,
            ContextResult result)
        {
            var mock = new Mock<IArgumentParser<MyOptions>>();
            mock.SetupGet(m => m.MultiValued).Returns(multiValued);
            mock.SetupGet(m => m.ParserType).Returns(parserType);
            mock.Setup(m => m.ProcessContext(It.IsAny<MyOptions>(), It.IsAny<ParseContext>()))
                .Returns<MyOptions, ParseContext>((_, ctx) =>
                {
                    parseAction(ctx);
                    return result;
                })
                .Verifiable();
            
            return mock;
        }
    }
}