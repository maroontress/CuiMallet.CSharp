namespace Maroontress.Cui.Impl
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default implementation of <see cref="Setting"/> interface.
    /// </summary>
    public sealed class SettingImpl : Setting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingImpl"/> class.
        /// </summary>
        /// <param name="schema">
        /// The instance that creates this object.
        /// </param>
        /// <param name="args">
        /// The remaining non-option arguments.
        /// </param>
        /// <param name="options">
        /// The options in order of appearance.
        /// </param>
        public SettingImpl(
            OptionSchema schema,
            IEnumerable<string> args,
            IEnumerable<Option> options)
        {
            Schema = schema;
            Arguments = args.ToArray();
            Options = options.ToArray();
        }

        /// <inheritdoc/>
        public OptionSchema Schema { get; }

        /// <inheritdoc/>
        public IEnumerable<string> Arguments { get; }

        /// <inheritdoc/>
        public IEnumerable<Option> Options { get; }
    }
}
