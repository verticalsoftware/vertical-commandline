// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.IO;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a provider that loads help content from a file.
    /// </summary>
    internal sealed class FileHelpContentProvider : IProvider<IReadOnlyCollection<string>>
    {
        private readonly string _path;
        private const int DefaultContentCapacity = 500;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        internal FileHelpContentProvider(string path)
        {
            Check.NotNullOrWhiteSpace(path, nameof(path));

            _path = path;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GetInstance()
        {
            var contentList = new List<string>(DefaultContentCapacity);
            
            using(var reader = new StreamReader(_path))
            {
                string content;

                while((content = reader.ReadLine()) != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList.AsReadOnly();
        }
    }
}
