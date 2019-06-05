namespace Maroontress.Cui
{
    using System;
    using System.Collections.Generic;
    using StyleChecker.Annotations;

    /// <summary>
    /// The definition of the command line options.
    /// </summary>
    public interface OptionSchema
    {
        /// <summary>
        /// Gets a new <see cref="OptionSchema"/> object with adding an option
        /// that requires an option argument and has the specified short name,
        /// the specified Action, the specified argument name, and the
        /// specified description.
        /// </summary>
        /// <param name="name">
        /// The name of the option. In general, it includes lowercase letters
        /// and hyphens. The name must be unique in this object.
        /// </param>
        /// <param name="shortName">
        /// The short name of the option. The short name must be unique in this
        /// object.
        /// </param>
        /// <param name="argumentName">
        /// The name of the option argument.
        /// </param>
        /// <param name="description">
        /// The description of the option. It can contain line feeds ('\n'),
        /// which represents a line separator.
        /// </param>
        /// <param name="action">
        /// The action to be invoked when the specified option and its argument
        /// are found while <see cref="OptionSchema.Parse(string[])"/> runs.
        /// </param>
        /// <returns>
        /// The new <see cref="OptionSchema"/> object.
        /// </returns>
        [return: DoNotIgnore]
        OptionSchema Add(
            string name,
            char? shortName,
            string argumentName,
            string description,
            Action<RequiredArgumentOption> action);

        /// <summary>
        /// Gets a new <see cref="OptionSchema"/> object with adding an option
        /// that has no argument, the specified short name, the specified
        /// Action, and the specified description.
        /// </summary>
        /// <param name="name">
        /// The name of the option. It can include lowercase letters and
        /// hyphens. The name must be unique in this object.
        /// </param>
        /// <param name="shortName">
        /// The short name of the option. The short name must be unique in this
        /// object.
        /// </param>
        /// <param name="description">
        /// The description of the option. It can contain line feeds ('\n'),
        /// which represents a line separator.
        /// </param>
        /// <param name="action">
        /// The action to be invoked when this option is found while <see
        /// cref="OptionSchema.Parse(string[])"/> runs.
        /// </param>
        /// <returns>
        /// The new <see cref="OptionSchema"/> object.
        /// </returns>
        [return: DoNotIgnore]
        OptionSchema Add(
            string name,
            char? shortName,
            string description,
            Action<Option> action);

        /// <summary>
        /// Gets a new <see cref="OptionSchema"/> object with adding an option
        /// that requires an option argument and has the specified short name,
        /// the specified argument name, and the specified description.
        /// </summary>
        /// <param name="name">
        /// The name of the option. In general, it includes lowercase letters
        /// and hyphens. The name must be unique in this object.
        /// </param>
        /// <param name="shortName">
        /// The short name of the option. The short name must be unique in this
        /// object.
        /// </param>
        /// <param name="argumentName">
        /// The name of the option argument.
        /// </param>
        /// <param name="description">
        /// The description of the option. It can contain line feeds ('\n'),
        /// which represents a line separator.
        /// </param>
        /// <returns>
        /// The new <see cref="OptionSchema"/> object.
        /// </returns>
        [return: DoNotIgnore]
        OptionSchema Add(
            string name,
            char? shortName,
            string argumentName,
            string description);

        /// <summary>
        /// Gets a new <see cref="OptionSchema"/> object with adding an option
        /// that has no argument, the specified short name, and the
        /// specified description.
        /// </summary>
        /// <param name="name">
        /// The name of the option. It can include lowercase letters and
        /// hyphens. The name must be unique in this object.
        /// </param>
        /// <param name="shortName">
        /// The short name of the option. The short name must be unique in this
        /// object.
        /// </param>
        /// <param name="description">
        /// The description of the option. It can contain line feeds ('\n'),
        /// which represents a line separator.
        /// </param>
        /// <returns>
        /// The new <see cref="OptionSchema"/> object.
        /// </returns>
        [return: DoNotIgnore]
        OptionSchema Add(string name, char? shortName, string description);

        /// <summary>
        /// Gets a new <see cref="Setting"/> object associated with this <see
        /// cref="OptionSchema"/>, which is the result of parsing the specified
        /// command-line options.
        /// </summary>
        /// <param name="args">
        /// The command-line options.
        /// </param>
        /// <returns>
        /// The new <see cref="Setting"/> object.
        /// </returns>
        Setting Parse(params string[] args);

        /// <summary>
        /// Gets the description of the command-line options.
        /// </summary>
        /// <returns>
        /// The description of this Options.
        /// </returns>
        IEnumerable<string> GetHelpMessage();
    }
}
