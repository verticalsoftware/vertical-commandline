using System;
using System.Diagnostics.CodeAnalysis;

namespace Vertical.CommandLine
{
    /// <summary>
    /// Represents the base type for exceptions in this namespace.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class CommandLineException : Exception
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception or null reference.</param>
        protected CommandLineException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
