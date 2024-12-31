# CuiMallet.CSharp

CuiMallet is a .NET library that makes command-line interfaces. It depends on
.NET Standard 2.1.

## Get started

CuiMallet.CSharp is available as the
[![NuGet-logo][nuget-logo] NuGet package][nuget-maroontress-cuimallet].

## How to parse command-line options

CuiMallet makes it easy to implement the conventions for command-line options
that POSIX recommends \[[1](#ref1)\] and to which GNU adds long options
\[[2](#ref2)\].

### Overview

First, create an [`OptionSchema`][apiref-optionschema] object as follows:

```csharp
var schema = Options.NewSchema();
```

Then, add the definition of the command-line options to the `OptionSchema`
object. For example, to add the `--help` option, which has the shortened form
`-h`, no argument, and the description `"Show help message"` for its help
message, use the `Add(string, char?, string)` method against the `OptionSchema`
object as follows:

```csharp
schema = schema.Add("help", 'h', "Show help message");
```

Note that the `OptionSchema` object is immutable, so the `Add` method returns
a new `OptionSchema` object.

After adding all the options, use the `Parse(string[])` method against the
`schema`, with the specified `string` array representing the command-line
arguments, which a static method `Main(string[])` typically provides at the
entry point of a program, as follows:

```csharp
public static void Main(string[] args)
{
    var schema = Options.NewSchema()
        // Adds options to schema.
        ⋮
        .Add("help", 'h', "Show help message");
    try
    {
        var setting = schema.Parse(args);
        var options = setting.Options;
        var arguments = setting.Arguments;
        // Customizes the program behavior according to the options and
        // arguments.
        ⋮
    }
    catch (OptionParsingException e)
    {
        // Handles the exception that the Parse method may throw.
        ⋮
    }
}
```

> [Run](https://dotnetfiddle.net/893GBp)

The `Parse(string[])` method returns the [`Setting`][apiref-setting] object,
which contains the options and the arguments. The `options` represent the parsed
options in order of appearance. Each option is given as an
[`Option`][apiref-option] object, and the type of the `options` is
`IEnumerable<Option>`. Meanwhile, the `arguments` represent the remaining
non-option arguments. The type of the `arguments` is `IEnumerable<string>`.

In addition, the `Parse(string[])` method must be in the `try` block followed by
the `catch` clause handling an `OptionParsingException` because it may throw the
exception when the specified option is not valid for the schema.

### Options and non-option arguments

Here, there is a relationship between the options and the non-option arguments
illustrated as follows:

> _command_ [_Options_...] [`--`] [_Arguments_...]

- A component of _Options..._ is a list separated with whitespace characters,
  which contains a string starting with a hyphen (`-`) character and consisting
  of two or more characters (e.g., `--help`, `-h`). The string starting with a
  double hyphen (`--`) is called an _option_, and the string after the double
  hyphen is called the _option name_. The option name consists of alphanumeric
  characters and hyphens. Meanwhile, the string consisting of a single character
  preceded by a single hyphen (`-`) is called a _shortened-form_ option, and the
  character after the hyphen is called a _short name_ of the option. The short
  names are single alphanumeric characters.

- A component of _Arguments..._ is a list of strings separated with whitespace
  characters.

Each component enclosed in square brackets can be omitted. However, a double
hyphen (`--`) must be placed between _Options..._ and _Arguments..._ only if the
first element of _Arguments..._ begins with a hyphen character and is not a
hyphen exactly.

Note that the option name in CuiMallet corresponds to the long option name in
GNU `getopt_long` \[[3](#ref3)\], and the short name corresponds to the option
name in POSIX `getopt` \[[4](#ref4)\].

### An option with an option argument

There are two types of options: the options that can't have an option argument
(e.g., `--help` as described above), and that must have a single-option argument.
For example, suppose the `--file` option, which has the shortened form `-f`,
requires an option argument. Then its argument must be supplied in any one of
the following forms:

- `--file ARGUMENT`
- `--file=ARGUMENT`
- `-f ARGUMENT`
- `-fARGUMENT`

Where you must replace the `ARGUMENT` with the actual option argument.

Thus, `--file index.html`, `--file=infex.html`, `-f index.html`, and
`-findex.html` are equivalent. However, if the `ARGUMENT` must be a zero-length
string (i.e., an empty string), you cannot use the `-fARGUMENT` form.

Note that, in general, two types of options have an option argument: the
required argument option and the optional argument option. The former cannot
omit an option argument, but the latter can. However, CuiMallet has not yet
implemented the latter.

### Concatenating the shortened-form options

Specifying `-a -b -c` and `-abc` are equivalent. The concatenated options,
except the last one, can't have an option argument. Only the last one can have
an option argument as follows:

> `-abcf ARGUMENT` (or `-abcfARGUMENT`)

Where the `ARGUMENT` is of the `-f` option, and `-a`, `-b`, and `-c`
options cannot have an option argument.

### Abbreviating an option name

You can abbreviate the option names as long as the abbreviations are unique. For
example, suppose there are only three options in the schema: `--help`,
`--verbose`, and `--version`. Specifying `--h`, `--he`, `--hel`, and `--help`
are equivalent. Likewise, `--verb --vers` is equivalent to
`--verbose --version`. However, specifying `--ver` causes an error because two
options start with it.

### Adding a required argument option to the schema

To add the `--file` option (as described above) to the schema, use the
`Add(string, char?, string, string)` method against the schema object as
follows:

```csharp
schema = schema.Add("file", 'f', "FILE", "Specify an input file");
```

To get the value of the actual option argument, use a
[`RequiredArgumentOption`][apiref-requiredargumentoption] object as follows:

```csharp
var options = setting.Options;
foreach (o in options)
{
    if (o is RequiredArgumentOption a)
    {
        var arg = a.ArgumentValue;
        ⋮
    }
    ⋮
}
```

Thus, the `ArgumentValue` property of a `RequiredArgumentOption` object provides
the value of the option argument.

### Options with an option argument specified two or more times

You can specify the same option two or more times. For example, suppose the
option `-f`, as noted above, which takes an argument, is specified on the
command line as follows:

> `-f foo -f bar -f baz`

In such cases, the `ArgumentValues` property of a `RequiredArgumentOption`
object is useful. The following code shows how to use the property:

```csharp
var options = setting.Options;
foreach (o in options)
{
    if (o is RequiredArgumentOption a)
    {
        var arg = a.ArgumentValue;
        var all = string.Join(",", a.ArgumentValues);
        Console.WriteLine($"{arg} {all}");
    }
}
```

> [Run](https://dotnetfiddle.net/tbeoDf)

The `ArgumentValues` property returns the values of all the option arguments
corresponding to the same option in occurrence order. Note that they do not
contain the option arguments of the options specified after it. Thus, the output
to the console is as follows:

```plaintext
foo foo
bar foo,bar
baz foo,bar,baz
```

### Callback at parsing a command-line option

You can also specify a callback function that takes the `Option` or
`RequiredArgumentOption` object as the argument when adding the definition of
the command-line options to the `OptionSchema` object. From invoking the
`Parse(string[])` method of the `OptionSchema` object until returning, the
function is called to provide the `Option` or `RequiredArgumentOption` object
each time the object is created.

To add the option, which has no option argument, with a callback function to the
`OptionSchema` object, use the `Add(string, char?, string, Action<Option>)`
method as follows:

```csharp
// Adds the definition of an Option with the callback function.
schema = schema.Add(
    "help",
    'h',
    "Show help message",
    o =>
    {
        // typeof(o) is Option.
        ⋮
    });
```

In the same way, to add the option, which has an option argument, with a
callback function to the `OptionSchema` object, use the
`Add(string, char?, string, string, Action<RequiredArgumentOption>)` method as
follows:

```csharp
// Adds the definition of a RequiredArgumentOption with the callback
// function.
schema = schema.Add(
    "file",
    'f',
    "FILE",
    "Specify an input file",
    o =>
    {
        // typeof(o) is RequiredArgumentOption.
        ⋮
    });
```

### Getting the help message

You can generate the help message of command-line options with the
`OptionSchema` object. To get the help message, use the `GetHelpMessage()`
method against the `schema` as follows:

```csharp
public static void Main(string[] args)
{
    var schema = Options.NewSchema()
        .Add("file", 'f', "FILE", "Specify an input file")
        .Add("help", 'h', "Show help message");

    PrintUsage(schema, Console.Out);
}

private static void PrintUsage(OptionSchema schema, TextWriter output)
{
    var usage = new[]
    {
        "usage: command [Options...] [--] Arguments...",
        "",
        "Options are:",
    };
    var messages = usage.Concat(schema.GetHelpMessage());
    foreach (var m in messages)
    {
        output.WriteLine(m);
    }
}
```

The type of the value the method returns is `IEnumerable<string>`. The output to
the console is as follows:

```plaintext
usage: command [Options...] [--] Arguments...

Options are:
-f, --file FILE     Specify an input file
-h, --help          Show help message
```

If the description has to be composed of two or more lines, you can split it
into them by inserting a line feed character (`'\n'`) as a line separator. See
the following example:

```csharp
var schema = Options.NewSchema()
    .Add(
        "file",
        'f',
        "FILE",
        "Specify an input file\n"
            + "The FILE can be - for standard input")
    .Add("help", 'h', "Show help message");
⋮
```

As in the previous example, the output to the console is as follows:

```plaintext
⋮
-f, --file FILE     Specify an input file
                    The FILE can be - for standard input
-h, --help          Show help message
```

> [Run](https://dotnetfiddle.net/sfEknT)

Note that the `GetHelpMessage()` method sorts options by name.

### Handling an exception at parsing command-line options

The `Parse(string[])` method, as mentioned earlier, throws an
`OptionParsingException` when the specified argument to the schema is invalid.
Specific cases are as follows:

> **Unknown option**  
> The specified option was not found in the schema.
>
> **Missing an argument**  
> The specified option requires an argument, but no argument was given.
>
> **Unable to get an argument**  
> The specified option takes no argument, but the argument was given.
>
> **Ambiguous option**  
> The name of the specified option is abbreviated,
> but the abbreviations were not unique.

In most cases, all you need in the catch clause is to write the message of the
exception to the standard error output, print the usage, and then exit with a
non-zero status code. The following code shows a typical example:

```csharp
try
{
    var setting = schema.Parse(args);
    ⋮
}
catch (OptionParsingException e)
{
    var output = Console.Error;
    output.WriteLine(e.Message);
    PrintUsage(schema, output);
    Environment.Exit(1);
}
```

When specifying a callback function for a required argument option, the value of
the option argument often has to be validated in the function. In that case, you
can throw an `OptionParsingException` when the validation fails so that the
catch clause noted above handles the exception as well as the other
`OptionParsingException`s.

For example, suppose the `--count` option, which requires the option argument
representing a positive integer. You can parse the command-line options to get
the value of the option argument, as follows:

```csharp
private const int DefaultCount = 3;

private static int Count { get; set; } = DefaultCount;

private static void ParseCount(RequiredArgumentOption o)
{
    var v = o.ArgumentValue;
    if (!int.TryParse(v, out var num) || num < 0)
    {
        var n = o.ArgumentName;
        throw new OptionParsingException(
            o, $"option '{o}': the value '{v}' is invalid for {n}");
    }
    Count = num;
}

public static void Main(string[] args)
{
    var schema = Options.NewSchema()
        .Add(
            "count",
            'c',
            "NUM",
            "Specifies the count",
            ParseCount)
        ⋮
    try
    {
        var setting = schema.Parse(args);
        ⋮
    }
    catch (OptionParsingException e)
    {
        var output = Console.Error;
        output.WriteLine(e.Message);
        PrintUsage(schema, output);
        Environment.Exit(1);
    }
    ⋮
```

> [Run `--count=10`](https://dotnetfiddle.net/rSR7Df)

The `Count` property returns 10 after parsing `args` containing `--count=10`.
But if replacing it with `--count=abc`, you have the output to the console as
follows:

```plaintext
option '--count=abc': the value 'abc' is invalid for NUM
```

> [Run `--count=abc`](https://dotnetfiddle.net/L1BCNi)

## API Reference

- [Maroontress.Cui][apiref-maroontress.cui] namespace

## How to build

### Requirements to build

- Visual Studio 2022 Version 17.12 or [.NET 9.0 SDK (SDK 9.0.101)][dotnet-sdk]

### How to get started

```plaintext
git clone URL
cd CuiMallet.CSharp
dotnet build --configuration Release
```

### How to get test coverage report with Coverlet

```plaintext
dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet test --configuration Release --no-build \
  --logger "console;verbosity=detailed" \
  --collect:"XPlat Code Coverage" \
  --results-directory MsTestResults
reportgenerator -reports:MsTestResults/*/coverage.cobertura.xml \
  -targetdir:Coverlet-html
```

## References

<a name="ref1"></a>
[1] [POSIX, _Utility Conventions_][posix-utility-conventions]

<a name="ref2"></a>
[2] [The GNU C Library, _Program Argument Syntax Conventions_][gnu-program-argument-syntax-conventions]

<a name="ref3"></a>
[3] [The GNU C Library, _Parsing Long Options with `getopt_long`_][gnu-getopt_long]

<a name="ref4"></a>
[4] [POSIX, _`getopt`, `optarg`, `opterr`, `optind`, `optopt` &mdash; command option parsing_][posix-getopt]

[posix-getopt]:
  https://pubs.opengroup.org/onlinepubs/9699919799/functions/getopt.html
[posix-utility-conventions]:
  https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html
[gnu-program-argument-syntax-conventions]:
  https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html
[gnu-getopt_long]:
  https://www.gnu.org/software/libc/manual/html_node/Getopt-Long-Options.html
[apiref-optionschema]:
  https://maroontress.github.io/CuiMallet-CSharp/api/latest/html/Maroontress.Cui.OptionSchema.html
[apiref-option]:
  https://maroontress.github.io/CuiMallet-CSharp/api/latest/html/Maroontress.Cui.Option.html
[apiref-setting]:
  https://maroontress.github.io/CuiMallet-CSharp/api/latest/html/Maroontress.Cui.Setting.html
[apiref-requiredargumentoption]:
  https://maroontress.github.io/CuiMallet-CSharp/api/latest/html/Maroontress.Cui.RequiredArgumentOption.html
[nuget-maroontress-cuimallet]:
  https://www.nuget.org/packages/Maroontress.CuiMallet/
[nuget-logo]:
  https://maroontress.github.io/images/NuGet-logo.png
[github-logo]:
  https://maroontress.github.io/images/GitHub-logo.png
[github-cuimallet-csharp]:
  https://github.com/maroontress/CuiMallet.CSharp
[dotnet-sdk]:
  https://dotnet.microsoft.com/en-us/download/dotnet/9.0
[apiref-maroontress.cui]:
  https://maroontress.github.io/CuiMallet-CSharp/api/latest/html/Maroontress.Cui.html
