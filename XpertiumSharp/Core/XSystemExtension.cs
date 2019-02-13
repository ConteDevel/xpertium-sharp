using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XpertiumSharp.Core
{
    public static class XSystemExtension
    {
        public static T Clone<T>(this T source)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
