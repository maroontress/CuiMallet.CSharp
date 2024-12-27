namespace Maroontress.Cui.Impl;

using System;

/// <summary>
/// The default implementation of <see cref="Option"/> interface.
/// </summary>
/// <param name="supplier">
/// The function returns the string representing this option, which is actually
/// specified with the command line.
/// </param>
/// <param name="spec">
/// The specification of the option.
/// </param>
/// <param name="schema">
/// The schema of the option.
/// </param>
/// <seealso cref="OptionSchema.Add(string, char?, string, Action{Option})"/>
public sealed class OptionImpl(
    Func<string> supplier, OptionSpec spec, OptionSchema schema)
    : AbstractOption(supplier), Option
{
    /// <inheritdoc/>
    public string Name => Spec.Name;

    /// <inheritdoc/>
    public char? ShortName => Spec.ShortName;

    /// <inheritdoc/>
    public string Description => Spec.Description;

    /// <inheritdoc/>
    public OptionSchema Schema { get; } = schema;

    private OptionSpec Spec { get; } = spec;
}
