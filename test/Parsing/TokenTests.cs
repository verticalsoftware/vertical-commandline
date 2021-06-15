// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class TokenTests
    {
        private const TokenType Type = TokenType.ShortOption;
        private const string Value = "t";
        private readonly Token _instanceUnderTest = new Token(Type, Value);

        [Fact]
        public void ConstructAssignsProperties()
        {
            _instanceUnderTest.Type.ShouldBe(Type);
            _instanceUnderTest.Value.ShouldBe(Value);
        }

        [Theory]
        [InlineData(TokenType.LongOption)]
        [InlineData(TokenType.ShortOption)]
        [InlineData(TokenType.NonTemplateValue)]
        public void ConstructWithNullValueThrows(TokenType type)
        {
            Should.Throw<ArgumentException>(() => new Token(type, null));
        }

        [Theory]
        [InlineData(Type, Value, true)]
        [InlineData(TokenType.LongOption, Value, false)]
        [InlineData(Type, "a", false)]
        [InlineData(TokenType.LongOption, "a", false)]
        public void EqualsReturnsFalseForMismatchProperties(TokenType type, string value, bool expected)
        {
            new Token(type, value).Equals(_instanceUnderTest).ShouldBe(expected);
        }

        [Fact]
        public void ObjectEqualsReturnsFalseForNull()
        {
            Token.Empty.Equals(null).ShouldBeFalse();
        }
    }
}
