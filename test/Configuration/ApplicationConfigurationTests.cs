// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Linq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ApplicationConfigurationTests
    {
        private readonly ApplicationConfiguration<object> _instanceUnderTest =
            new ApplicationConfiguration<object>();

        [Fact]
        public void CommandCreatesSubConfiguration()
        {
            _instanceUnderTest.Command("test", _ => { });
            _instanceUnderTest.SubConfigurations.Single().ShouldNotBeNull();
        }

        [Fact]
        public void InvalidConfigurationThrows()
        {
            Should.Throw<ConfigurationException>(() => _instanceUnderTest.Command("-i", _ => { }));
        }

        [Fact]
        public void HelpRegisters()
        {
            _instanceUnderTest.HelpOption("--help", ConsoleHelpWriter.Default);
            _instanceUnderTest.RuntimeCommand.MapArguments(new ParseContext(new[] { "--help" }),
                new object(),
                ParserType.Help)
                .ShouldBe(ContextResult.Help);
        }
    }
}
