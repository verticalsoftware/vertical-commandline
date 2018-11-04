using System;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;

namespace LessHelp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration<object>()
                .HelpOption("-h|--help", InteractiveConsoleHelpWriter.Default)
                .Help.UseFile("help.txt")
                .OnExecute(_ => throw new Exception("Use -h or --help"));

            try
            {
                CommandLineApplication.Run(config, args);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
