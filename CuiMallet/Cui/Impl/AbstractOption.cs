namespace Maroontress.Cui.Impl;

using System;

/// <summary>
/// Provides abstraction of <see cref="OptionImpl"/> and <see
/// cref="RequiredArgumentOptionImpl"/>.
/// </summary>
public abstract class AbstractOption
{
    private protected AbstractOption(Func<string> supplier)
    {
        Supplier = supplier;
    }

    private Func<string> Supplier { get; }

    /// <summary>
    /// Gets the string representing this option, which is actually specified
    /// with the command line.
    /// </summary>
    /// <returns>
    /// If the option is specified with the shortened-form, its short name.
    /// Otherwise, the actual argument of the command-line options.
    /// </returns>
    public override sealed string ToString() => Supplier();
}
