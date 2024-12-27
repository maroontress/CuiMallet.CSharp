namespace Maroontress.Cui.Impl;

using System.Collections.Generic;

/// <summary>
/// The specification of the <see cref="OptionSpec"/>Option.
/// </summary>
public interface Spec
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
    /// Visits the long option.
    /// </summary>
    /// <param name="kit">
    /// The parse kit.
    /// </param>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="argument">
    /// The argument if any.
    /// </param>
    /// <param name="queue">
    /// The queue of the command line options. If this option has an option
    /// argument, the first element of the queue is consumed as the option
    /// argument. Otherwise, the queue is not modified.
    /// </param>
    void VisitLongOption(
        ParseKit kit,
        Builder builder,
        string? argument,
        Queue<string> queue);

    /// <summary>
    /// Visits the short option.
    /// </summary>
    /// <param name="kit">
    /// The parse kit.
    /// </param>
    /// <param name="builder">
    /// The builder.
    /// </param>
    /// <param name="shortNameQueue">
    /// The queue of the short name options. If this option does not have an
    /// option argument, the queue is not modified. Otherwise, all the elements
    /// of the queue are consumed as the option argument (and then, the queue
    /// gets empty).
    /// </param>
    /// <param name="queue">
    /// The queue of the command line options. If this option has an option
    /// argument and <paramref name="shortNameQueue"/> is empty, the first
    /// element of the queue is consumed as the option argument. Otherwise, the
    /// queue is not modified.
    /// </param>
    void VisitShortOption(
        ParseKit kit,
        Builder builder,
        Queue<char> shortNameQueue,
        Queue<string> queue);

    /// <summary>
    /// Gets the heading of the help message.
    /// </summary>
    /// <returns>
    /// The heading of the help message.
    /// </returns>
    string GetHelpHeading();
}
