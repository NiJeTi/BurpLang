using System.Collections;
using System.Linq;
using System.Text;

namespace BurpLang.Core
{
    public static class Converter
    {
        public static string Serialize(object? target) => SerializeInternal(target, 1);
        
        private static string SerializeInternal(object? target, int nestingLevel)
        {
            switch (target)
            {
                case var t when t is null:
                    return "_";

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

                    string margin = CreateMargin(nestingLevel);

                    foreach (var item in e)
                    {
                        string serialized = SerializeInternal(item, nestingLevel + 1);

                        serializer.Append(margin).AppendLine(serialized);
                    }

                    serializer.Append(CreateMargin(nestingLevel - 1)).Append(']');

                    return serializer.ToString();
                }

                default:
                {
                    var serializer = new StringBuilder();
                    serializer.Append('<').AppendLine();

                    string margin = CreateMargin(nestingLevel);

                    foreach (var property in target!.GetType().GetProperties())
                    {
                        string serialized = SerializeInternal(property.GetValue(target), nestingLevel + 1);

                        serializer.Append(margin).AppendLine($"{property.Name} = {serialized}");
                    }

                    serializer.Append(CreateMargin(nestingLevel - 1)).Append('>');

                    return serializer.ToString();
                }
            }
        }

        private static string CreateMargin(int level) => new string(Enumerable.Repeat(' ', level * 4).ToArray());
    }
}