using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using BurpLang.Exceptions;

namespace BurpLang
{
    internal static class Deserializer
    {
        private const string ObjectPattern = @"\<.*\>";
        private const string StringPattern = "\"[^\"]*\"";
        private const string IntPattern = @"\d+";
        private const string FloatPattern = IntPattern + "." + IntPattern;
        private const string BoolPattern = "(TRUE)|(FALSE)";
        private const string EnumerablePattern = @"\[[^\]]*\]";

        private const RegexOptions SearchOptions = RegexOptions.Singleline;

        public static T Deserialize<T>(string input)
            where T : notnull, new()
        {
            input = Regex.Replace(input, "\\s|\r|\n|\r\n", string.Empty);

            if (!Regex.IsMatch(input, ObjectPattern, SearchOptions))
                throw new RootObjectNotFoundException(input);

            var rootObject = new T();

            foreach (var property in typeof(T).GetProperties()
               .Where(p => IsDeserializableType(p.PropertyType)))
            {
                var name = property.Name;
                var type = property.PropertyType;

                var valuePattern = GetRegex(property.PropertyType);
                var searchPattern = $@"{name}\=(?<{name}>{valuePattern});";

                var propertyStart = input.IndexOf(name, StringComparison.Ordinal);
                var i = propertyStart;

                var propertyRelatedText = string.Empty;
                
                while (!propertyRelatedText.EndsWith(';'))
                {
                    propertyRelatedText += input[i];
                    i++;
                }

                var parsedValue = Regex
                   .Match(propertyRelatedText, searchPattern, RegexOptions.Singleline)
                   .Groups[name].Value;

                object? deserializedValue;

                try
                {
                    deserializedValue = DeserializeValue(type, parsedValue);
                }
                catch (FormatException innerException)
                {
                    throw new FormatException($"Cannot deserialize \"{propertyRelatedText}\"", innerException);
                }

                property.SetValue(rootObject, deserializedValue);
            }

            return rootObject;
        }

        private static object DeserializeValue(Type type, string value)
        {
            switch (type)
            {
                case var t when t == typeof(string):
                    return value.Trim('\"');
                case var t when t == typeof(int):
                    return int.Parse(value);
                case var t when t == typeof(float):
                    return float.Parse(value);
                case var t when t == typeof(bool):
                    return value switch
                    {
                        "TRUE" => true,
                        "FALSE" => false,
                        _ => throw new FormatException()
                    };
                case var t when typeof(IEnumerable).IsAssignableFrom(t):
                {
                    var itemType = type.GetGenericArguments().Single();
                    type = typeof(List<>).MakeGenericType(itemType);

                    var resultEnumerable = (Activator.CreateInstance(type) as IList)!;

                    var itemRegex = GetRegex(type);
                    var valueMatch = itemRegex.Match(value);

                    var enumerableItems = valueMatch.Value[1..^2].Trim('[', ']').Split(',');

                    foreach (var item in enumerableItems)
                    {
                        var itemValue = DeserializeValue(itemType, item);

                        resultEnumerable.Add(itemValue);
                    }

                    return resultEnumerable;
                }

                default:
                    throw new UnsupportedTypeException(type);
            }
        }

        private static Regex GetRegex(Type type)
        {
            var pattern = type switch
            {
                var t when t == typeof(string) => StringPattern,
                var t when t == typeof(int) => IntPattern,
                var t when t == typeof(float) => FloatPattern,
                var t when t == typeof(bool) => BoolPattern,
                var t when typeof(IEnumerable).IsAssignableFrom(t) => EnumerablePattern,
                _ => throw new UnsupportedTypeException(type)
            };

            return new Regex(pattern, SearchOptions);
        }

        private static bool IsDeserializableType(Type type) =>
            IsDeserializablePrimitiveType(type)
            || IsDeserializableEnumerableType(type);

        private static bool IsDeserializablePrimitiveType(Type type) =>
            type == typeof(string)
            || type == typeof(int)
            || type == typeof(float)
            || type == typeof(bool);

        private static bool IsDeserializableEnumerableType(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            var elementType = type.GetGenericArguments().SingleOrDefault();

            return elementType != default
                && IsDeserializablePrimitiveType(elementType);
        }
    }
}