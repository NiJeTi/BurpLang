using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using BurpLang.Exceptions;

namespace BurpLang
{
    internal static class Deserializer
    {
        private const string ObjectPattern = @"\<.*\>";
        private const string IntPattern = @"\d+";
        private const string FloatPattern = IntPattern + @"\." + IntPattern;
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

                var propertyIndex = input.IndexOf(name, StringComparison.Ordinal);

                if (propertyIndex == -1)
                    continue;

                var propertyRelatedTextBuilder = new StringBuilder(name);
                var i = propertyIndex + propertyRelatedTextBuilder.Length;
                var lastChar = propertyRelatedTextBuilder[^1];

                while (lastChar != ';')
                {
                    lastChar = input[i++];
                    propertyRelatedTextBuilder.Append(lastChar);
                }

                var propertyRelatedText = propertyRelatedTextBuilder.ToString();

                var searchPattern = $@"{name}\=(?<{name}>.*)\;";

                var parsedValue = Regex
                   .Match(propertyRelatedText, searchPattern, RegexOptions.Singleline)
                   .Groups[name].Value;

                if (string.IsNullOrEmpty(parsedValue))
                    throw new PropertyParsingException(name, searchPattern, propertyRelatedText);

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
                {
                    if (value.First() != '\"' || value.Last() != '\"')
                        throw new FormatException($"Cannot parse `{value}`. String should be in quotes.");

                    if (value[1..^1].Contains('\"'))
                        throw new FormatException($"Cannot parse `{value}`. String should not contain quotes.");

                    return value.Trim('\"');
                }
                case var t when t == typeof(int):
                {
                    if (!int.TryParse(value, out var parsed))
                        throw new FormatException($"Cannot parse `{value}`. It's not an integer.");

                    return parsed;
                }
                case var t when t == typeof(float):
                {
                    if (!Regex.IsMatch(value, FloatPattern) || !float.TryParse(value, out var parsed))
                        throw new FormatException($"Cannot parse `{value}`. It's not a float.");

                    return parsed;
                }
                case var t when t == typeof(bool):
                    return value switch
                    {
                        "TRUE" => true,
                        "FALSE" => false,
                        _ => throw new FormatException($"Cannot deserialize `{value}`. Expected `TRUE` or `FALSE`.")
                    };
                case var t when typeof(IEnumerable).IsAssignableFrom(t):
                {
                    var itemType = type.GetGenericArguments().Single();
                    type = typeof(List<>).MakeGenericType(itemType);

                    var resultEnumerable = (Activator.CreateInstance(type) as IList)!;

                    var valueMatch = Regex.Match(value, EnumerablePattern);
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