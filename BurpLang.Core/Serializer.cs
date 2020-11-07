using System.Collections;
using System.Linq;
using System.Text;

namespace BurpLang.Core
{
    internal static class Serializer
    {
        public static string? Serialize(object obj, int nestingLevel)
        {
            switch (obj)
            {
                case var t when t is string s:
                    return $"\"{s}\"";

                case var t when t is int i:
                    return i.ToString("D");

                case var t when t is float f:
                    return f.ToString("G");

                case var t when t is bool b:
                    return b ? "TRUE" : "FALSE";

                case var t when t is IEnumerable e:
                {
                    var serializer = new StringBuilder();
                    serializer.Append('[').AppendLine();

                    var margin = CreateMargin(nestingLevel);

                    foreach (var item in e)
                    {
                        var serialized = Serialize(item!, nestingLevel + 1);

                        if (serialized != null)
                            serializer.Append(margin).Append(serialized).AppendLine(",");
                    }

                    serializer.Append(CreateMargin(nestingLevel - 1)).Append(']');

                    return serializer.ToString();
                }

                default:
                {
                    if (nestingLevel > 1)
                        return null;

                    var serializer = new StringBuilder();
                    serializer.Append('<').AppendLine();

                    var margin = CreateMargin(nestingLevel);

                    foreach (var property in obj.GetType().GetProperties())
                    {
                        var name = property.Name;
                        var value = property.GetValue(obj);

                        if (value is null)
                            continue;

                        var serialized = Serialize(value!, nestingLevel + 1);

                        if (serialized != null)
                            serializer.Append(margin).Append(name).Append(" = ").Append(serialized).AppendLine();
                    }

                    serializer.Append(CreateMargin(nestingLevel - 1)).Append('>');

                    return serializer.ToString();
                }
            }
        }

        private static string CreateMargin(int level) =>
            new string(Enumerable.Repeat(' ', level * Converter.IndentationSize).ToArray());
    }
}