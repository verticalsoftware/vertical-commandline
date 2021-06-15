// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using Vertical.CommandLine.Help;
using Vertical.CommandLine.Parsing;
using Vertical.CommandLine.Runtime;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Defines the interface of a configuration object.
    /// </summary>
    public interface ICommandLineConfiguration
    {
        /// <summary>
        /// Gets the sub configurations for this instance.
        /// </summary>
        IEnumerable<ICommandLineConfiguration>? SubConfigurations { get; }

        /// <summary>
        /// Gets the help writer instance.
        /// </summary>
        IHelpWriter? HelpWriter { get; }

        /// <summary>
        /// Gets the runtime command for the configuration.
        /// </summary>
        /// <returns><see cref="IRuntimeCommand"/></returns>
        IRuntimeCommand GetRuntimeCommand();

        /// <summary>
        /// Gets the template that identifies the help option.
        /// </summary>
        Template? HelpTemplate { get; }
    }
}