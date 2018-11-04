using System;
using System.IO;
using System.Linq;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;

namespace CommandLineTest
{
    public class Options
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration<Options>();

            var lines = Enumerable.Range(0, 500).Select(i => $"Line content @{i}");


            config
                .HelpOption("-h | --help", InteractiveConsoleHelpWriter.Default)
                .Command<Options>("build", command => command
                    .Help.UseFile("help-build.txt")
                    .OnExecute(RunBuild))                
                .Help.UseFile("help.txt")
                .OnExecute(RunDotNet);

            CommandLineApplication.Run(config, new[]{"--help"});
        }

        private static void RunBuild(Options obj)
        {
            Console.WriteLine("Ran build");
        }

        private static void RunDotNet(Options obj)
        {
            Console.WriteLine("Ran root");
        }
    }
}
