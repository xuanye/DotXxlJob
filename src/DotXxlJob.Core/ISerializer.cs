// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public interface ISerializer
    {
        T? Deserialize<T>(byte[] data) where T : class;
        byte[] Serialize<T>(T item) where T : class;
        object? Deserialize(byte[] data,Type type);
        byte[] Serialize(object item,Type type);
    }
}