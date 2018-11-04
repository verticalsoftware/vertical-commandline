// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vertical.CommandLine.Help;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Runtime
{
    /// <summary>
    /// Represents an IProgram implementation that displays help.
    /// </summary>
    internal sealed class HelpProgram : IProgram
    {
        private readonly IHelpWriter _helpWriter;
        private readonly IProvider<IReadOnlyCollection<string>> _helpContentProvider;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="helpWriter">Help writer</param>
        /// <param name="helpContentProvider">Provider for content</param>
        internal HelpProgram(IHelpWriter helpWriter, IProvider<IReadOnlyCollection<string>> helpContentProvider)
        {
            _helpWriter = helpWriter;
            _helpContentProvider = helpContentProvider ?? throw ConfigurationExceptions.NoHelpContentProviderDefined();
        }

        /// <inheritdoc />
        public void Invoke(object _) => InvokeHelp();

        /// <inheritdoc />
        public Task InvokeAsync(object _)
        {
            InvokeHelp();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Invokes the help system.
        /// </summary>
        private void InvokeHelp()
        {
            _helpWriter.WriteContent(GetHelpContent());
        }

        /// <summary>
        /// Resolves help content.
        /// </summary>
        private IReadOnlyCollection<string> GetHelpContent()
        {
            try
            {
                return _helpContentProvider.GetInstance() ??
                       throw ConfigurationExceptions.HelpProviderReturnedNull(_helpContentProvider.GetType());
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.HelpProviderFailed(_helpContentProvider.GetType(), ex);
            }
        }
    }
}