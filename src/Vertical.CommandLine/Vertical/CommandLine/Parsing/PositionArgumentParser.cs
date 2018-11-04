// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a parser that handles position arguments.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class PositionArgumentParser<TOptions, TValue> : ArgumentParser<TOptions, TValue>
        where TOptions : class
    {
        private readonly int _argumentIndex;
        
        /// <inheritdoc />
        internal PositionArgumentParser(int argumentIndex,
            IValueConverter<TValue> converter,
            IValidator<TValue> validator,
            IMapper<TOptions, TValue> mapper) :
            base(ParserType.PositionArgument, converter, validator, mapper)
        {
            _argumentIndex = argumentIndex;
        }

        /// <inheritdoc />
        public override ContextResult ProcessContext(TOptions options, ParseContext parseContext)
        {
            if (!parseContext.TryTakeStringValue(out var valueToken))
                return ContextResult.NoMatch;
            
            AcceptArgumentValue(options, valueToken);
            return ContextResult.Argument;
        }

        /// <summary>
        /// Gets the context for the parser.
        /// </summary>
        protected override string Context => Common.FormatArgumentContext(_argumentIndex);
    }
}