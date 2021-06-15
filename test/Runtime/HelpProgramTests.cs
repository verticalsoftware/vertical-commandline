// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;
using Vertical.CommandLine.Provider;
using Vertical.CommandLine.Runtime;
using Xunit;

namespace Vertical.CommandLine.Tests.Runtime
{
    public class HelpProgramTests
    {
        [Fact]
        public void ConstructWithNullProviderThrows()
        {
            Should.Throw<ConfigurationException>(() => new HelpProgram(
                new Mock<IHelpWriter>().Object,
                null));
        }

        [Fact]
        public Task InvokeCallsWriter()
        {
            const string value = "test";

            var helpWriterMock = new Mock<IHelpWriter>();
            helpWriterMock.Setup(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>()))
                .Callback<IReadOnlyCollection<string>>(content =>
                    content.Single().ShouldBe(value));

            var providerMock = new Mock<IProvider<IReadOnlyCollection<string>>>();
            providerMock.Setup(m => m.GetInstance()).Returns(new[] {value});

            var helpProgram = new HelpProgram(helpWriterMock.Object, providerMock.Object);

            helpProgram.Invoke(null);
            return helpProgram.InvokeAsync(null);
        }

        [Fact]
        public void InvokeWithNullProviderReturnThrows()
        {
            var helpProgram = new HelpProgram(
                new Mock<IHelpWriter>().Object,
                new Mock<IProvider<IReadOnlyCollection<string>>>().Object);

            Should.Throw<ConfigurationException>(() => helpProgram.Invoke(null));
        }

        [Fact]
        public void InvokeWhenProviderThrowsThrows()
        {
            var providerMock = new Mock<IProvider<IReadOnlyCollection<string>>>();
            providerMock.Setup(m => m.GetInstance()).Throws<Exception>();

            var helpProgram = new HelpProgram(
                new Mock<IHelpWriter>().Object,
                providerMock.Object);

            Should.Throw<ConfigurationException>(() => helpProgram.Invoke(null));
        }
    }
}