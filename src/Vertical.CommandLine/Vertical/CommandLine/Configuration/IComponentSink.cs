// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Configuration
{
    /// <summary>
    /// Represents an object that receives configuration components.
    /// </summary>
    public interface IComponentSink<in TComponent>
    {
        /// <summary>
        /// Sinks the given component.
        /// </summary>
        /// <param name="component">The component instance.</param>
        void Sink(TComponent component);
    }
}