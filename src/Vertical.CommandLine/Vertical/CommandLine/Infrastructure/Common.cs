// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using Vertical.CommandLine.Parsing;

namespace Vertical.CommandLine.Infrastructure
{
    internal static class Common
    {
        internal const string SingleDash = "-";

        internal const string DoubleDash = "--";

        internal const string OpExplicitMethodName = "op_Explicit";

        internal const string OpImplicitMethodName = "op_Implicit";
        
        internal const string InvalidValue = "value is invalid";

        internal static string FormatArgumentContext(int index) => $"argument @{index}";

        internal static string FormatSwitchContext(Template template) => $"switch {template}";

        internal static string FormatOptionContext(Template template) => $"option {template}";

        internal static string LessThanMessage<T>(T value) => $"value must be less than {value}";

        internal static string LessOrEqualMessage<T>(T value) => $"value must be less or equal to {value}";

        internal static string GreaterThanMessage<T>(T value) => $"value must be greater than {value}";

        internal static string GreaterEqualMessage<T>(T value) => $"value must be greater or equal to {value}";

        internal static string OneOfMessage<T>(IEnumerable<T> values) => $"value must be one of the following: {string.Join(", ", values)}";

        internal static string PatternMessage(object pattern) => $"value must match the following pattern: {pattern}";
    }
}