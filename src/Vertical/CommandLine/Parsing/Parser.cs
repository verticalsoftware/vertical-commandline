// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Linq;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents an object that can populate an options object
    /// with parsed argument values.
    /// </summary>
    internal static class Parser<TOptions> where TOptions : class
    {
        /// <summary>
        /// Maps the arguments found in the given context to the options instance using a set of
        /// parsers defined in a configuration.
        /// </summary>
        /// <param name="options">Options type.</param>
        /// <param name="parsers">Parsers used to process the context arguments.</param>
        /// <param name="parseContext">Context that contains the command line arguments.</param>
        /// <param name="parserType">Parser type filter.</param>
        /// <returns><see cref="ContextResult"/></returns>
        internal static ContextResult Map(TOptions options,
            IEnumerable<IArgumentParser<TOptions>> parsers,
            ParseContext parseContext,
            ParserType parserType)
        {
            return parsers
                .Where(p => p.ParserType == parserType)
                .TakeWhile(p => parseContext.Reset())
                .Aggregate(ContextResult.NoMatch, (current, parser) => InvokeParser(options, parseContext, current, parser));
        }

        // Invokes the parser on the context arguments
        private static ContextResult InvokeParser(TOptions options, 
            ParseContext parseContext, 
            ContextResult result,
            IArgumentParser<TOptions> parser)
        {
            while (parseContext.Ready)
            {
                var state = parseContext.Count;
                result |= parser.ProcessContext(options, parseContext);

                if (state != parseContext.Count && !parser.MultiValued)
                    break;
            }

            return result;
        }
    }
}