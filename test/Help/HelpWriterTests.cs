// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Shouldly;
using Vertical.CommandLine.Help;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

namespace Vertical.CommandLine.Tests.Help
{
    public class HelpWriterTests
    {
        private const string LoremIpsum = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.";
        
        [Fact]
        public void ConstructWithNullFormatInfoThrows()
        {
            Should.Throw<ArgumentNullException>(() => new HelpWriter(new Mock<TextWriter>().Object, null));
        }

        [Fact]
        public void ConstructWithNullWriterThrows()
        {
            Should.Throw<ArgumentNullException>(() => new HelpWriter(null, FormatInfo.Default));
        }

        [Theory, MemberData(nameof(Theories))]
        public void WriteContentReturnsExpected(string[] source, string[] expected, FormatInfo formatInfo, int expectedVirtualCount)
        {
            using (var stringWriter = new StringWriter())
            {
                var virtualCount = HelpWriter.WriteContent(stringWriter, source, formatInfo);
                expectedVirtualCount.ShouldBe(expectedVirtualCount);
                var result = stringWriter.ToString().Split(Environment.NewLine);
                result.ShouldBe(expected);
            }
        }

        [Fact]
        public void WriteContextPopulatesTextWriter()
        {
            using (var stringWriter = new StringWriter())
            {
                var helpWriter = new HelpWriter(stringWriter, FormatInfo.Default);
                helpWriter.WriteContent(new[] {"test"});
                stringWriter.ToString().ShouldBe("test");
            }
        }

        public static IEnumerable<object[]> Theories => Scenarios
        (
            // extreme happy path
            Scenario(
                Generate(0, 1),
                Generate(0, 1),
                FormatInfo.Default,
                1
            ),
            // constrained start row > 0
            Scenario(
                Generate(0, 2),
                Generate(1, 1),
                new FormatInfo(int.MaxValue, int.MaxValue, 1),
                2
            ),
            // constrained height
            Scenario(
                Generate(0, 5),
                Generate(0, 2),
                new FormatInfo(int.MaxValue, 2, 0),
                5
            ),
            // constrained height with > 0 start row
            Scenario(
                Generate(0, 5),
                Generate(1, 2),
                new FormatInfo(int.MaxValue, 2, 1),
                5
            ),
            // Constrained width
            Scenario(
                Generate(0, 1),
                new[]
                {
                    "line 0: Lorem Ipsum is simply dummy text",
                    "of the printing and typesetting industry."
                },
                new FormatInfo(40, int.MaxValue, 0),
                2
            ),
            // constrained width with indent
            Scenario(
                new[]{$"    {LoremIpsum}"},
                new[]
                {
                    "    Lorem Ipsum is simply dummy",
                    "    text of the printing and",
                    "    typesetting industry."
                },
                new FormatInfo(32, int.MaxValue, 0),
                3
            )
        );
        
        private static string[] Generate(int start, int count) => Enumerable.Range(start, count).Select(i => $"line {i}: {LoremIpsum}").ToArray();        
    }
}
