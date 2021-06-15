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
    public class HelpOptionParserTests
    {
        private static readonly Template Template = Template.ForOptionOrSwitch("-h|--help");
        private readonly IArgumentParser<object> _instanceUnderTest = new HelpOptionParser<object>(Template);

        [Theory, MemberData(nameof(Theories))]
        public void ProcessContextReturnsExpected(string arg, ContextResult expected)
        {
            var context = new ParseContext(new[]{arg});
            var result = _instanceUnderTest.ProcessContext(null, context);
            
            result.ShouldBe(expected);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> Theories => Scenarios
        (
            Scenario("help", ContextResult.NoMatch),
            Scenario("file.txt", ContextResult.NoMatch),
            Scenario("-h", ContextResult.Help),
            Scenario( "--help", ContextResult.Help)
        );

        [Fact] 
        public void ParserTypeReturnsHelp() => _instanceUnderTest.ParserType.ShouldBe(ParserType.Help);

        [Fact]
        public void ParserMultiValuedReturnsFalse() => _instanceUnderTest.MultiValued.ShouldBe(false);
    }
}