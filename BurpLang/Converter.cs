namespace BurpLang
{
    public static class Converter
    {
        public static int IndentationSize { get; set; } = 4;

        public static string Serialize(object target) => Serializer.Serialize(target, 1) ?? string.Empty;

        public static T Deserialize<T>(string input)
            where T : notnull, new() =>
            Deserializer.Deserialize<T>(input);
    }
}