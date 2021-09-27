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
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Runtime;
using Vertical.CommandLine.Parsing;
using System.Linq;
using System.Threading;
using Vertical.CommandLine.Provider;
using System.Threading.Tasks;

namespace Vertical.CommandLine.Tests.Runtime
{
    public class AggregatedRuntimeCommandTests
    {
        [Fact]
        public void ConstructFindsCommand()
        {
            var cmd = MockSubConfiguration("test");
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.SelectedRuntime.ShouldBe(cmd.GetRuntimeCommand());
        }

        [Fact]
        public void ConstructIgnoresCommandForNonMatchingTemplate()
        {
            var cmd = MockSubConfiguration("ignore");
            var appRuntime = new Mock<IRuntimeCommand>();
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                appRuntime.Object,
                new[] { cmd });
            testInstance.SelectedRuntime.ShouldBeSameAs(appRuntime.Object);
        }

        [Fact]
        public void ConstructAssignsAppCommandAsDefault()
        {
            var appRuntime = new Mock<IRuntimeCommand>();
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(Array.Empty<string>()),
                appRuntime.Object,
                Enumerable.Empty<ICommandLineConfiguration>());
            testInstance.SelectedRuntime.ShouldBeSameAs(appRuntime.Object);
        }

        [Fact]
        public void TemplateReturnsCommandTemplate()
        {
            var cmd = MockSubConfiguration("test");
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.SelectedRuntime.Template.Tokens.Single().ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
            testInstance.Template.Tokens.Single().ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
        }

        [Fact]
        public void GetOptionsReturnsCommandInstance()
        {
            var options = new object();
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
                m.Setup(c => c.GetOptions()).Returns(options));
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.GetOptions().ShouldBe(options);
        }

        [Fact]
        public void GetOptionsReturnsAppInstance()
        {
            var appOptions = new object();
            var cmdOptions = new object();
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.GetOptions()).Returns(appOptions);
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
                m.Setup(c => c.GetOptions()).Returns(cmdOptions));
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "app" }),
                appMock.Object,
                new[] { cmd });
            testInstance.GetOptions().ShouldBe(appOptions);
        }

        [Fact]
        public void GetHelpContentProviderReturnsCommandInstance()
        {
            var provider = new Mock<IProvider<IReadOnlyCollection<string>>>().Object;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
                m.Setup(c => c.HelpContentProvider).Returns(provider));
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.HelpContentProvider.ShouldBe(provider);
        }

        [Fact]
        public void GetHelpContentProviderReturnsAppInstance()
        {
            var cmdProvider = new Mock<IProvider<IReadOnlyCollection<string>>>().Object;
            var appProvider = new Mock<IProvider<IReadOnlyCollection<string>>>().Object;
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.HelpContentProvider).Returns(appProvider);
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
                m.Setup(c => c.HelpContentProvider).Returns(cmdProvider));
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "app" }),
                appMock.Object,
                new[] { cmd });
            testInstance.HelpContentProvider.ShouldBe(appProvider);
        }

        [Fact]
        public void InvokeCallsCommand()
        {
            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
                {
                    cmdMock = m;
                    m.Setup(c => c.Invoke(It.IsAny<object>())).Verifiable();
                });
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.Invoke(new object());
            cmdMock.Verify(c => c.Invoke(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void InvokeCallsAppCommand()
        {
            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.Invoke(It.IsAny<object>())).Verifiable();
            });
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.Invoke(It.IsAny<object>())).Verifiable();
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "app" }),
                appMock.Object,
                new[] { cmd });
            testInstance.Invoke(new object());
            cmdMock.Verify(c => c.Invoke(It.IsAny<object>()), Times.Never);
            appMock.Verify(c => c.Invoke(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsyncCallsCommand()
        {
            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            });
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "test" }),
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            await testInstance.InvokeAsync(new object(), CancellationToken.None);
            cmdMock.Verify(c => c.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsyncCallsApp()
        {
            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            });
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "app" }),
                appMock.Object,
                new[] { cmd });
            await testInstance.InvokeAsync(new object(), CancellationToken.None);
            cmdMock.Verify(c => c.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
            appMock.Verify(c => c.InvokeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsyncCallsAppWithGivenCancellationToken()
        {
            using var cancelTokenSource = new CancellationTokenSource();
            var cancellationToken = cancelTokenSource.Token;
            Mock<IRuntimeCommand> cmdMock = null;
            
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.InvokeAsync(It.IsAny<object>(), cancellationToken))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            });
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.InvokeAsync(It.IsAny<object>(), cancellationToken)).Returns(Task.CompletedTask).Verifiable();
            var testInstance = new AggregatedRuntimeCommand(new ParseContext(new[] { "app" }),
                appMock.Object,
                new[] { cmd });
            await testInstance.InvokeAsync(new object(), cancellationToken);
            cmdMock.Verify(c => c.InvokeAsync(It.IsAny<object>(), cancellationToken), Times.Never);
            appMock.Verify(c => c.InvokeAsync(It.IsAny<object>(), cancellationToken), Times.Once);
        }

        [Fact]
        public void MapArgumentsCallsCommand()
        {
            var parseContext = new ParseContext(new[] { "test", "arg" });
            var parserType = ParserType.Option;
            var options = new object();

            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.MapArguments(parseContext, options, parserType)).Verifiable();
            });
            var testInstance = new AggregatedRuntimeCommand(parseContext,
                new Mock<IRuntimeCommand>().Object,
                new[] { cmd });
            testInstance.MapArguments(parseContext, options, parserType);
            cmdMock.Verify(c => c.MapArguments(parseContext, options, parserType), Times.Once);
        }

        [Fact]
        public void MapArgumentsCallsApp()
        {
            var parseContext = new ParseContext(new[] { "app", "arg" });
            var parserType = ParserType.Option;
            var options = new object();

            Mock<IRuntimeCommand> cmdMock = null;
            var cmd = MockSubConfiguration("test", runtimeCmdConfig: m =>
            {
                cmdMock = m;
                m.Setup(c => c.MapArguments(parseContext, options, parserType)).Verifiable();
            });
            var appMock = new Mock<IRuntimeCommand>();
            appMock.Setup(m => m.MapArguments(parseContext, options, parserType));
            var testInstance = new AggregatedRuntimeCommand(parseContext,
                appMock.Object,
                new[] { cmd });
            testInstance.MapArguments(parseContext, options, parserType);
            cmdMock.Verify(c => c.MapArguments(parseContext, options, parserType), Times.Never);
            appMock.Verify(c => c.MapArguments(parseContext, options, parserType), Times.Once);
        }

        private static ICommandLineConfiguration MockSubConfiguration(string template,
            Action<Mock<ICommandLineConfiguration>> setupConfig = null,
            Action<Mock<IRuntimeCommand>> runtimeCmdConfig = null)
        {
            var mock = new Mock<ICommandLineConfiguration>();
            var runtimeCmdMock = new Mock<IRuntimeCommand>();
            runtimeCmdMock.Setup(m => m.Template).Returns(Template.ForCommand(template));
            mock.Setup(m => m.GetRuntimeCommand()).Returns(runtimeCmdMock.Object);
            setupConfig?.Invoke(mock);
            runtimeCmdConfig?.Invoke(runtimeCmdMock);
            return mock.Object;
        }
    }
}
