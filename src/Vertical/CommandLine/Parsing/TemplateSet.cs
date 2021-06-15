// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a unique template set.
    /// </summary>
    internal sealed class TemplateSet
    {
        private readonly ISet<Token> _tokenSet = new HashSet<Token>();

        /// <summary>
        /// Adds a template to the set.
        /// </summary>
        /// <param name="template">Template to add.</param>
        /// <exception cref="System.Exception">Token is in use.</exception>
        public void Add(Template template)
        {
            Check.NotNull(template, nameof(template));

            foreach (var token in template.Tokens)
            {
                if (_tokenSet.Add(token))
                    continue;

                throw ConfigurationExceptions.TemplateTokenInUse(token);
            }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count => _tokenSet.Count;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => _tokenSet.Count.ToString();
    }
}