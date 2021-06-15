// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a text writer that writes help content to the console.
    /// </summary>
    public sealed class ConsoleHelpWriter : IHelpWriter
    {
        private ConsoleHelpWriter()
        {
        }

        /// <summary>
        /// Defines a default instance.
        /// </summary>
        public static IHelpWriter Default { get; } = new ConsoleHelpWriter();

        /// <inheritdoc />
        public void WriteContent(IReadOnlyCollection<string> content)
        {
            HelpWriter.WriteContent(Console.Out,
                content,
                new FormatInfo(Console.WindowWidth, int.MaxValue, 0));

            Console.WriteLine();
        }
    }
}
