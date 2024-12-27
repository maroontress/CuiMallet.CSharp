namespace Maroontress.Cui.Impl;

using System;

/// <summary>
/// The builder of option schema.
/// </summary>
/// <param name="addOption">
/// The action that consumes an <see cref="OptionSpec"/> object.
/// </param>
/// <param name="addRequiredArgumentOption">
/// The action that consumes an <see cref="RequiredArgumentOptionSpec"/>
/// object and its argument.
/// </param>
public sealed class Builder(
    Action<Func<string>, OptionSpec> addOption,
    Action<Func<string>, RequiredArgumentOptionSpec, string>
            addRequiredArgumentOption)
{
    /// <summary>
    /// Gets the action that consumes an <see cref="OptionSpec"/> object.
    /// </summary>
    public Action<Func<string>, OptionSpec> AddOption { get; } = addOption;

    /// <summary>
    /// Gets the action that consumes an <see
    /// cref="RequiredArgumentOptionSpec"/> object and its argument.
    /// </summary>
    public Action<Func<string>, RequiredArgumentOptionSpec, string>
        AddRequiredArgumentOption { get; } = addRequiredArgumentOption;
}
