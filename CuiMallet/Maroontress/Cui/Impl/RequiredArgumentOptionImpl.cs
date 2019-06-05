namespace Maroontress.Cui.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// The default implementation of <see cref="RequiredArgumentOption"/>
    /// interface.
    /// </summary>
    public sealed class RequiredArgumentOptionImpl
        : AbstractOption, RequiredArgumentOption
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="RequiredArgumentOptionImpl"/> class.
        /// </summary>
        /// <param name="supplier">
        /// The function returns the string representing this option, which is
        /// actually specified with the command line.
        /// </param>
        /// <param name="spec">
        /// The specification of the option.
        /// </param>
        /// <param name="schema">
        /// The schema of the option.
        /// </param>
        /// <param name="values">
        /// The argument values of the option.
        /// </param>
        /// <seealso cref="OptionSchema.Add(string, char?, string, string,
        /// Action{RequiredArgumentOption})"/>
        public RequiredArgumentOptionImpl(
            Func<string> supplier,
            RequiredArgumentOptionSpec spec,
            OptionSchema schema,
            IEnumerable<string> values)
            : base(supplier)
        {
            Spec = spec;
            Schema = schema;
            ArgumentValues = values.ToImmutableArray();
        }

        /// <inheritdoc/>
        public string Name => Spec.Name;

        /// <inheritdoc/>
        public char? ShortName => Spec.ShortName;

        /// <inheritdoc/>
        public string ArgumentName => Spec.ArgumentName;

        /// <inheritdoc/>
        public string Description => Spec.Description;

        /// <inheritdoc/>
        public IEnumerable<string> ArgumentValues { get; }

        /// <inheritdoc/>
        public string ArgumentValue => ArgumentValues.Last();

        /// <inheritdoc/>
        public OptionSchema Schema { get; }

        private RequiredArgumentOptionSpec Spec { get; }
    }
}
