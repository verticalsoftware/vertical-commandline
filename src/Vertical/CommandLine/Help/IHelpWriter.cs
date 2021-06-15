// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Defines the interface of an object that writes help content.
    /// </summary>
    public interface IHelpWriter
    {
        /// <summary>
        /// Writes the command help content obtained from the given file resource.
        /// </summary>
        /// <param name="content">The content to display.</param>
        void WriteContent(IReadOnlyCollection<string> content);
    }
}
