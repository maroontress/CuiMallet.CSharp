namespace Maroontress.Cui
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the command-line options containing the parsed options and
    /// the remaining non-option arguments.
    /// </summary>
    public interface Setting
    {
        /// <summary>
        /// Gets the <see cref="OptionSchema"/> object that created this
        /// object.
        /// </summary>
        OptionSchema Schema { get; }

        /// <summary>
        /// Gets the remaining non-option arguments.
        /// </summary>
        IEnumerable<string> Arguments { get; }

        /// <summary>
        /// Gets the options in order of appearance.
        /// </summary>
        IEnumerable<Option> Options { get; }
    }
}
