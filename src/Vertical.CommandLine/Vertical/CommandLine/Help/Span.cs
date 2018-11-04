// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a span that consists of a start index and a length.
    /// </summary>
    public struct Span : IEquatable<Span>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="length">Span length.</param>
        public Span(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Defines an empty instance.
        /// </summary>
        public static Span Empty { get; } = new Span(0, 0);

        /// <summary>
        /// Gets the start position of the span.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the length of the span.
        /// </summary>
        public int Length { get; }

        /// <inheritdoc />
        public bool Equals(Span other) => Start == other.Start && Length == other.Length;

        /// <summary>
        /// Determines equality among values
        /// </summary>
        /// <param name="x">First value to compare</param>
        /// <param name="y">Second value to compare</param>
        /// <returns>True if the instances are equal</returns>
        public static bool operator ==(Span x, Span y) => x.Equals(y);

        /// <summary>
        /// Determines inequality among values
        /// </summary>
        /// <param name="x">First value to compare</param>
        /// <param name="y">Second value to compare</param>
        /// <returns>True if the instances are not equal</returns>
        public static bool operator !=(Span x, Span y) => !x.Equals(y);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Span other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Start.GetHashCode(), Length.GetHashCode());

        /// <inheritdoc />
        public override string ToString() => $"@{Start} x {Length}";
    }
}
