using System;
using System.Collections.Generic;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;

namespace ComplexMapping
{
    class Program
    {
        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Options
        {
            public Customer Customer { get; set; } = new Customer();
            public IList<string> Items { get; } = new List<string>();
        }

        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration<Options>();

            config
                .Option("-f|--first-name", arg => arg.Map.Using((options, value) => options.Customer.FirstName = value))
                .Option("-l|--last-name", arg => arg.Map.Using((options, value) => options.Customer.LastName = value))
                .PositionArgument(arg => arg.MapMany.ToCollection(opt => opt.Items));

            config
                .HelpOption("-h|--help")
                .Help.UseContent(new[]
                {
                    "",
                    "Name:      ComplexMapping",
                    "Usage:     dotnet run -- [options] [items...]",
                    "Options:   -f, --first-name : The customer's first name",
                    "           -l, --last-name  : The customer's last name",
                    "Arguments: [items...] : One or more items the customer will purchase"
                });

            config.OnExecute(options =>
            {
                
            });

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
