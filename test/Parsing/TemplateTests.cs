// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using System;
using System.Linq;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class TemplateTests
    {
        [Fact]
        public void ImplicitOperatorCreatesInstance()
        {
            (Template.ForCommand("test")).Tokens.Single().ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
        }

        [Fact]
        public void ForCommandWithNullOrEmptyStringThrows()
        {
            Should.Throw<ArgumentException>(() => Template.ForCommand(null));
            Should.Throw<ArgumentException>(() => Template.ForCommand(string.Empty));
            Should.Throw<ArgumentException>(() => Template.ForCommand(" "));
        }

        [Theory]
        [InlineData("-a")] // Reject short form
        [InlineData("-abc")] // Reject compact short form
        [InlineData("--long")] // Reject long form
        [InlineData("$arg")] // Reject non-word
        [InlineData("--")] // Reject options termination
        public void ForCommandRejectsInvalidTemplate(string template)
        {
            Should.Throw<ConfigurationException>(() => Template.ForCommand(template));
        }

        [Fact]
        public void ForOptionOrSwitchWithNullOrEmptyStringThrows()
        {
            Should.Throw<ArgumentException>(() => Template.ForOptionOrSwitch(null));
            Should.Throw<ArgumentException>(() => Template.ForOptionOrSwitch(string.Empty));
            Should.Throw<ArgumentException>(() => Template.ForOptionOrSwitch(" "));
        }

        [Theory]
        [InlineData("word")] // Reject word
        [InlineData("-$")] // Reject symbols
        [InlineData("--$")]
        [InlineData("$arg")] // Reject symbols
        [InlineData("arg-arg")]
        // Reject non word
        public void ForOptionOrSwitchRejectsInvalidTemplate(string template)
        {
            Should.Throw<ConfigurationException>(() => Template.ForOptionOrSwitch(template));
        }

        [Fact]
        public void ConstructWithDoubleDashThrows()
        {
            Should.Throw<ConfigurationException>(() => Template.ForCommand("--"));
            Should.Throw<ConfigurationException>(() => Template.ForCommand("-h|--help|--"));
            Should.Throw<ConfigurationException>(() => Template.ForOptionOrSwitch("--"));
            Should.Throw<ConfigurationException>(() => Template.ForOptionOrSwitch("-h|--help|--"));
        }

        [Fact]
        public void ContainsReturnsTrueOnAnyMatch()
        {
            Template.ForOptionOrSwitch("-t").Contains(new Token(TokenType.ShortOption, "t")).ShouldBeTrue();
        }

        [Fact]
        public void BuildTokensSplitsString()
        {
            var template = Template.ForOptionOrSwitch("-t|--test");
            template.Tokens.ShouldContain(t => t.Equals(new Token(TokenType.ShortOption, "t")), 1);
            template.Tokens.ShouldContain(t => t.Equals(new Token(TokenType.LongOption, "test")), 1);
        }

        [Fact]
        public void BuildTokensRejectsCompactMultiShortOption()
        {
            Should.Throw<ConfigurationException>(() => Template.ForOptionOrSwitch("-abc"));
        }
    }
}
