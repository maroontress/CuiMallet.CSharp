namespace Maroontress.Cui;

using System.Collections.Generic;

/// <summary>
/// Represents an <see cref="Option"/> that must have an option argument in the
/// command-line options.
/// </summary>
public interface RequiredArgumentOption : Option
{
    /// <summary>
    /// Gets the name of the option argument.
    /// </summary>
    string ArgumentName { get; }

    /// <summary>
    /// Gets the values of all the option arguments corresponding to the same
    /// option in occurrence order. Note that they do not contain the option
    /// arguments of the options specified after this option.
    /// </summary>
    IEnumerable<string> ArgumentValues { get; }

    /// <summary>
    /// Gets the value of the option argument.
    /// </summary>
    string ArgumentValue { get; }
}
