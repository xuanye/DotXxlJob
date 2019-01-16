using System;
using System.IO;
using System.Text;

namespace Hessian
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            var bytes = Encoding.UTF8.GetBytes("Hessian");
            var ms = new MemoryStream();
            ms.WriteByte((byte)"Hessian".Length);
            ms.Write (bytes, 0, bytes.Length);
            ms.Position = 0;

            var ds = new Deserializer(ms);

            var actual = ds.ReadValue();
            Console.WriteLine(actual);
        }
    }
}
