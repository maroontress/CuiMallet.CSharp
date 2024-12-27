namespace Maroontress.Cui.Impl;

using System;
using System.Collections.Generic;
using StyleChecker.Annotations;

/// <summary>
/// The specification part of <see cref="OptionImpl"/>.
/// </summary>
/// <param name="name">
/// The name.
/// </param>
/// <param name="shortName">
/// The short name.
/// </param>
/// <param name="description">
/// The description.
/// </param>
/// <param name="action">
/// The action to be invoked.
/// </param>
/// <seealso cref="OptionSchema.Add(string, char?, string, Action{Option})"/>
public sealed class OptionSpec(
    string name,
    char? shortName,
    string description,
    Action<Option> action)
    : AbstractSpec<Option>(name, shortName, description, action)
{
    /// <inheritdoc/>
    public override void VisitLongOption(
        ParseKit kit,
        Builder builder,
        string? arguemnt,
        [Unused] Queue<string> queue)
    {
        if (arguemnt is not null)
        {
            throw kit.NewUnableToGetArgument();
        }
        builder.AddOption(() => kit, this);
    }

    /// <inheritdoc/>
    public override void VisitShortOption(
        ParseKit kit,
        Builder builder,
        [Unused] Queue<char> shortNameQueue,
        [Unused] Queue<string> queue)
    {
        builder.AddOption(() => kit, this);
    }

    /// <inheritdoc/>
    public override string GetHelpHeading()
    {
        return $"{GetShortNameHeading()}--{Name}";
    }
}
