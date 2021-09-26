// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Parsing
{
    /// <summary>
    /// Defines the various token types.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Represents an argument that begins with a single dash. 
        /// </summary>
        ShortOption,
        
        /// <summary>
        /// Represents an argument that begins with a double dash.
        /// </summary>
        LongOption,
        
        /// <summary>
        /// Represents an argument that terminates the option section,
        /// a sole double dash value.
        /// </summary>
        OptionsEnd,
        
        /// <summary>
        /// Indicates an unprefixed string value.
        /// </summary>
        NonTemplateValue,
        
        /// <summary>
        /// Represents a dashed argument whose value is combined with = or :.
        /// </summary>
        CompositeOption
    }
}