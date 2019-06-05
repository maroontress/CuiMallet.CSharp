namespace Maroontress.Cui.Impl
{
    using System;

    /// <summary>
    /// The builder of option schema.
    /// </summary>
    public sealed class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        /// <param name="addOption">
        /// The action that consumes an <see cref="OptionSpec"/> object.
        /// </param>
        /// <param name="addRequiredArgumentOption">
        /// The action that consumes an <see
        /// cref="RequiredArgumentOptionSpec"/> object and its argument.
        /// </param>
        public Builder(
            Action<Func<string>, OptionSpec> addOption,
            Action<Func<string>, RequiredArgumentOptionSpec, string>
                addRequiredArgumentOption)
        {
            AddOption = addOption;
            AddRequiredArgumentOption = addRequiredArgumentOption;
        }

        /// <summary>
        /// Gets the action that consumes an <see cref="OptionSpec"/> object.
        /// </summary>
        public Action<Func<string>, OptionSpec> AddOption { get; }

        /// <summary>
        /// Gets the action that consumes an <see
        /// cref="RequiredArgumentOptionSpec"/> object and its argument.
        /// </summary>
        public Action<Func<string>, RequiredArgumentOptionSpec, string>
            AddRequiredArgumentOption { get; }
    }
}
