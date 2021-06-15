// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using Moq;
using System.Linq;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ParserConfigurationTests
    {
        private readonly ParserConfiguration<object> _instanceUnderTest = new ParserConfiguration<object>();

        [Fact]
        public void AddTemplateThrowsForDuplicate()
        {
            _instanceUnderTest.AddTemplate(Template.ForOptionOrSwitch("--test"));
            Should.Throw<ConfigurationException>(() => _instanceUnderTest.AddTemplate(Template.ForOptionOrSwitch("--test")));
        }

        [Fact]
        public void AddParserPutsToCollection()
        {
            var parser = new Mock<IArgumentParser<object>>().Object;
            _instanceUnderTest.AddParser(parser);
            _instanceUnderTest.ArgumentParsers.Single().ShouldBeSameAs(parser);
        }

        [Fact]
        public void ConfigureParserPutsToCollection()
        {
            var parser = new Mock<IArgumentParser<object>>().Object;
            _instanceUnderTest.ConfigureParser<string>("test", _ => parser);
            _instanceUnderTest.ArgumentParsers.Single().ShouldBeSameAs(parser);
        }

        [Fact]
        public void ConfigureParserThrowsForDuplicateTemplate()
        {
            var parser = new Mock<IArgumentParser<object>>().Object;
            _instanceUnderTest.ConfigureParser<string>(Template.ForCommand("test"), _ => parser);
            Should.Throw<ConfigurationException>(() => _instanceUnderTest.ConfigureParser<string>(Template.ForCommand("test"), _ => parser));
        }
    }
}
