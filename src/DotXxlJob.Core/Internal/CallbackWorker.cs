// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Internal
{
    internal class CallbackWorker : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
