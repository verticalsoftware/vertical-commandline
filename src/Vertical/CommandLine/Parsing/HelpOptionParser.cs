// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a parser specific to the options parser.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    internal sealed class HelpOptionParser<TOptions> : IArgumentParser<TOptions>
        where TOptions : class
    {
        private readonly Template _template;

        internal HelpOptionParser(Template template)
        {
            _template = template;
        }
        
        /// <inheritdoc />
        public ParserType ParserType => ParserType.Help;

        /// <inheritdoc />
        public bool MultiValued => false;

        /// <inheritdoc />
        public ContextResult ProcessContext(TOptions options, ParseContext parseContext)
        {
            return parseContext.TryTakeTemplate(_template) ? ContextResult.Help : ContextResult.NoMatch;
        }
    }
}