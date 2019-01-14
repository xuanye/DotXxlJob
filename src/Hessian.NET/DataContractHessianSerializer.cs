using System;
using System.IO;

namespace Hessian.Net
{
    /// <summary>
    /// Serialize objects to binary and deserialize binary to objects using the Caucho Hessian 2.0 Serialization Protocol.
    /// </summary>
    /// <remarks>
    /// This class cannot be inherited.
    /// </remarks>
    public sealed class DataContractHessianSerializer
    {
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractHessianSerializer" /> class to serialize or deserialize an object of the specified type and serializer settings.
        /// </summary>
        /// <param name="type">The type of the instance that is serialized or deserialized.</param>
        /// <param name="settings">The serializer settings.</param>
        public DataContractHessianSerializer(Type type, HessianSerializerSettings settings = null)
        {
            this.type = type;
        }

        /// <summary>
        /// Serializes a specified object to binary data and writes the resulting to a stream.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream" /> that is written to.</param>
        /// <param name="graph">The object that contains the data to write to the stream.</param>
        public void WriteObject(Stream stream, object graph)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var factory = new HessianObjectSerializerFactory();
            var scheme = HessianSerializationScheme.CreateFromType(type, factory);

            using (var writer = new HessianOutputWriter(stream))
            {
                var context = new HessianSerializationContext();
                scheme.Serialize(writer, graph, context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public object ReadObject(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var factory = new HessianObjectSerializerFactory();
            var scheme = HessianSerializationScheme.CreateFromType(type, factory);

            using (var reader = new HessianInputReader(stream))
            {
                var context = new HessianSerializationContext();
                return scheme.Deserialize(reader, context);
            }
        }
    }
}
