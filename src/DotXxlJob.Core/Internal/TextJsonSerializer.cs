// Copyright (c) Xuanye Wang. All rights reserved.
// Licensed under MIT license

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Internal
{
    public class TextJsonSerializer : ISerializer
    {
        public T? Deserialize<T>(byte[] data) where T : class
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json);
        }

        public byte[] Serialize<T>(T item) where T : class
        {
            return JsonSerializer.SerializeToUtf8Bytes(item);
        }

        public object? Deserialize(byte[] data, Type type)
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize(json, type);
        }

        public byte[] Serialize(object item, Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(item);
        }


    }
}