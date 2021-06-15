// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Provides a base class for argument parsers.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public abstract class ArgumentParser<TOptions, TValue> : IArgumentParser<TOptions>
        where TOptions : class
    {
        private readonly IValueConverter<TValue> _converter;
        private readonly IValidator<TValue> _validator;
        private readonly IMapper<TOptions, TValue> _mapper;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="order"><see cref="Parsing.ParserType"/></param>
        /// <param name="converter">Converter instance.</param>
        /// <param name="validator">Validator instance chain.</param>
        /// <param name="mapper">Mapper instance.</param>
        protected ArgumentParser(ParserType order,
            IValueConverter<TValue> converter,
            IValidator<TValue> validator,
            IMapper<TOptions, TValue> mapper
            ) : this(null, order, converter, validator, mapper)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="parserType">Parse order.</param>
        /// <param name="converter">Converter instance.</param>
        /// <param name="validator">Validator chain.</param>
        /// <param name="mapper">Mapper instance.</param>
        protected ArgumentParser(Template template,
            ParserType parserType,
            IValueConverter<TValue> converter,
            IValidator<TValue> validator,
            IMapper<TOptions, TValue> mapper)
        {
            _converter = converter ?? throw new ArgumentNullException();
            _validator = validator;
            _mapper = mapper ?? throw new ArgumentNullException();
            Template = template;
            ParserType = parserType;
        }
        
        /// <inheritdoc />
        public ParserType ParserType { get; }

        /// <inheritdoc />
        public bool MultiValued => _mapper.MultiValued;

        /// <inheritdoc />
        public abstract ContextResult ProcessContext(TOptions options, ParseContext parseContext);

        /// <summary>
        /// Gets the template value.
        /// </summary>
        protected Template Template { get; }
        
        /// <summary>
        /// Maps the given argument value.
        /// </summary>
        /// <param name="options">Options instance.</param>
        /// <param name="token">Value to map.</param>
        protected void AcceptArgumentValue(TOptions options, Token token)
        {
            // Convert
            var convertedValue = ValueConverter.Convert(_converter, Context, token.Value);

            // Validate
            Validator.Validate(_validator, Context, convertedValue);

            // Map
            Mapper.MapValue(_mapper, Context, options, convertedValue);
        }

        /// <summary>
        /// Gets the context for the parser.
        /// </summary>
        protected abstract string Context { get; }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => Context;
    }
}