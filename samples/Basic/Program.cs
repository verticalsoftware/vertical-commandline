using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;

namespace Basic
{
    class Program
    {
        public enum Role {  Dev, QA, Architect }
        
        public class Options
        {
            public string Name { get; set; } = "good sir/ma'am";
            public Role Role { get; set; }
            public IList<string> Languages { get; } = new List<string>();
        }

        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration<Options>()
                .Option("-n|--name", arg => arg.Map.ToProperty(opt => opt.Name))
                .Option<Role>("-r|--role", arg => arg.Map.ToProperty(opt => opt.Role))
                .PositionArgument(arg => arg.MapMany.ToCollection(opt => opt.Languages))
                .OnExecute(options =>
                {
                    var languageStr = options.Languages.Any()
                        ? string.Join(", ", options.Languages)
                        : "...you don't have any language preferences?";

                    Console.WriteLine($"Hello {options.Name}, as a {options.Role} I see you enjoy {languageStr}");
                });
            
            try
            {
                CommandLineApplication.Run(config, args);
            }
            catch(Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        public class DocOptions
        {
            public DateTimeOffset Date { get; set; }
            public string Password { get; set; }
            public Color Color { get; set; }
            public IList<string> Sources { get; }
            public double Amount { get; set;  }
            public int Size { get; set; }
        }

        static void Docs()
        {
            var config = new ApplicationConfiguration<DocOptions>();

            var extensions = new[] {".cs", ".js", ".sql"};

            config.Options.UseFactory(() => new DocOptions());

            config.Option("--source", arg => arg.Validate.Using(
                    extensions,
                    (state, value) => state.Contains(Path.GetExtension(value)),
                    (state, value) => $"Files can only be of type: {string.Join(",", extensions)}")
                )
                .OnExecute(Run);
        }

        private static void Run(DocOptions obj)
        {
            throw new NotImplementedException();
        }
    }
}
