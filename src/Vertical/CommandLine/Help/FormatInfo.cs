// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents properties that control how help content is formatted.
    /// </summary>
    public class FormatInfo
    {
        /// <summary>
        /// Defines the minimum margin width.
        /// </summary>
        public const int MinimumMarginWidth = 1;

        /// <summary>
        /// Defines the minimum margin height.
        /// </summary>
        public const int MinimumMarginHeight = 1;

        /// <summary>
        /// Creates a new instance of this type using the default formatter.
        /// </summary>
        /// <param name="marginWidth">Output width.</param>
        /// <param name="marginHeight">Output height.</param>
        /// <param name="startRow">Start row.</param>
        public FormatInfo(int marginWidth, int marginHeight, int startRow)
            : this(marginWidth, marginHeight, startRow, Formatter.JustifiedFormatter)
        {
        }

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="marginWidth">Output width.</param>
        /// <param name="marginHeight">Output height.</param>
        /// <param name="startRow">Start row.</param>
        /// <param name="formatter">Line formatter</param>
        public FormatInfo(int marginWidth, int marginHeight, int startRow, IFormatter formatter)
        {
            if (marginWidth < MinimumMarginWidth)
                throw ConfigurationExceptions.MinimumFormatInfoWidthNotMet(MinimumMarginWidth);

            if (marginHeight < MinimumMarginHeight)
                throw ConfigurationExceptions.MinimumFormatInfoHeightNotMet(MinimumMarginHeight);

            FormatWidth = marginWidth;
            FormatHeight = marginHeight;
            StartRow = startRow;
            LineFormatter = formatter ?? throw ConfigurationExceptions.HelpLineFormatterNotSet();
        }

        /// <summary>
        /// Defines a default instance.
        /// </summary>
        public static FormatInfo Default { get; } = new FormatInfo(int.MaxValue, int.MaxValue, 0);

        /// <summary>
        /// Gets the character width of the output.
        /// </summary>
        public int FormatWidth { get; }

        /// <summary>
        /// Gets the character height of the output.
        /// </summary>
        public int FormatHeight { get; }

        /// <summary>
        /// Gets the first row to print.
        /// </summary>
        public int StartRow { get; }

        /// <summary>
        /// Gets the line formatter.
        /// </summary>
        public IFormatter LineFormatter { get; }
    }
}
