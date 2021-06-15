# vertical-commandline

An easily configured command line arguments parser that makes short work of turning those pesky `string[] args` into a strongly-typed configuration object.

![.net](https://img.shields.io/badge/Frameworks-.netstandard21+net50-purple)
![GitHub](https://img.shields.io/github/license/verticalsoftware/vertical-commandline)
![Package info](https://img.shields.io/nuget/vpre/vertical-commandline.svg)
![Package info](https://img.shields.io/nuget/v/vertical-commandline.svg)

[![Dev](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/dev-build.yml/badge.svg?branch=Dev)](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/dev-build.yml)
[![Pre release build and publish](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/pre-release.yml/badge.svg)](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/pre-release.yml)
[![Release](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/release.yml/badge.svg?branch=Dev)](https://github.com/verticalsoftware/vertical-commandline/actions/workflows/release.yml)
[![codecov (dev)](https://codecov.io/gh/verticalsoftware/vertical-commandline/branch/Dev/graph/badge.svg?token=U9GBSP77J9)](https://codecov.io/gh/verticalsoftware/vertical-commandline)

## At a glance
- No dependencies, targets netstandard2.1 and net5
- Declarative mapping without attributes
- Simple, concise configuration API
- Command verb support
- Double-dash option termination
- Rich out-of-box conversion and validation
- Synchronous and asynchronous Main method support
- Pretty print help system

## Quick start
Install the vertical-command nuget package into your project.
```
> Install-Package vertical-commandline
```
Define a class that will hold the application options, build a configuration, and then call the `Run` method of the `CommandLineApplication` class.
```csharp
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

        CommandLineApplication.Run(config, args);
    }
}
```

## Lexicon
In general, the API understands command line arguments as you would expect. For clarity, the lexicon is defined below.
- An _option_ or a _switch_ is defined using a single or a double dash and then an identifier. Short form identifiers are composed as a single dash followed by a single character (e.g. -a, -b). Long form options are composed as a double dash followed by a one or more characters (e.g. --help, --help-me). Note that short form identifiers can be combined from the command line (e.g. -abc is treated as -a -b -c).
- An option requires an operand that must immediately follow the identifier, and unlike GNU and most Unix command line utilities, there must be a space in between (good: --size 100MB, bad: --size100MB).
- An option can be configured to accept multiple values provided the identifier precedes each operand (e.g. --source IContract.cs --source Contract.cs ...)
- A switch is a boolean indication only and will not accept an operand.
- A command verb is a word that is not prefixed with dashes, and is only recognized if it is the very first argument.
- A position argument is a value that cannot be associated as the operand of an option, and is not matched to any defined command verbs. The ordinal placement of a position argument from the command line is irrelevant. Consider this example: --hash md5 **file1.txt** --size 100MB **file2.txt** -z **file3.txt**. All of the bolded arguments are treated as position arguments (assume -z is a switch).
- Any arguments that follow a double-dash are treated as position arguments regardless of whether or not they match an option, switch or command identifier. Any dashes in the arguments are retained.
- DOS/Powershell style slash options and switches are not supported (sorry).

## The API

### Overview
As shown in the quick start, the logical place to begin would be to create a class that holds the option values. Generally, this class should contain public writeable scalar properties or initialized read-only collection properties. It is recommended that the class has either a default or parameterless constructor. For both concerns, the API defines configurations to handle custom mapping and construction.

Next, you use a fluent configuration object to define your application's options, switches, arguments, commands, program logic handlers, and help content. This configuration can be finally passed to the `CommandLineApplication` class along with the arguments array given by the `Main` method.

### Defining options, switches and position arguments
Using the configuration object, the parsing capability is composed by invoking the `Option`, `Switch` or `PositionArgument` methods, specifying a template, and then configuring the item using an action. 

#### Templates
Templates instruct the parser what to look for when matching an option or switch. This is a string that has one or more identifiers separated by the pipe character. Any identifiers found in the template must be unique. The parser will trim each component of a template, so it's up to you whether or not to put spaces between the entries.

#### Conversion configuration
Arguments enter the API as strings. Because of this, they must be converted before they are mapped to an object if the target property is anything other than a string. Out-of-box, the API supports conversion to the following types:
- System numerical types (byte, int, long, short, etc..)
- Enums
- Types with a static Parse method that accepts a string
- Types that have an implicit or explicit conversion operator defined
- Types that have a constructor that accepts a string
- Nullable variants of any value types listed above

Rarely will you have to explicitly configure conversion, but for unhandled scenarios, you can use a delegate in the configuration or specify a map. You can find conversion methods by accessing the `Convert` property of the configuration object provided to the action.

```csharp
// Configure a conversion to Color using a delegate
config.Option<Color>("--color", arg => arg.Convert.Using(Color.FromName));

// Use a map to convert to System.Color. 
var colors = new Dictionary<string, Color>
{
    ["#FF0000"] = Color.Red,
    ["#00FF00"] = Color.Green,
    ["#0000FF"] = Color.Blue
};

config.Option<Color>("--color", arg => arg.Convert.UsingValues(colors), 
    StringComparer.OrdinalIgnoreCase // Optional - defaults to Comparer<string>.Default
);
```

#### Mapping configuration
Mapping is a required task so the API knows where to put the values it finds. It uses strongly-typed expressions to identify the properties of the target object. The API can map scalar values to public properties, or in the case of multi-valued options and position arguments, values can be added to collections, stacks, queues, or sets. You can find mapping methods by accessing the `Map` and `MapMany` properties of the configuration object provided to the action. The two methods differentiate single and multi-valued items, respectively.

```csharp
// Map a string value to the Password property
config.Option("-p|--pwd|--password", arg => arg.Map.ToProperty(opt => opt.Password));

// Map a value to the Color property. Note we specify the generic parameter since
// the property type is not a string
config.Option<Color>("--color", arg => arg.Map.ToProperty(opt => opt.Color));

// Map a multi-valued position argument to a collection. There are similar methods
// for the collection types such as ToStack(), ToQueue(), ToSet(), etc.
config.PositionArgument(arg => arg.MapMany.ToCollection(opt => opt.Sources));

// For mapping to a complex type, use a delegate
config.Option("--first-name", arg => arg.Map.Using((opt, value) => opt.Customer.FirstName = value));
```

#### Validation
You may opt to perform some validation on the command line values given by your users. Using the validation API, you can configure basic range checks on comparable values, intersection checks on equitable values, and pattern matching on string values. You can find validation methods by accessing the `Validate` property of the configuration object provided to the action.

```csharp
// Minimum check
config.Option<int>("--size", arg => arg.Validate.Greater(100));

// Inclusive range check
config.Option<double>("-a|--amount", arg => arg.Validate.Between(10, 20));

// Define a custom message to display if validation fails. The first argument
// to the lambda is the valid state value and the second is the actual value
// given. If a custom message is not provided, the API will display a default
// but sensible message
config.Option<int>("--size", arg => arg.Validate.Greater(
    100, 
    messageFormat: (min, value) => $"Value must be greater than {min}, you entered {value}")
);

// Use a delegate to perform validation
config.Option("--source", arg => arg.Validate.Using(File.Exists,
    value => $"Could not find source file: {value}")
);

// Embed the valid state into the validation. Useful for when you can't hard code
// the values.
var extensions = MyAppConfiguration["valid-source-extensions"]; // -> [.cs, .js, .sql]

config.Option("--source", arg => arg.Validate.Using(
    extensions,
    (state, value) => state.Contains(Path.GetExtension(value)),
    (state, value) => $"Files can only be of type: {string.Join(",", extensions)}")
);
```

#### The options type instance
If the type you define for options has a default constructor, the API will automatically create an instance of it. Otherwise if more control is needed, you can register a factory delegate or provide an instance.

```csharp
// Create using a factory delegate
config.Options.UseFactory(() => new MyOptions( /* I need parameters */ ));

// Give an instance
config.Options.UseInstance(myInstance);
```

#### Program handlers
Another required task is to configure where to direct the program execution after the arguments are parsed. The reason the API insists on this will be apparent in the next section when command verbs are introduced. In the meantime, you must define a delegate that can be called by the API. Both synchronous and asynchronous type callbacks are supported. Be sure to correctly pair `config.OnExecute()` and `config.OnExecuteAsync()` with `CommandLineApplication.Run()` and `CommandLineApplication.RunAsync()`, respectively. If you mismatch these, the API will forgive you and invoke the handler, but note your async delegate will run synchronously if called from the `Run()` method.

```csharp
// Synchronous
public static void Main(string[] args)
{
    // Configuration...

    // Register the program handler
    config.OnExecute(Run);    

    CommandLineApplication.Run(config, args);
}

private static void Run(Options options)
{
    // TODO: Program logic
}

// Asynchronous
public static async Task Main(string[] args)
{
    // Configuration...

    config.OnExecuteAsync(RunAsync);

    await CommandLineApplication.RunAsync(config, args);
}

private static Task RunAsync(Options options)
{
    // TODO: Asychronous program logic    
}
```

#### Command verbs
Command verbs can best be described as sub-programs within your main application. Consider the dotnet CLI. Certainly you've typed the following:
```
$ dotnet build -c release
```
Here _dotnet_ is the application being run, while _build_ is a command verb. The _dotnet_ application acts a gateway to the wide array of tools used to build, package and deploy .net applications. The API supports the notion of command verbs, and they can be configured directly off of the configuration objects we've been building so far. 

When introducing command verbs, we refer to the main application configuration as the _root_ configuration. Note the following API behaviors regarding command verb support:
- As stated earlier, the command verb must be the very first argument from the command line, and its template must not lead with a dash.
- Command verbs can contain an entirely new set of options, switches and position arguments that are unique to the verb, but they also inherit the properties of the root configuration. This allows you to reuse options, switches and position arguments common to all programs in the command verb configuration.
- Options, switches and position arguments that exist in the root configuration can be _redefined_ at the command level. That's to say you can have option -s defined in the root and in the command. In this case, if the command is invoked by the user, the option is parsed according to the rules defined by the command (e.g. conversion, validation and mapping) and not the root. If the command is not invoked, parsing falls back to the root.
- Options, switches and position arguments defined at the root continue to be parsed when a command is invoked, provided the command did not override the configuration as discussed in the last behavior.
- Command verbs can use a derivative subclass of the root configuration's option type as its own option type. This facilitates inheritance from the root configuration.
- Command verbs take their own execution path, so program handlers must be defined for each verb.
- Handlers for the root configuration and command verb configurations should be consistent with regard to the synchronicity model.

Configuration of a command begins by invoking the `Command` method off of the application configuration. Here you specify a template that identifies the verb, and then provide an action that is used for configuration. The configuration object fed to your action is a superclass of the application configuration, so there isn't anything additional that needs to be discussed. Refer to the CommandPattern sample to see a demonstration of implementing commands.

#### Displaying help content
The API decouples help content from the individual pieces that comprise a configuration. This means the library will make no assumptions regarding the format of your help page (aside from justifying text lines), and help content is provided to the API as a whole. The recommended way to define your help content is by including a plain text file in your application. The API will consume the content of the file, apply some basic formatting, and display it to the console. Content can also be provided in code, and output can be redirected to any text writer. Finally, there is also an interactive help mode that emulates the _less_ utility found on Linux.

There are two pieces to enabling application help. First, the help option and output options are defined in the root configuration using the `HelpOption` method, then the content is defined separately. Commands can have their own documentation, which is why the help configuration is split in this manner. When commands are involved, the API will seek specific help content, but fall back to the root help content if necessary. 

```csharp
// Define what triggers help. If the help writer is not specified, it defaults
// to the console help writer
config.HelpOption("-h|--help", ConsoleHelpWriter.Default);

// Define content for the root application
config.Help.UseFile("help.txt");

// Provide specific help for commands
config
    .Command("build", cmd => cmd.Help.UseFile("build-help.txt"))
    .Command("publish", cmd => cmd.Help.UseFile("publish-help.txt"))    
    .Help.UseFile("main-help.txt");

// Use the interactive help writer
console.HelpOption("-h|--help", InteractiveConsoleHelpWriter.Default);
```

#### Parsing only
If you only want to obtain the arguments without invoking the program handler, you may do so from the `CommandLineApplication` class. The `ParseArguments` method requires a generic parameter that identifies the type to return. When calling this method from your program code, and your setup includes commands, always specify the superclass type of the root configuration and never a subclassed command type since the return type depends on the command the user invoked. Use subclassed command types in unit tests only when you can control the arguments.

```csharp
var args = CommandLineApplication.ParseArguments<Options>(config, args);
```

#### Handling errors
The API can throw two flavors of exceptions:
- `ConfigurationException` is thrown when there is an error in setup. This includes things such as invalid or duplicate templates, arguments not defining a mapping to the options target, unsupported type conversions, invalid or inaccessible mapping expressions, program handlers not being defined, etc.
- `UsageException` is thrown when the configuration is correct, but the client of your application misuses the command line parameters. Examples include not providing a parameter value to an option, conversion failure, validation errors, etc. Specific types that derive from `UsageException` are `ConversionException` and `ValidationException`.

It is important to note that exceptions in the API are caught _up until_ control is referred to the defined client handler. This allows for clean separation between parsing problems and errors in the logic of your program.

## Finally
### Additional resources
Check out the projects in the [samples](https://github.com/verticalsoftware/vertical-commandline/tree/master/samples) folder to see complete configuration examples. If you're stuck, raise an issue for the benfit of others.

### Contributing
Yes! Help from the community is highly appreciated. Please [create an issue](https://github.com/verticalsoftware/vertical-commandline/issues/new) though so we can discuss the bug or feature.