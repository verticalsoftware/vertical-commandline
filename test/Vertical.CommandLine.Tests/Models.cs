// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;

namespace Vertical.CommandLine.Tests
{
    public static class Models
    {
        public enum Config { Debug, Release }
        public enum Framework { Net471, Net472, NetCoreApp20, NetCoreApp21 }
        public enum Runtime { Win10x86, Win10x64 }
        public enum Verbosity { Trace, Debug, Normal, Quiet, Minimal }

        public class Options : IEquatable<Options>
        {
            public string Project { get; set; }
            public bool Equals(Options other) => String.Equals(Project, other.Project);
        }

        public class BuildOptions : Options, IEquatable<BuildOptions>
        {
            public Config Configuration { get; set; }
            public Framework? Framework { get; set; }
            public Runtime? Runtime { get; set; }
            public Verbosity Verbosity { get; set; }
            public bool Force { get; set; }
            public bool NoDependencies { get; set; }
            public bool NoRestore { get; set; }
            public string Output { get; set; }
            public string VersionSuffix { get; set; }

            public bool Equals(BuildOptions other)
            {
                return Configuration == other.Configuration
                    && Framework == other.Framework
                    && Runtime == other.Runtime
                    && Force == other.Force
                    && NoDependencies == other.NoDependencies
                    && NoRestore == other.NoRestore
                    && String.Equals(Output, other.Output)
                    && String.Equals(VersionSuffix, other.VersionSuffix)
                    && base.Equals(other);
            }
        }
    }
}
