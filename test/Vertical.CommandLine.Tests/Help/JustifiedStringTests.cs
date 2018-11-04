// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Vertical.CommandLine.Help;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

namespace Vertical.CommandLine.Tests.Help
{
    public class JustifiedStringTests
    {
        [Fact]
        public void ConstructWithNoIndentSetsProperties()
        {
            var js = new JustifiedString("test");
            js.Indent.ShouldBe(0);
            js.StartIndex.ShouldBe(0);
        }

        [Fact]
        public void ConstructWithSpaceIndentSetsProperties()
        {
            var js = new JustifiedString("  test");
            js.Indent.ShouldBe(2);
            js.StartIndex.ShouldBe(2);
        }

        [Theory, MemberData(nameof(Theories))]
        public void WriteContentSplitsAsExpected(string source, int width, IEnumerable<Span> expectedSpans)
        {
            var js = new JustifiedString(source);
            var spans = js.SplitToWidth(width).ToArray();
            
            spans.ShouldBe(expectedSpans);
        }

        [Fact]
        public void ConstructWithNoContentReturnsEmpty()
        {
            var empty = new JustifiedString(null);
            empty.Source.ShouldBeNull();
            empty.StartIndex.ShouldBe(0);
            empty.Indent.ShouldBe(0);
        }

        public static IEnumerable<object[]> Theories => Scenarios
            (
                MyScenario(string.Empty, int.MaxValue, Span.Empty),
                MyScenario("test", int.MaxValue, new Span(0, 4)),
                MyScenario("    test", int.MaxValue, new Span(4, 4)),
                MyScenario("\ttest", int.MaxValue, new Span(1, 4)),
                MyScenario("split word", 5, new Span(0, 5), new Span(6, 4)),
                MyScenario("    split word", 5, new Span(4, 5), new Span(10, 4)),
                MyScenario("\tsplit word", 5, new Span(1, 5), new Span(7, 4)),
                MyScenario("icantbespltbecauseimtoolong", 10, new Span(0, 27)),
                MyScenario("    icantbespltbecauseimtoolong", 10, new Span(4, 27))
            );

        internal static object[] MyScenario(string source, int width, params Span[] spans) => new object[] {source, width, spans};
    }
}
