using System;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;

namespace CommandPattern
{
    class Program
    {
        public class Options
        {
            public string Destination { get; set; } = "No where";
            public int Pace { get; set; } = 30;
        }

        public class RunOptions : Options
        {
            public bool Sprint { get; set; }
        }

        public class WalkOptions : Options
        {
            public bool Skip { get; set; }
        }

        private static string[] HelpContent =
        {
            "",
            "NAME:     CommandPatternDemo",
            "USAGE:    dotnet run -- {run | walk} [options]",
            "COMMANDS: run  - You will run like Forest Gump to your destination.",
            "          walk - You will leisurely walk to your destination. ",
            "OPTIONS:  -d, --destination: Where you are going",
            "          -p, --pace:        Number of steps between 30 and 60 per minute you will take",
            "          --skip:            Whether to skip merrily while walking",
            "          --sprint:          Whether to spring while running",
            ""
        };

        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration<Options>();

            // Configure run command
            config
                .Command<RunOptions>("run", cmd => cmd
                    .Switch("--sprint", arg => arg.Map.ToProperty(opt => opt.Sprint))
                    // Notice we are "overriding" the pace option, and changing the validation range because
                    // we're running. The 60-120 rule will take precedence over the 30-60 rule if the user
                    // decides to run
                    .Option<int>("-p|--pace", arg => arg
                        .Map.ToProperty(opt => opt.Pace)
                        .Validate.Between(60, 120, messageFormat: (_, value) => $"Pace should be between 60 and 120 steps per minute (you put {value}...)"))
                    .OnExecute(Run));

            // Configure walk command
            config
                .Command<WalkOptions>("walk", cmd => cmd
                    .Switch("--skip", arg => arg.Map.ToProperty(opt => opt.Skip))
                    .OnExecute(Walk));

            // Configure main application stuff
            config
                .HelpOption("-h|--help")
                .Help.UseContent(HelpContent)
                // Main/base/root application options and switches common to all commands
                .Option("-d|--destination", arg => arg.Map.ToProperty(opt => opt.Destination))
                .Option<int>("-p|--pace", arg => arg
                    .Map.ToProperty(opt => opt.Pace)
                    .Validate.Between(30, 60, messageFormat: (_, value) => $"Pace should be between 30 and 60 steps per minute (you put {value}...)"))
                // Here we are insisting that the user specify a command
                .OnExecute(_ => throw new Exception("You must run or walk..."));

            try
            {
                CommandLineApplication.Run(config, args);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Type -h or --help to get help.");
            }
        }

        private static void Walk(WalkOptions obj)
        {
            if (obj.Skip) Console.WriteLine($"Skipping to {obj.Destination} at a pace of {obj.Pace} steps/minute.");
            else Console.WriteLine($"Walking (boring...) to {obj.Destination} at a pace of {obj.Pace} steps/minute.");
        }

        private static void Run(RunOptions obj)
        {
            if (obj.Sprint) Console.WriteLine($"Sprinting to {obj.Destination} at a pace of {obj.Pace} steps/minute.");
            else Console.WriteLine($"Running to {obj.Destination} at a pace of {obj.Pace} steps/minute.");
        }
    }
}
