using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BurpLang.Exceptions;

namespace BurpLang
{
    public class Parser<T>
        where T : notnull, new()
    {
        private readonly string _input;

        private int _i;

        public Parser(string input)
        {
            _input = input;
        }

        public T GetObject()
        {
            var result = new T();
            _i = 0;

            var properties = typeof(T)
               .GetProperties()
               .Where(p => IsDeserializableType(p.PropertyType))
               .ToArray();

            SkipWhiteSpace(); // Пропустили все пробелы до корневого объекта.

            if (GetChar(_i++) != '<')
                throw new ParsingException("Ожидалось '<'.")
                {
                    StartIndex = _i,
                    EndIndex = _i + 1
                };

            SkipWhiteSpace(); // Пропустили все пробелы после открытия объекта. Остановились на свойстве.

            while (GetChar(_i) != '>')
            {
                // Обрабатываем НАЗВАНИЕ
                var nameStart = _i;

                while (GetChar(_i) != '=' && GetChar(_i) != ' ')
                {
                    if (!char.IsLetter(GetChar(_i)))
                        throw new ParsingException("Название должно содержать только буквы.")
                        {
                            StartIndex = nameStart,
                            EndIndex = _i
                        };

                    _i++;
                }

                var propertyName = GetRange(nameStart, _i);

                var property = properties.SingleOrDefault(p => p.Name == propertyName) ??
                    throw new ParsingException($"Целевой объект не имеет свойства '{propertyName}'.")
                    {
                        StartIndex = nameStart,
                        EndIndex = _i
                    };

                SkipWhiteSpace(); // Пропустили все пробелы после названия. Остановились на '='.

                if (GetChar(_i++) != '=')
                    throw new ParsingException("Ожидалось '='.")
                    {
                        StartIndex = _i,
                        EndIndex = _i + 1
                    };

                SkipWhiteSpace(); // Пропустили все пробелы после '='. Остановились либо на значении, либо на '['.

                // Обрабатываем ПРИМИТИВ
                if (IsDeserializablePrimitiveType(property.PropertyType))
                {
                    var valueStart = _i;

                    switch (property.PropertyType)
                    {
                        case var t when t == typeof(string):
                        {
                            if (GetChar(_i++) != '\"')
                                throw new ParsingException("Строка должна начинаться с '\"'.")
                                {
                                    StartIndex = _i - 1,
                                    EndIndex = _i
                                };

                            var valueBuilder = new StringBuilder();

                            while (GetChar(_i) != '\"' && char.IsLetterOrDigit(GetChar(_i)))
                                valueBuilder.Append(GetChar(_i++));

                            if (!char.IsLetterOrDigit(GetChar(_i)) && GetChar(_i) != '\"')
                                throw new ParsingException("Строка может содержать только буквы и цифры.")
                                {
                                    StartIndex = valueStart,
                                    EndIndex = _i
                                };

                            // if (GetChar(_i) != '\"')
                            //     throw new ParsingException("Строка должна заканчиваться '\"'.");

                            var value = valueBuilder.ToString();
                            property.SetValue(result, value);

                            break;
                        }

                        case var t when t == typeof(int):
                        {
                            var valueBuilder = new StringBuilder();

                            while (char.IsDigit(GetChar(_i)))
                                valueBuilder.Append(GetChar(_i++));

                            if (!char.IsDigit(GetChar(_i)) && GetChar(_i) != ';')
                                throw new ParsingException("Целое число может содержать только цифры.")
                                {
                                    StartIndex = valueStart,
                                    EndIndex = _i
                                };

                            _i--;

                            var value = int.Parse(valueBuilder.ToString());
                            property.SetValue(result, value);

                            break;
                        }

                        case var t when t == typeof(float):
                        {
                            var valueBuilder = new StringBuilder();

                            while (GetChar(_i) != '.' && char.IsDigit(GetChar(_i)))
                                valueBuilder.Append(GetChar(_i++));

                            if (GetChar(_i++) != '.')
                                throw new ParsingException("Ожидалась цифра или точка.")
                                {
                                    StartIndex = valueStart,
                                    EndIndex = _i
                                };

                            valueBuilder.Append('.');

                            while (char.IsDigit(GetChar(_i)))
                                valueBuilder.Append(GetChar(_i++));

                            if (GetChar(_i) != ';')
                                throw new ParsingException("Ожидалась цифра.")
                                {
                                    StartIndex = valueStart,
                                    EndIndex = _i
                                };

                            _i--;

                            var value = float.Parse(valueBuilder.ToString());
                            property.SetValue(result, value);

                            break;
                        }

                        case var t when t == typeof(bool):
                        {
                            var valueBuilder = new StringBuilder();

                            while (GetChar(_i) != ';')
                                valueBuilder.Append(GetChar(_i++));

                            _i--;

                            var rawValue = valueBuilder.ToString();

                            var value = rawValue switch
                            {
                                "TRUE" => true,
                                "FALSE" => false,
                                _ => throw new ParsingException("Ожидалось TRUE или FALSE.")
                                {
                                    StartIndex = valueStart,
                                    EndIndex = _i
                                }
                            };

                            property.SetValue(result, value);

                            break;
                        }
                    }
                }
                // Обрабатываем МАССИВ
                else if (IsDeserializableEnumerableType(property.PropertyType))
                {
                    if (GetChar(_i++) != '[')
                        throw new ParsingException("Ожидалось '['.")
                        {
                            StartIndex = _i - 1,
                            EndIndex = _i
                        };

                    SkipWhiteSpace(); // Пропустили все пробелы от начала массива. Остановились либо на первом значении, либо на ']'.

                    var elementType = property.PropertyType.GetGenericArguments().Single();
                    var enumerableType = typeof(List<>).MakeGenericType(elementType);
                    var resultEnumerable = (Activator.CreateInstance(enumerableType) as IList)!;

                    while (GetChar(_i) != ']')
                    {
                        var valueStart = _i;

                        switch (elementType)
                        {
                            case var t when t == typeof(string):
                            {
                                if (GetChar(_i++) != '\"')
                                    throw new ParsingException("Строка должна начинаться с '\"'.")
                                    {
                                        StartIndex = _i - 1,
                                        EndIndex = _i
                                    };

                                var valueBuilder = new StringBuilder();

                                while (GetChar(_i) != '\"' && char.IsLetterOrDigit(GetChar(_i)))
                                    valueBuilder.Append(GetChar(_i++));

                                if (!char.IsLetterOrDigit(GetChar(_i)) && GetChar(_i) != '\"')
                                    throw new ParsingException("Строка может содержать только буквы и цифры.")
                                    {
                                        StartIndex = valueStart,
                                        EndIndex = _i
                                    };

                                var value = valueBuilder.ToString();
                                resultEnumerable.Add(value);

                                break;
                            }
                            case var t when t == typeof(int):
                            {
                                var valueBuilder = new StringBuilder();

                                while (char.IsDigit(GetChar(_i)))
                                    valueBuilder.Append(GetChar(_i++));

                                if (!char.IsDigit(GetChar(_i)) && GetChar(_i) != ',')
                                    throw new ParsingException("Целое число может содержать только цифры.")
                                    {
                                        StartIndex = valueStart,
                                        EndIndex = _i
                                    };

                                _i--;

                                var value = int.Parse(valueBuilder.ToString());
                                resultEnumerable.Add(value);

                                break;
                            }
                            case var t when t == typeof(float):
                            {
                                var valueBuilder = new StringBuilder();

                                while (GetChar(_i) != '.' && char.IsDigit(GetChar(_i)))
                                    valueBuilder.Append(GetChar(_i++));

                                if (GetChar(_i++) != '.')
                                    throw new ParsingException("Ожидалась цифра или точка.")
                                    {
                                        StartIndex = valueStart,
                                        EndIndex = _i
                                    };

                                valueBuilder.Append('.');

                                while (char.IsDigit(GetChar(_i)))
                                    valueBuilder.Append(GetChar(_i++));

                                if (GetChar(_i) != ',')
                                    throw new ParsingException("Ожидалась цифра.")
                                    {
                                        StartIndex = valueStart,
                                        EndIndex = _i
                                    };

                                var value = float.Parse(valueBuilder.ToString());
                                resultEnumerable.Add(value);

                                break;
                            }
                            case var t when t == typeof(bool):
                            {
                                var valueBuilder = new StringBuilder();

                                while (GetChar(_i) != ',')
                                    valueBuilder.Append(GetChar(_i++));

                                _i--;

                                var rawValue = valueBuilder.ToString();

                                var value = rawValue switch
                                {
                                    "TRUE" => true,
                                    "FALSE" => false,
                                    _ => throw new ParsingException("Ожидалось TRUE или FALSE.")
                                    {
                                        StartIndex = valueStart,
                                        EndIndex = _i
                                    }
                                };

                                resultEnumerable.Add(value);

                                break;
                            }
                        }

                        if (GetChar(++_i) != ',')
                            throw new ParsingException("Элемент массива должен оканчиваться ','.");

                        _i++;
                        SkipWhiteSpace(); // Пропустили все пробелы после окончания элемента массива. Остановились либо на следующем элементе, либо на ']'.
                    }

                    property.SetValue(result, resultEnumerable);
                }

                SkipWhiteSpace(); // Пропустили все пробелы после окончания свойства. Остановились на ';'.

                if (GetChar(++_i) != ';')
                    throw new ParsingException("Свойство должно заканчиваться ';'.");

                _i++;
                SkipWhiteSpace(); // Пропустили все пробелы после ';'. Остановились либо на следующем свойстве, либо на '>'.
            }

            if (GetChar(_i) != '>')
                throw new ParsingException("Ожидалось '>'.")
                {
                    StartIndex = _i,
                    EndIndex = _i + 1
                };

            return result;
        }

        private static bool IsDeserializableType(Type type) =>
            IsDeserializablePrimitiveType(type) ||
            IsDeserializableEnumerableType(type);

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

        private char GetChar(int index)
        {
            try
            {
                return _input[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParsingException("Внезапный конец строки.")
                {
                    StartIndex = _i,
                    EndIndex = _i + 1
                };
            }
        }

        private string GetRange(int start, int end)
        {
            try
            {
                return _input[start..end];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParsingException("Внезапный конец строки.")
                {
                    StartIndex = _i,
                    EndIndex = _i + 1
                };
            }
        }

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(GetChar(_i)))
                _i++;
        }
    }
}