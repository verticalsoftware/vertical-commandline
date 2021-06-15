// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;
using Moq;
using Xunit;
using Shouldly;
using Vertical.CommandLine.Parsing;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class TokenParserTests
    {
        private const string TestValue = "test";
        private static readonly Token MatchedToken = new Token(TokenType.NonTemplateValue, TestValue);
        private readonly Mock<ITokenMatcher> PositiveMatcher = new Mock<ITokenMatcher>();
        private readonly Mock<ITokenMatcher> NegativeMatcher = new Mock<ITokenMatcher>();

        public TokenParserTests()
        {
            PositiveMatcher.Setup(m => m.GetTokens(TestValue))
                .Returns(new[] {MatchedToken})
                .Verifiable();
            NegativeMatcher.Setup(m => m.GetTokens(TestValue))
                .Returns(Array.Empty<Token>())
                .Verifiable();
        }

        [Fact]
        public void ParseInvokesAllMatchers()
        {
            var parser = new TokenParser(NegativeMatcher.Object, NegativeMatcher.Object);
            Should.Throw<ArgumentException>(() => parser.Parse(TestValue));
            NegativeMatcher.Verify(m => m.GetTokens(TestValue), Times.Exactly(2));
        }

        [Fact]
        public void ParseReturnsFirstMatchedTokens()
        {
            var parser = new TokenParser(PositiveMatcher.Object, NegativeMatcher.Object);
            parser.Parse(TestValue).ShouldBe(new []{MatchedToken});
        }

        [Fact]
        public void ParseInvokesPositiveMatcherOnlyWhenFirst()
        {
            var parser = new TokenParser(PositiveMatcher.Object, NegativeMatcher.Object);
            parser.Parse(TestValue);
            PositiveMatcher.Verify(m => m.GetTokens(TestValue), Times.Once);
            NegativeMatcher.Verify(m => m.GetTokens(TestValue), Times.Never);
        }

        [Fact]
        public void ParseInvokesAllMatchersIfMatchIsLast()
        {
            var parser = new TokenParser(PositiveMatcher.Object, NegativeMatcher.Object);
            parser.Parse(TestValue);
            PositiveMatcher.Verify(m => m.GetTokens(TestValue), Times.Once);
            NegativeMatcher.Verify(m => m.GetTokens(TestValue), Times.Never);
        }

        [Fact]
        public void ParseNoMatchThrows()
        {
            var parser = new TokenParser(PositiveMatcher.Object, NegativeMatcher.Object);
            Should.Throw<ArgumentException>(() => parser.Parse("no-match"));
        }

        [Fact]
        public void ParseWithEmptyStringReturnsEmptyEnumerable()
        {
            // Verifies https://github.com/verticalsoftware/vertical-commandline/issues/18
            var parser = new TokenParser(PositiveMatcher.Object, NegativeMatcher.Object);
            parser.Parse("").ShouldBe(Enumerable.Empty<Token>());
        }
    }
}
