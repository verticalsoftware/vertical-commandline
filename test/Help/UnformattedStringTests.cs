// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Linq;
using Shouldly;
using Vertical.CommandLine.Help;
using Xunit;

namespace Vertical.CommandLine.Tests.Help
{
    public class UnformattedStringTests
    {
        private const string Value = "unformatted";

        private readonly IFormattedString _testInstance = new UnformattedString(Value);

        [Fact]
        public void SplitToWidthReturnsFullSpan()
        {
            _testInstance.SplitToWidth(0).Single().ShouldBe(new Span(0, Value.Length));
        }

        [Fact]
        public void IndentShouldBeZero() => _testInstance.Indent.ShouldBe(0);

        [Fact]
        public void StartIndexShouldBeZero() => _testInstance.StartIndex.ShouldBe(0);

        [Fact]
        public void SourceShouldBeValue() => _testInstance.Source.ShouldBe(Value);
    }
}