using System;
using System.IO;
using System.Reflection;

namespace BingMapsTest.Resources
{
    public class BaseResource
    {
        public Stream GetStream(Type type, string filename)
        {
            var file = GetPath(type, filename);
            if (string.IsNullOrEmpty(file)) return null;
            var assembly = IntrospectionExtensions.GetTypeInfo(type).Assembly;
            if (assembly == null) return null;
            return assembly.GetManifestResourceStream(file);
        }

        public string GetPath(Type type, string filename)
        {
            return $"{type.Namespace}.{filename}";
        }
    }
}
