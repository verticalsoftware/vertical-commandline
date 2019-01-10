// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vertical.CommandLine.Infrastructure;
using Vertical.CommandLine.Provider;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a provider that loads help content from a file.
    /// </summary>
    internal sealed class FileHelpContentProvider : IProvider<IReadOnlyCollection<string>>
    {
        private static readonly string DefaultContentPath = Assembly
            .GetExecutingAssembly()
            .Location;
        
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
            
            using(var reader = new StreamReader(ResolveContentPath(_path)))
            {
                string content;

                while((content = reader.ReadLine()) != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList.AsReadOnly();
        }

        /// <summary>
        /// Resolves the content path if not rooted.
        /// </summary>
        private static string ResolveContentPath(string path)
        {
            // When invoking the utility from a location other than the bin
            // directory, relative paths won't be resolved correctly (for instance
            // from terminal)
            if (Path.IsPathRooted(path))
            {
                // Fully qualified
                return path;
            }

            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyPath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                return Path.Combine(directory, path);
            }
            
            throw new FileNotFoundException($"Cannot find help resource file '{path}'");
        }
    }
}
