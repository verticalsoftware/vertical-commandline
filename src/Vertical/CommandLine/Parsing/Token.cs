// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Diagnostics.CodeAnalysis;
using Vertical.CommandLine.Infrastructure;
using HashCode = System.HashCode;

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Represents a token of a template or command line argument.
    /// </summary>
    public readonly struct Token : IEquatable<Token>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <param name="value">Value (without prefixes).</param>
        internal Token(TokenType type, string? value)
        {
            Type = type;
            Value = value;

            if (type != TokenType.OptionsEnd)
            {
                Check.NotNullOrWhiteSpace(value, nameof(value));
            }
        }

        /// <summary>
        /// Defines an empty instance.
        /// </summary>
        public static Token Empty { get; } = default(Token);

        /// <summary>
        /// Defines the options end token.
        /// </summary>
        public static Token OptionsEnd { get; } = new Token(TokenType.OptionsEnd, null);
        
        /// <summary>
        /// Defines a token that represents boolean true.
        /// </summary>
        public static Token True { get; } = new Token(TokenType.NonTemplateValue, true.ToString());
        
        /// <summary>
        /// Gets the token type.
        /// </summary>
        public TokenType Type { get; }
        
        /// <summary>
        /// Gets the token value.
        /// </summary>
        public string? Value { get; }
        
        /// <inheritdoc />
        public override string ToString() => $"Type={Type}, Value={Formatting.Quote(Value ?? string.Empty)}";

        /// <summary>
        /// Gets a description of the token.
        /// </summary>
        /// <returns>String</returns>
        [ExcludeFromCodeCoverage]
        public string? DistinguishedForm
        {
            get
            {
                switch (Type)
                {
                    case TokenType.ShortOption: return $"{Common.SingleDash}{Value}";
                    case TokenType.LongOption: return $"{Common.DoubleDash}{Value}";
                    case TokenType.OptionsEnd: return Common.DoubleDash;
                    default: return Value;
                }
            }
        }

        /// <inheritdoc />
        public bool Equals(Token other)
        {
            return IsTypeCompatibleWith(other.Type) && string.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Token other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type.GetHashCode(), 
            (Value?.GetHashCode()).GetValueOrDefault());

        private bool IsTypeCompatibleWith(TokenType otherType)
        {
            switch (Type)
            {
                case TokenType.ShortOption:
                case TokenType.LongOption:
                case TokenType.CompositeOption:
                    return otherType == TokenType.LongOption
                           || otherType == TokenType.ShortOption
                           || otherType == TokenType.CompositeOption;
                
                default:
                    return Type == otherType;
            }
        }
    }
}