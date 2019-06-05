namespace Maroontress.Cui
{
    using System;

    /// <summary>
    /// Represents a program termination.
    /// </summary>
    public sealed class TerminateProgramException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="TerminateProgramException"/> class.
        /// </summary>
        public TerminateProgramException()
        {
            StatusCode = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="TerminateProgramException"/> class.
        /// </summary>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public TerminateProgramException(int statusCode)
            => StatusCode = statusCode;

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public int StatusCode { get; }
    }
}
