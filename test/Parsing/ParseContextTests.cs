// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Vertical.CommandLine.Parsing;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

// ReSharper disable MemberCanBePrivate.Global

namespace Vertical.CommandLine.Tests.Parsing
{
    public class ParseContextTests
    {
        [Fact]
        public void ReadyReturnsTrueAfterCreate()
        {
            new ParseContext(new[]{"arg"}).Ready.ShouldBeTrue();
        }

        [Fact]
        public void ReadyReturnsFalseWhenEmpty()
        {
            var context = new ParseContext(Enumerable.Empty<string>());
            context.Ready.ShouldBeFalse();
        }

        [Fact]
        public void ReadyReturnsTrueWhenNotEmpty()
        {
            var context = new ParseContext(new[]{"arg", "arg"});
            context.TryTakeStringValue(out _);
            context.Ready.ShouldBeTrue();
        }

        [Theory, MemberData(nameof(TryTakeStringValueTheories))]
        public void TryTakeStringValueReturnsExpected(string[] args, bool expected, Token token)
        {
            var context = new ParseContext(args);
            context.TryTakeStringValue(out var returnToken).ShouldBe(expected);
            if (expected) returnToken.ShouldBe(token);
        }

        public static IEnumerable<object[]> TryTakeStringValueTheories => Scenarios
        (
            Scenario(Array.Empty<string>(), false, Token.Empty),
            Scenario(new[]{"test"}, true, new Token(TokenType.NonTemplateValue, "test")),
            Scenario(new[]{"test1", "test2"}, true, new Token(TokenType.NonTemplateValue,"test1")),
            Scenario(new[]{"--help", "test"}, false, Token.Empty),
            Scenario(new[]{"-h", "test"}, false, Token.Empty)
        );

        [Theory, MemberData(nameof(TryTakeTemplateTheories))]
        public void TryTakeTemplateReturnsExpected(string[] args, bool expected)
        {
            var context = new ParseContext(args);
            context.TryTakeTemplate(Template.ForOptionOrSwitch("-h|--help")).ShouldBe(expected);
        }

        public static IEnumerable<object[]> TryTakeTemplateTheories => Scenarios
        (
            Scenario(Array.Empty<string>(), false),
            Scenario(new []{"-h"}, true),
            Scenario(new []{"--help"}, true),
            Scenario(new []{"arg"}, false),
            Scenario(new []{"-n"}, false),
            Scenario(new []{"--no"}, false)
        );

        [Fact]
        public void TryTakeTemplateWithIndexReturnsTrueForIndexMatch()
        {
            var context = new ParseContext(new[]{"-h"});
            context.TryTakeTemplate(Template.ForOptionOrSwitch("-h"), 0).ShouldBeTrue();
        }

        [Fact]
        public void TryTakeTemplateWithIndexReturnsFalseForNonIndexMatch()
        {
            var context = new ParseContext(new[]{"arg", "-h"});
            context.TryTakeStringValue(out _).ShouldBeTrue();
            context.TryTakeTemplate(Template.ForOptionOrSwitch("-h"), 0).ShouldBeFalse();
        }

        [Fact]
        public void TakeConsumesToken()
        {
            var context = new ParseContext(new[]{"arg1","arg2"});
            context.TryTakeStringValue(out _).ShouldBeTrue();
            context.Reset().ShouldBeTrue();
            context.Count.ShouldBe(1);
            context.Ready.ShouldBeTrue();
            context.TryTakeStringValue(out var token).ShouldBeTrue();
            token.ShouldBe(new Token(TokenType.NonTemplateValue, "arg2"));
        }

        [Fact]
        public void TakeSkipsForNoMatch()
        {
            var context = new ParseContext(new[]{"arg","--help"});
            context.TryTakeTemplate(Template.ForOptionOrSwitch("--help")).ShouldBeFalse();
            context.TryTakeTemplate(Template.ForOptionOrSwitch("--help")).ShouldBeTrue();
        }

        [Fact]
        public void ResetRestartsTokenOrder()
        {
            var context = new ParseContext(new[]{"arg","--help"});
            while (context.Ready)
                context.TryTakeTemplate(Template.ForOptionOrSwitch("--no-match"));
            context.Reset();
            context.TryTakeStringValue(out var token).ShouldBeTrue();
            token.ShouldBe(new Token(TokenType.NonTemplateValue, "arg"));
        }

        [Theory, MemberData(nameof(ParseArgumentsTheories))]
        public void ParseArgumentsReturnsExpected(string[] args, Token[] expected)
        {
            var parseContext = new ParseContext(args);
            var tokens = parseContext.ToArray();

            tokens.ShouldBe(expected);
        }

        public static IEnumerable<object[]> ParseArgumentsTheories => Scenarios(
            Scenario(
                new[] { "-t", "--test" },
                new[] { new Token(TokenType.ShortOption, "t"), new Token(TokenType.LongOption, "test") }
            ),
            Scenario(
                new[] { "--", "-t", "--test", "test" },
                new[] { new Token(TokenType.NonTemplateValue, "-t"), new Token(TokenType.NonTemplateValue, "--test"), new Token(TokenType.NonTemplateValue, "test") }
            ),
            Scenario(
                new[] { "-t", "--", "-file" },
                new[] { new Token(TokenType.ShortOption, "t"), new Token(TokenType.NonTemplateValue, "-file") }
            ),
            Scenario(
                new[] { "-abc" },
                new[] { new Token(TokenType.ShortOption, "a"), new Token(TokenType.ShortOption, "b"), new Token(TokenType.ShortOption, "c") }
            )
        );

        [Fact]
        public void NonGenericGetEnumeratorReturnsTokens()
        {
            var context = new ParseContext(new []{"test"});
            ((IEnumerable) context).Cast<Token>().Single()
                .ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
        }
    }
}