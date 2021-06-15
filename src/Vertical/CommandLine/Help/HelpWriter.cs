// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents an object that writes help content to a text writer.
    /// </summary>
    public class HelpWriter : IHelpWriter
    {
        private const char Space = ' ';
        
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="textWriter">Text writer that receives the output.</param>
        /// <param name="formatInfo">Formatting information.</param>
        public HelpWriter(TextWriter textWriter, FormatInfo formatInfo)
        {
            FormatInfo = formatInfo ?? throw new ArgumentNullException(nameof(formatInfo));
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        /// <summary>
        /// Gets the format info.
        /// </summary>
        protected FormatInfo FormatInfo { get; }

        /// <summary>
        /// Gets the text writer.
        /// </summary>
        protected TextWriter TextWriter { get; }

        /// <inheritdoc />
        public virtual void WriteContent(IReadOnlyCollection<string> content)
        {
            WriteContent(TextWriter, content, FormatInfo);
        }

        /// <summary>
        /// Writes the given help content within the specified display region.
        /// </summary>
        /// <param name="textWriter">The text writer that receives the content.</param>
        /// <param name="content">The source content.</param>
        /// <param name="formatInfo">The format info.</param>
        /// <returns>The number of virtually rendered lines of help content.</returns>
        internal static int WriteContent(TextWriter textWriter, IEnumerable<string> content, FormatInfo formatInfo)
        {
            var virtualRowId = 0;
            var firstRow = formatInfo.StartRow;
            var lastRow = firstRow + formatInfo.FormatHeight - 1;
            var insertCrLf = false;

            foreach (var line in content)
            {
                var charArray = line.ToCharArray();
                var js = formatInfo.LineFormatter.CreateFormatted(line);

                foreach (var span in js.SplitToWidth(formatInfo.FormatWidth - js.Indent))
                {
                    if (virtualRowId >= firstRow && virtualRowId <= lastRow)
                    {
                        if (insertCrLf) textWriter.WriteLine();
                        for (var i = 0; i < js.Indent; i++) textWriter.Write(Space);
                        textWriter.Write(charArray, span.Start, span.Length);
                        insertCrLf = true;
                    }

                    ++virtualRowId;
                }
            }

            return virtualRowId;
        }
    }
}
