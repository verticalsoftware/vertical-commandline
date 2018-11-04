// Copyright(c) 2017 Vertical Software - All rights reserved
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
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Runtime;
using Xunit;
using static Vertical.CommandLine.Tests.Macros;

namespace Vertical.CommandLine.Tests.Runtime
{
    public class RuntimeCommandBuilderTests
    {
        private const string Project = "commandline.csproj";
        private const string BuildCommand = "build";

        [Fact]
        public void BuildWithNullArgsThrows()
        {
            Should.Throw<ArgumentNullException>(() =>
                RuntimeCommandBuilder.Build(new ApplicationConfiguration<object>(), null, out _));
        }

        [Fact]
        public void BuildWithNullConfigThrows()
        {
            Should.Throw<ArgumentNullException>(() =>
                RuntimeCommandBuilder.Build(null, Array.Empty<string>(), out _));
        }

        [Fact]
        public void ThrowsForInvalidOption()
        {
            var config = new ApplicationConfiguration<Models.Options>();
            config
                .PositionArgument(arg => arg.Map.ToProperty(o => o.Project))
                .OnExecute(_ => { });

            Should.Throw<UsageException>(() => RuntimeCommandBuilder.Build(
                config, new[] {"-f"}, out var options).Invoke(options));
                ;
        }

        [Fact]
        public void HelpInvoked()
        {
            var config = new ApplicationConfiguration<Models.Options>();
            var writerMock = new Mock<IHelpWriter>();
            writerMock.Setup(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>())).Verifiable();
            config.HelpOption("--help", writerMock.Object)
                .Help.UseContent(new[] { "help" });
            RuntimeCommandBuilder.Build(config, new[] { "--help" }, out var options).Invoke(options);
            writerMock.Verify(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>()), Times.Once);
        }

        [Fact]
        public void HelpInvokedWhenCommandConfigured()
        {
            var config = new ApplicationConfiguration<Models.Options>();
            var writerMock = new Mock<IHelpWriter>();
            writerMock.Setup(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>())).Verifiable();
            config.Command<Models.BuildOptions>("build", _ => { })
                .HelpOption("--help", writerMock.Object)
                .Help.UseContent(new[] { "help" });
            RuntimeCommandBuilder.Build(config, new[] { "--help" }, out var options).Invoke(options);
            writerMock.Verify(m => m.WriteContent(It.IsAny<IReadOnlyCollection<string>>()), Times.Once);
        }

        [Fact]
        public void HelpWithNoProviderThrows()
        {
            var config = new ApplicationConfiguration<Models.Options>();
            var writerMock = new Mock<IHelpWriter>();
            config.HelpOption("--help", writerMock.Object);
            Should.Throw<ConfigurationException>(() => RuntimeCommandBuilder.Build(config, new[] { "--help" },
                out var options).Invoke(options));
        }

        [Theory, MemberData(nameof(BaseAppTheories))]
        public void BaseAppTest(string[] args,
            Action<ApplicationConfiguration<Models.Options>> configure,
            Models.Options expectedOptions)
        {
            var configuration = new ApplicationConfiguration<Models.Options>();
            configure(configuration);
            configuration.OnExecute(opt => opt.ShouldBe(expectedOptions));
            RuntimeCommandBuilder.Build(configuration, args, out var options).Invoke(options);
        }

        [Theory, MemberData(nameof(BaseAppTheories))]
        public Task BaseAppAsyncTest(string[] args,
            Action<ApplicationConfiguration<Models.Options>> configure,
            Models.Options expectedOptions)
        {
            var configuration = new ApplicationConfiguration<Models.Options>();
            configure(configuration);
            configuration.OnExecuteAsync(opt =>
            {
                opt.ShouldBe(expectedOptions);
                return Task.CompletedTask;
            });
            return RuntimeCommandBuilder.Build(configuration, args, out var options).InvokeAsync(options);
        }

        public static IEnumerable<object[]> BaseAppTheories => new[]
        {
            // Empty arguments
            Scenario(
                Array.Empty<string>(),
                new Action<CommandConfiguration<Models.Options>>(_ => { }),
                new Models.Options()
            )

            // Project argument
            ,Scenario(
                new[]{Project},
                new Action<ApplicationConfiguration<Models.Options>>(app => app.PositionArgument(arg => arg.Map.ToProperty(o => o.Project))),
                new Models.Options{ Project = Project})

            // Project argument after --
            ,Scenario(
                new[]{ Common.DoubleDash, Project },
                new Action<ApplicationConfiguration<Models.Options>>(app => app.PositionArgument(arg => arg.Map.ToProperty(o => o.Project))),
                new Models.Options{ Project = Project })
        };

        [Theory, MemberData(nameof(CommandAppTheories))]
        public void CommandAppTest(string[] args,
            string commandName,
            Action<CommandConfiguration<Models.BuildOptions>> configure,
            Models.BuildOptions expectedOptions)
        {
            var configuration = new ApplicationConfiguration<Models.Options>();
            configuration
                .Command<Models.BuildOptions>(commandName, cfg =>
                {
                    configure(cfg);
                    cfg.OnExecute(opt => opt.ShouldBe(expectedOptions));
                })
                .PositionArgument(arg => arg.Map.ToProperty(o => o.Project))
                .OnExecute(_ => false.ShouldBeTrue("App handler called, shouldn't been"));
            RuntimeCommandBuilder.Build(configuration, args, out var options).Invoke(options);
        }

        [Theory, MemberData(nameof(CommandAppTheories))]
        public Task CommandAppAsyncTest(string[] args,
            string commandName,
            Action<CommandConfiguration<Models.BuildOptions>> configure,
            Models.BuildOptions expectedOptions)
        {
            var configuration = new ApplicationConfiguration<Models.Options>();
            configuration
                .Command<Models.BuildOptions>(commandName, cfg =>
                {
                    configure(cfg);
                    cfg.OnExecuteAsync(opt =>
                    {
                        opt.ShouldBe(expectedOptions);
                        return Task.CompletedTask;
                    });
                })
                .PositionArgument(arg => arg.Map.ToProperty(o => o.Project))
                .OnExecuteAsync(_ =>
                {
                    false.ShouldBeTrue("App handler called, shouldn't been");
                    return Task.CompletedTask;
                });
            return RuntimeCommandBuilder.Build(configuration, args, out var options)
                .InvokeAsync(options);
        }

        public static IEnumerable<object[]> CommandAppTheories => new[]
        {
            // Empty args
            Scenario(
                new[]{BuildCommand},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(_ => { }),
                new Models.BuildOptions())

            // Project routes to app options
            ,Scenario(
                new[]{BuildCommand, Project},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(_ => { }),
                new Models.BuildOptions{Project = Project})

            // Enums
            ,Scenario(
                new[]{BuildCommand, "-c", "release", "--framework", "NetCoreApp20", "-r", "Win10x64", "-v", "quiet"},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(ConfigureCommand),
                new Models.BuildOptions
                {
                    Configuration = Models.Config.Release,
                    Framework = Models.Framework.NetCoreApp20,
                    Runtime = Models.Runtime.Win10x64,
                    Verbosity = Models.Verbosity.Quiet
                })

            // Switches long form
            ,Scenario(
                new[]{BuildCommand, "--force", "--no-dependencies", "--no-restore"},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(ConfigureCommand),
                new Models.BuildOptions
                {
                    Force = true,
                    NoDependencies = true,
                    NoRestore = true
                })

            // Switches compact form
            ,Scenario(
                new[]{BuildCommand, "-fdx"},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(ConfigureCommand),
                new Models.BuildOptions
                {
                    Force = true,
                    NoDependencies = true,
                    NoRestore = true
                })

            // String arguments
            ,Scenario(
                new[]{BuildCommand, "--output", "bin/Debug/", "--version-suffix", "beta", Project},
                BuildCommand,
                new Action<CommandConfiguration<Models.BuildOptions>>(ConfigureCommand),
                new Models.BuildOptions
                {
                    Output = "bin/Debug/",
                    VersionSuffix = "beta",
                    Project = Project
                })
        };

        private static void ConfigureCommand(CommandConfiguration<Models.BuildOptions> config)
        {
            config
                .Option<Models.Config>("-c|--config", opt => opt.Map.ToProperty(o => o.Configuration))
                .Option<Models.Framework?>("--framework", opt => opt.Map.ToProperty(o => o.Framework))
                .Option<Models.Runtime?>("-r|--runtime", opt => opt.Map.ToProperty(o => o.Runtime))
                .Option<Models.Verbosity>("-v|--verbosity", opt => opt.Map.ToProperty(o => o.Verbosity))
                .Switch("-f|--force", opt => opt.Map.ToProperty(o => o.Force))
                .Switch("-d|--no-dependencies", opt => opt.Map.ToProperty(o => o.NoDependencies))
                .Switch("-x|--no-restore", opt => opt.Map.ToProperty(o => o.NoRestore))
                .Option("--output", opt => opt.Map.ToProperty(o => o.Output))
                .Option("--version-suffix", opt => opt.Map.ToProperty(o => o.VersionSuffix));
        }
    }
}