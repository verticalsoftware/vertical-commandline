// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using Shouldly;
using Vertical.CommandLine.Parsing;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class TokenMatcherTests
    {
        [Theory, MemberData(nameof(PositiveTheories))]
        public void PositiveMatchReturnsTokens(TokenMatcher matcher, string input, Token[] expected)
        {
            matcher.GetTokens(input).ShouldBe(expected);
        }

        public static IEnumerable<object[]> PositiveTheories => Scenarios
        (
            Scenario(TokenMatcher.ShortOption, "-a", new[]{ new Token(TokenType.ShortOption, "a") }),
            Scenario(TokenMatcher.ShortOption, "-1", new[]{ new Token(TokenType.ShortOption, "1") }),
            Scenario(TokenMatcher.OptionsEnd, "--", new[]{ Token.OptionsEnd }),
            Scenario(TokenMatcher.CompactShortOption, "-a", new[]{ new Token(TokenType.ShortOption, "a" )}),
            Scenario(TokenMatcher.CompactShortOption, "-1", new[]{ new Token(TokenType.ShortOption, "1" )}),
            Scenario(TokenMatcher.CompactShortOption, "-ab", new[]{ new Token(TokenType.ShortOption, "a"), new Token(TokenType.ShortOption, "b") }),
            Scenario(TokenMatcher.CompactShortOption, "-12", new[]{ new Token(TokenType.ShortOption, "1"), new Token(TokenType.ShortOption, "2") }),
            Scenario(TokenMatcher.LongOption, "--long", new[]{ new Token(TokenType.LongOption, "long") }),
            Scenario(TokenMatcher.LongOption, "--l0ng", new[]{ new Token(TokenType.LongOption, "l0ng") }),
            Scenario(TokenMatcher.Word, "word", new[]{ new Token(TokenType.NonTemplateValue, "word") }),
            Scenario(TokenMatcher.Word, "w0rd", new[]{ new Token(TokenType.NonTemplateValue, "w0rd") }),
            Scenario(TokenMatcher.Any, "$", new[]{ new Token(TokenType.NonTemplateValue, "$")}),
            Scenario(TokenMatcher.CompositeOption, "-a:", new[]{ new Token(TokenType.CompositeOption, "a")}),
            Scenario(TokenMatcher.CompositeOption, "-a=", new[]{ new Token(TokenType.CompositeOption, "a")}),
            Scenario(TokenMatcher.CompositeOption, "--long=", new[]{ new Token(TokenType.CompositeOption, "long")}),
            Scenario(TokenMatcher.CompositeOption, "--long:", new[]{ new Token(TokenType.CompositeOption, "long")}),
            Scenario(TokenMatcher.CompositeOption, "--long-with-dashes:", new[]{ new Token(TokenType.CompositeOption, "long-with-dashes")}),
            Scenario(TokenMatcher.CompositeOption, "--long-with-dashes=", new[]{ new Token(TokenType.CompositeOption, "long-with-dashes")})
        );

        [Theory, MemberData(nameof(NegativeTheories))]
        public void NegativeMatchReturnsNull(TokenMatcher matcher, string input)
        {
            matcher.GetTokens(input).ShouldBeEmpty();
        }

        public static IEnumerable<object[]> NegativeTheories => Scenarios
        (
            // Impractical to test every negative non matching pattern, but the goal here is to
            // make sure the matchers do not intersect
            Scenario(TokenMatcher.ShortOption, "-ab"),
            Scenario(TokenMatcher.ShortOption, "--long"),
            Scenario(TokenMatcher.ShortOption, "-12"),
            Scenario(TokenMatcher.ShortOption, "word"),
            Scenario(TokenMatcher.ShortOption, "$any"),
            Scenario(TokenMatcher.ShortOption, "--"),
            Scenario(TokenMatcher.OptionsEnd, "-a"),
            Scenario(TokenMatcher.OptionsEnd, "-1"),
            Scenario(TokenMatcher.OptionsEnd, "-ab"),
            Scenario(TokenMatcher.OptionsEnd, "-12"),
            Scenario(TokenMatcher.OptionsEnd, "--long"),
            Scenario(TokenMatcher.OptionsEnd, "word"),
            Scenario(TokenMatcher.OptionsEnd, "$any"),
            Scenario(TokenMatcher.CompactShortOption, "--long"),
            Scenario(TokenMatcher.CompactShortOption, "word"),
            Scenario(TokenMatcher.CompactShortOption, "$any"),
            Scenario(TokenMatcher.CompactShortOption, "--"),
            Scenario(TokenMatcher.LongOption, "--"),
            Scenario(TokenMatcher.LongOption, "-a"),
            Scenario(TokenMatcher.LongOption, "-ab"),
            Scenario(TokenMatcher.LongOption, "-1"),
            Scenario(TokenMatcher.LongOption, "-12"),
            Scenario(TokenMatcher.LongOption, "word"),
            Scenario(TokenMatcher.LongOption, "$any"),
            Scenario(TokenMatcher.Word, "--"),
            Scenario(TokenMatcher.Word, "-a"),
            Scenario(TokenMatcher.Word, "-ab"),
            Scenario(TokenMatcher.Word, "-1"),
            Scenario(TokenMatcher.Word, "-12"),
            Scenario(TokenMatcher.Word, "--long"),
            Scenario(TokenMatcher.Word, "$any"),
            Scenario(TokenMatcher.CompositeOption, "-a"),
            Scenario(TokenMatcher.CompositeOption, "-ab"),
            Scenario(TokenMatcher.CompositeOption, "--long"),
            Scenario(TokenMatcher.CompositeOption, "$any"),
            Scenario(TokenMatcher.CompositeOption, "--"),
            Scenario(TokenMatcher.CompositeOption, "-1"),
            Scenario(TokenMatcher.CompositeOption, "-12")
        );
    }
}