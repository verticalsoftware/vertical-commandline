// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using System.Linq;
using System.Threading;
using Vertical.CommandLine.Parsing;
using System.Threading.Tasks;
using Vertical.CommandLine.Provider;
using Vertical.CommandLine.Tests.Parsing;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class CommandConfigurationTests
    {
        public class MyOptions
        {
            public string Value { get; set; }
            public bool Bool { get; set; }
        }

        private readonly CommandConfiguration<MyOptions> _instanceUnderTest =
            new CommandConfiguration<MyOptions>(null);
        private readonly MyOptions _testOptions = new MyOptions();
        

        [Fact]
        public void ConstructAssignsTemplate()
        {
            var config = new CommandConfiguration<MyOptions>(Template.ForCommand("test"));
            var template = config.RuntimeCommand.Template;
            template.Tokens.Single().ShouldBe(new Token(TokenType.NonTemplateValue, "test"));
        }

        [Fact]
        public void PositionArgumentRegistersParser()
        {
            const string value = "file";
            _instanceUnderTest.PositionArgument(arg => arg.Map.ToProperty(opt => opt.Value));
            _instanceUnderTest.RuntimeCommand.MapArguments(NewParseContext(value), _testOptions, ParserType.PositionArgument);
            _testOptions.Value.ShouldBe(value);
        }

        [Fact]
        public void OptionRegistersParser()
        {
            const string value = "value";
            _instanceUnderTest.Option("-o", cfg => cfg.Map.ToProperty(opt => opt.Value));
            _instanceUnderTest.RuntimeCommand.MapArguments(NewParseContext("-o", value), _testOptions, ParserType.Option);
            _testOptions.Value.ShouldBe(value);
        }

        [Fact]
        public void SwitchRegistersParser()
        {
            _instanceUnderTest.Switch("-s", cfg => cfg.Map.ToProperty(opt => opt.Bool));
            _instanceUnderTest.RuntimeCommand.MapArguments(NewParseContext("-s"), _testOptions, ParserType.Option);
            _testOptions.Bool.ShouldBeTrue();
        }

        [Fact]
        public void OnExecuteRegistersDelegate()
        {
            var invoked = false;
            _instanceUnderTest.OnExecute(_ => invoked = true);
            _instanceUnderTest.RuntimeCommand.Invoke(_testOptions);
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task OnExecuteAsyncRegistersDelegate()
        {
            var invoked = false;
            _instanceUnderTest.OnExecuteAsync(_ =>
            {
                invoked = true;
                return Task.CompletedTask;
            });
            await _instanceUnderTest.RuntimeCommand.InvokeAsync(_testOptions, CancellationToken.None);
            invoked.ShouldBeTrue();
        }
        
        [Fact]
        public async Task OnExecuteAsyncWithCancellationSupportRegistersDelegate()
        {
            using var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            var invoked = false;
            
            _instanceUnderTest.OnExecuteAsync((_, ct) =>
            {
                invoked = true;
                ct.ShouldBe(cancelToken);
                return Task.CompletedTask;
            });
            await _instanceUnderTest.RuntimeCommand.InvokeAsync(_testOptions, cancelToken);
            invoked.ShouldBeTrue();
        }

        [Fact]
        public void OnExecuteThrowsWhenHandlerNull()
        {
            Should.Throw<ArgumentNullException>(() => _instanceUnderTest.OnExecute(null!));
        }

        [Fact]
        public void OnExecuteAsyncThrowsWhenHandlerNull()
        {
            Should.Throw<ArgumentNullException>(() => _instanceUnderTest.OnExecuteAsync(default(Func<MyOptions, Task>)!));
        }

        [Fact]
        public void OptionsRegistersProvider()
        {
            _instanceUnderTest.Options.UseInstance(_testOptions);
            _instanceUnderTest.RuntimeCommand.GetOptions().ShouldBeSameAs(_testOptions);
        }

        [Fact]
        public void HelpRegistersProvider()
        {
            _instanceUnderTest.Help.UseContent(new[] { "help" });
            _instanceUnderTest.RuntimeCommand.HelpContentProvider.ShouldBeOfType(typeof(InstanceProvider<IReadOnlyCollection<string>>));
        }

        [Fact]
        public void RuntimeCommandReturnsInstance()
        {
            _instanceUnderTest.RuntimeCommand.ShouldNotBeNull();
        }

        [Fact]
        public void SubConfigurationsReturnsNull()
        {
            // Commands cannot have sub configurations
            _instanceUnderTest.SubConfigurations.ShouldBeNull();
        }

        [Fact]
        public void HelpWriterReturnsNull()
        {
            // Help writer registered at root level, property override
            _instanceUnderTest.HelpWriter.ShouldBeNull();
        }

        private static ParseContext NewParseContext(params string[] args) => new ParseContext(args);
    }
}
