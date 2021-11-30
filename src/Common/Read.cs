namespace AdventOfCode
{
    internal class Read
    {
        public static string SampleText(Type type) => Text(type, "sample.txt");
        public static string[] SampleLines(Type type) => Lines(type, "sample.txt").ToArray();
        public static string InputText(Type type) => Text(type, "input.txt");
        public static string[] InputLines(Type type) => Lines(type, "input.txt").ToArray();
        public static StreamReader InputStream(Type type) => new StreamReader(Stream(type, "input.txt"));
        public static Answer Answers(Type type) => JsonSerializer.Deserialize<Answer>(Text(type, "answers.json")) ?? Answer.Empty;

        public static string Text(Type type, string embededResource)
        {
            var stream = Stream(type, embededResource);
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
        public static IEnumerable<string> Lines(Type type, string embeddedResource)
        {
            var stream = Stream(type, embeddedResource);
            using (var sr = new StreamReader(stream))
            {
                while (sr.Peek() >= 0)
                    yield return sr.ReadLine()!;
            }
        }

        public static Stream Stream(Type type, string embeddedResource)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(type, embeddedResource);
            if (stream == null)
                throw new InvalidOperationException($"Not found: {type.FullName}.{embeddedResource}");
            return stream;
        }
    }
}
