// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Parsing;

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Base class for configurations.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public sealed class ParserConfiguration<TOptions> where TOptions : class
    {
        private readonly ICollection<IArgumentParser<TOptions>> _parsers = new List<IArgumentParser<TOptions>>();
        private readonly TemplateSet _templateSet = new TemplateSet();
        private int _positionArgIndex;

        internal ParserConfiguration()
        {
        }
        
        /// <summary>
        /// Configures an argument.
        /// </summary>
        /// <param name="context">Context of the argument.</param>
        /// <param name="configuration">Configuration action.</param>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <exception cref="Exception">Configuration exception.</exception>
        public void ConfigureParser<TValue>(object context,
            Func<ParserBuilder<TOptions, TValue>, IArgumentParser<TOptions>> configuration)
        {
            try
            {
                AddParser(configuration(new ParserBuilder<TOptions, TValue>()));
                if (context is Template template) AddTemplate(template);
            }
            catch (Exception ex)
            {
                throw ConfigurationExceptions.InvalidParserConfiguration(context.ToString(), ex);
            }
        }

        /// <summary>
        /// Adds a parser.
        /// </summary>
        /// <param name="parser">The parser to add.</param>
        public void AddParser(IArgumentParser<TOptions> parser) => _parsers.Add(parser);

        /// <summary>
        /// Adds a template.
        /// </summary>
        /// <param name="template">Template to add.</param>
        public void AddTemplate(Template template) => _templateSet.Add(template);
        
        /// <summary>
        /// Gets the argument parsers.
        /// </summary>
        public IArgumentParser<TOptions>[] ArgumentParsers => _parsers.ToArray();

        /// <summary>
        /// Gets the identity of the next argument.
        /// </summary>
        /// <returns>Identity.</returns>
        public int GetNextArgumentIdentity() => _positionArgIndex++;
    }
}