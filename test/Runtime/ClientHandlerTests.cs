// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using Shouldly;
using Vertical.CommandLine.Runtime;
using Xunit;

namespace Vertical.CommandLine.Tests.Runtime
{
    public class ClientHandlerTests
    {
        [Fact]
        public void ConstructWithNullDelegateThrows()
        {
            Should.Throw<ArgumentNullException>(() => new ClientHandler<object>((Action<object>) null));
        }

        [Fact]
        public void ConstructWithNullAsyncDelegateThrows()
        {
            Should.Throw<ArgumentNullException>(() => new ClientHandler<object>((Func<object, Task>)null));
        }

        [Fact]
        public void IsAsyncReturnsFalseForSyncHandler()
        {
            new ClientHandler<object>(opt => { }).IsAsync.ShouldBeFalse();
        }

        [Fact]
        public void IsAsyncReturnsTrueForAsyncHandler()
        {
            new ClientHandler<object>(opt => Task.CompletedTask).IsAsync.ShouldBeTrue();
        }

        [Fact]
        public void InvokeCallsDelegate()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(opt => { invoked = true; });
            handler.Invoke(null);
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task InvokeCallsDelegateWhenCalledAsync()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(opt => { invoked = true; });
            await handler.InvokeAsync(null);
            invoked.ShouldBeTrue();
        }

        [Fact]
        public async Task InvokeCallsDelegateAsync()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(opt =>
            {
                invoked = true;
                return Task.CompletedTask;
            });
            await handler.InvokeAsync(null);
            invoked.ShouldBeTrue();   
        }

        [Fact]
        public void InvokeCallsDelegateWhenConfiguredAsync()
        {
            var invoked = false;
            var handler = new ClientHandler<object>(opt =>
            {
                invoked = true;
                return Task.CompletedTask;
            });
            handler.Invoke(null);
            invoked.ShouldBeTrue();
        }
    }
}