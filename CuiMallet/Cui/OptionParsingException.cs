namespace Maroontress.Cui;

using System;

/// <summary>
/// Represents errors that occurs while parsing the command-line options.
/// </summary>
public sealed class OptionParsingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionParsingException"/>
    /// class.
    /// </summary>
    /// <param name="schema">
    /// The schema.
    /// </param>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public OptionParsingException(OptionSchema schema, string message)
        : base(message)
    {
        Schema = schema;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionParsingException"/>
    /// class.
    /// </summary>
    /// <param name="option">
    /// The option.
    /// </param>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public OptionParsingException(Option option, string message)
        : base(message)
    {
        Option = option;
        Schema = option.Schema;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionParsingException"/>
    /// class.
    /// </summary>
    /// <param name="option">
    /// The option.
    /// </param>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null
    /// reference if no inner exception is specified.
    /// </param>
    public OptionParsingException(
        Option option, string message, Exception innerException)
        : base(message, innerException)
    {
        Option = option;
        Schema = option.Schema;
    }

    /// <summary>
    /// Gets the option that failed to parse, if any.
    /// </summary>
    public Option? Option { get; }

    /// <summary>
    /// Gets the schema.
    /// </summary>
    public OptionSchema Schema { get; }
}
