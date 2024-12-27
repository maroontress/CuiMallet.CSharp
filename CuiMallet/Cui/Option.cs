namespace Maroontress.Cui;

/// <summary>
/// Represents an Option of the command-line options.
/// </summary>
public interface Option
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the short name, if any.
    /// </summary>
    char? ShortName { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the schema of this option.
    /// </summary>
    OptionSchema Schema { get; }
}
