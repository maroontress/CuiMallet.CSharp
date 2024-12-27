namespace Maroontress.Cui.Impl;

using System;
using System.Collections.Generic;

/// <summary>
/// The specification part of RequiredArgumentOptionImpl.
/// </summary>
public sealed class RequiredArgumentOptionSpec
    : AbstractSpec<RequiredArgumentOption>
{
    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="RequiredArgumentOptionSpec"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="shortName">
    /// The short name.
    /// </param>
    /// <param name="argumentName">
    /// The name of the option argument.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="action">
    /// The action to be invoked.
    /// </param>
    /// <seealso cref="OptionSchema.Add(string, char?, string, string,
    /// Action{RequiredArgumentOption})"/>
    public RequiredArgumentOptionSpec(
        string name,
        char? shortName,
        string argumentName,
        string description,
        Action<RequiredArgumentOption> action)
        : base(name, shortName, description, action)
    {
        ArgumentName = argumentName;
    }

    /// <summary>
    /// Gets the argument name of the option.
    /// </summary>
    public string ArgumentName { get; }

    /// <inheritdoc/>
    public override void VisitLongOption(
        ParseKit kit,
        Builder builder,
        string? argument,
        Queue<string> queue)
    {
        if (argument is null)
        {
            AddOptionWithNextArgument(kit, builder, queue);
            return;
        }
        builder.AddRequiredArgumentOption(() => kit, this, argument);
    }

    /// <inheritdoc/>
    public override void VisitShortOption(
        ParseKit kit,
        Builder builder,
        Queue<char> shortNameQueue,
        Queue<string> queue)
    {
        if (shortNameQueue.Count == 0)
        {
            AddOptionWithNextArgument(kit, builder, queue);
            return;
        }
        var argument = new string([.. shortNameQueue]);
        shortNameQueue.Clear();
        builder.AddRequiredArgumentOption(() => kit, this, argument);
    }

    /// <inheritdoc/>
    public override string GetHelpHeading()
    {
        return $"{GetShortNameHeading()}--{Name} {ArgumentName}";
    }

    private void AddOptionWithNextArgument(
        ParseKit kit,
        Builder builder,
        Queue<string> queue)
    {
        if (queue.Count == 0)
        {
            throw kit.NewMissingArgument();
        }
        var argument = queue.Dequeue();
        builder.AddRequiredArgumentOption(() => kit, this, argument);
    }
}
