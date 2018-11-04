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
    /// Represents a parser that handles an option.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class OptionParser<TOptions, TValue> : ArgumentParser<TOptions, TValue>
        where TOptions : class
    {
        /// <inheritdoc />
        public OptionParser(Template template,
            IValueConverter<TValue> converter,
            IValidator<TValue> validator,
            IMapper<TOptions, TValue> mapper) :
            base(template, ParserType.Option, converter, validator, mapper)
        {
            Check.NotNull(Template, nameof(template));
        }

        /// <inheritdoc />
        protected override string Context => Common.FormatOptionContext(Template);

        /// <inheritdoc />
        public override ContextResult ProcessContext(TOptions options, ParseContext parseContext)
        {
            if (!parseContext.TryTakeTemplate(Template))
                return ContextResult.NoMatch;

            if (!parseContext.TryTakeStringValue(out var operandToken))
                throw Exceptions.OperandMissing(Template);
            
            AcceptArgumentValue(options, operandToken);
            
            return ContextResult.Argument;
        }
    }
}