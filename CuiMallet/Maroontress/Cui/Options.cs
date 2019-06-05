namespace Maroontress.Cui
{
    using Maroontress.Cui.Impl;

    /// <summary>
    /// Provides <see cref="OptionSchema"/> instances.
    /// </summary>
    public static class Options
    {
        /// <summary>
        /// Gets an empty OptionSchema object.
        /// </summary>
        /// <returns>
        /// The empty OptionSchema object.
        /// </returns>
        public static OptionSchema NewSchema()
        {
            return OptionSchemaImpl.Empty;
        }
    }
}
