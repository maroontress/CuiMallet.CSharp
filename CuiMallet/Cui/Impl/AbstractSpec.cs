namespace Maroontress.Cui.Impl;

using System;
using System.Collections.Generic;

/// <summary>
/// The abstraction of <see cref="OptionSpec"/> and subclasses.
/// </summary>
/// <typeparam name="T">
/// The type of <see cref="Option"/> interface.
/// </typeparam>
/// <param name="name">
/// The name of the option.
/// </param>
/// <param name="shortName">
/// The short name of the option, if any.
/// </param>
/// <param name="description">
/// The description of the option.
/// </param>
/// <param name="action">
/// The action to be invoked.
/// </param>
public abstract class AbstractSpec<T>(
    string name,
    char? shortName,
    string description,
    Action<T> action)
    : Spec
    where T : Option
{
    /// <inheritdoc/>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public char? ShortName { get; } = shortName;

    /// <inheritdoc/>
    public string Description { get; } = description;

    private Action<T> Action { get; } = action;

    /// <summary>
    /// Invokes the callback function.
    /// </summary>
    /// <param name="option">
    /// The option to be consumed.
    /// </param>
    public void Fire(T option) => Action(option);

    /// <inheritdoc/>
    public abstract void VisitLongOption(
        ParseKit kit,
        Builder builer,
        string? argument,
        Queue<string> queue);

    /// <inheritdoc/>
    public abstract void VisitShortOption(
        ParseKit kit,
        Builder builder,
        Queue<char> shortNameQueue,
        Queue<string> queue);

    /// <inheritdoc/>
    public abstract string GetHelpHeading();

    private protected string GetShortNameHeading()
    {
        return ShortName.HasValue
            ? $"-{ShortName}, "
            : "    ";
    }
}
