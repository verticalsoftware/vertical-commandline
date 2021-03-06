﻿// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

namespace Vertical.CommandLine.Tests
{
    public class CommandLineApplicationTests
    {
        [Fact]
        public void RunInvokesHandler()
        {
            var invoked = false;
            
            CommandLineApplication.Run(new ApplicationConfiguration<object>()
                .OnExecute(_ => invoked = true), Array.Empty<string>());
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task RunAsyncInvokesHandler()
        {
            var invoked = false;

            await CommandLineApplication.RunAsync(new ApplicationConfiguration<object>()
                .OnExecuteAsync(_ =>
                {
                    invoked = true;
                    return Task.CompletedTask;
                }), Array.Empty<string>());

            invoked.ShouldBeTrue();
        }

        [Fact]
        public void ShowHelpThrowsForNoConfiguredTemplate()
        {
            Should.Throw<ConfigurationException>(() => CommandLineApplication.ShowHelp(
                new ApplicationConfiguration<object>()));
        }

        [Fact]
        public void ShowHelpInvokesWriter()
        {
            var writer = new Mock<IHelpWriter>();
            writer.Setup(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>())).Verifiable();
            var config = new ApplicationConfiguration<object>()
                .HelpOption("-h", writer.Object)
                .Help.UseContent(new[] {"help"});
            CommandLineApplication.ShowHelp(config);
            writer.Verify(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>()), Times.Once);
        }

        [Theory, MemberData(nameof(ShowHelpTheories))]
        public void ShowHelpDisplaysCorrectContent(ApplicationConfiguration<object> config, string command, string expected)
        {
            using (var textWriter = new StringWriter())
            {
                config.HelpOption("-h", new HelpWriter(textWriter, FormatInfo.Default));
                CommandLineApplication.ShowHelp(config, command);
                var helpContent = textWriter.ToString();
                helpContent.ShouldBe(expected);
            }
        }

        public static IEnumerable<object[]> ShowHelpTheories = Scenarios
        (
            Scenario(new ApplicationConfiguration<object>().Help.UseContent(new[]{"root"}), string.Empty, "root"),
            Scenario
            (
                new ApplicationConfiguration<object>()
                    .Command("test", cmd => cmd.Help.UseContent(new []{"command"}))
                    .Help.UseContent(new []{"root"}),
                string.Empty,
                "root"
            ),
            Scenario
            (
                new ApplicationConfiguration<object>()
                    .Command("command", cmd => cmd.Help.UseContent(new[] { "command" }))
                    .Help.UseContent(new[] { "root" }),
                "command",
                "command"
            )
        );

        [Fact]
        public void ParseArgumentsReturnsArgs()
        {
            var counter = 0;
            var config = new ApplicationConfiguration<IDictionary<string, string>>();

            config
                .Option("--option", arg => arg.Map.Using((options, value) => options["option"] = value))
                .Switch("--switch", arg => arg.Map.Using((options, _) => options["switch"] = "true"))
                .PositionArgument(arg => arg.MapMany.Using((options, value) => options[$"{++counter}"] = value))
                .Options.UseInstance(new Dictionary<string, string>());

            var result = CommandLineApplication.ParseArguments<IDictionary<string,string>>(config, new[]
            {
                "--option", "test-option",
                "--switch",
                "value1",
                "value2"
            });

            result["option"].ShouldBe("test-option");
            result["switch"].ShouldBe("true");
            result["1"].ShouldBe("value1");
            result["2"].ShouldBe("value2");
        }
    }
}
