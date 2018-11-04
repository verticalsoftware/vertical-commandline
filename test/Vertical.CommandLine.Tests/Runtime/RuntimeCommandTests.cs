// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Runtime;
using System.Linq;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Provider;
using System.Threading.Tasks;

namespace Vertical.CommandLine.Tests.Runtime
{
    public class RuntimeCommandTests
    {
        [Fact]
        public void ConstructAssignsTemplate()
        {
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null);
            command.Template.Tokens.Single().ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
        }

        [Fact]
        public void GetOptionsReturnsConstructorProvider()
        {
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null);
            command.GetOptions().ShouldNotBeNull();
        }

        [Fact]
        public void GetOptionsReturnsValueFromSunkProvider()
        {
            var options = new object();
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null)
            {
                OptionsProvider = new InstanceProvider<object>(options)
            };
            command.GetOptions().ShouldBe(options);
        }

        [Fact]
        public void GetOptionsThrowsWhenNullReturned()
        {
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null)
            {
                OptionsProvider = new Mock<IProvider<object>>().Object
            };
            Should.Throw<ConfigurationException>(() => command.GetOptions());
        }

        [Fact]
        public void GetOptionsThrowsWhenGetInstanceThrows()
        {
            var providerMock = new Mock<IProvider<object>>();
            providerMock.Setup(m => m.GetInstance()).Throws(new Exception());
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null)
            {
                OptionsProvider = providerMock.Object
            };
            Should.Throw<ConfigurationException>(() => command.GetOptions());
        }

        [Fact]
        public void InvokeCallsClientHandler()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(_ => invoked = true);
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null)
            {
                ClientHandler = handler
            };
            command.Invoke(new object());
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task InvokeAsyncCallsClientHandler()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(_ =>
            {
                invoked = true;
                return Task.CompletedTask;
            });
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null)
            {
                ClientHandler = handler
            };
            await command.InvokeAsync(new object());
            invoked.ShouldBeTrue();
        }

        [Fact]
        public void InvokeThrowsWhenNullHandler()
        {
            Should.Throw<ConfigurationException>(() => new RuntimeCommand<object>(Template.ForCommand("test"),
                null).Invoke(new object()));
        }

        [Fact]
        public void InvokeAsyncThrowsWhenNullHandler()
        {
            Should.ThrowAsync<ConfigurationException>(async () => await new RuntimeCommand<object>(Template.ForCommand("test"),
                null).InvokeAsync(new object()));
        }

        [Fact]
        public void HelpContentProviderReturnsSunkInstance()
        {
            var provider = new Mock<IProvider<IReadOnlyCollection<string>>>();
            var command = new RuntimeCommand<object>(Template.ForCommand("test"), null);
            ((IComponentSink<IProvider<IReadOnlyCollection<string>>>)command).Sink(provider.Object);
            command.HelpContentProvider.ShouldBe(provider.Object);
        }
    }
}
