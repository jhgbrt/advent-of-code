using System.IO;
using System.Threading.Tasks;

namespace Net.Code.Graph.Dot.Compilation;

public class CompilationContext(TextWriter textWriter, CompilationOptions options)
{
    public TextWriter TextWriter { get; } = textWriter;

    public CompilationOptions Options { get; } = options;

    public int IndentationLevel { get; set; } = 0;

    public bool DirectedGraph { get; set; } = false;

    public async Task WriteIndentationAsync()
    {
        if (!Options.Indented)
            return;

        for (var i = 0; i < IndentationLevel; i++)
            await TextWriter.WriteAsync("\t");
    }

    public async Task WriteAsync(string value)
    {
        await TextWriter.WriteAsync(value);
    }

    public async Task WriteLineAsync(string value = "")
    {
        await TextWriter.WriteAsync(value);
        await TextWriter.WriteAsync(Options.Indented ? '\n' : ' ');
    }
}