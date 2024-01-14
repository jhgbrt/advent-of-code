using System.Text;
using System.Text.RegularExpressions;

using Net.Code.Graph.Dot.Compilation;
using Net.Code.Graph.Dot.Extensions;

namespace Net.Code.Graph.Dot.Core;

public static partial class DotGraphWriter
{
    public static void WriteTo(DotGraph graph, StringBuilder sb)
    {
        using var writer = new StringWriter(sb);
        CompileAsync(graph, writer, null).Wait();
    }
    public async static Task<string> CompileAsync(DotGraph graph, CompilationOptions? options = null)
    {
        await using var writer = new StringWriter();
        await CompileAsync(graph, writer, options);
        return writer.GetStringBuilder().ToString();

    }
    public async static Task CompileAsync(DotGraph graph, TextWriter writer, CompilationOptions? options = null)
    {
        var context = new CompilationContext(writer, options ?? new CompilationOptions());
        await CompileAsync(context, graph);
    }
    async static Task CompileAsync(CompilationContext context, DotGraph graph)
    {
        context.DirectedGraph = graph.Directed;
        await context.WriteIndentationAsync();
        if (graph.Strict)
            await context.WriteAsync("strict ");
        await context.WriteAsync(graph.Directed ? "digraph " : "graph ");
        await CompileGraphBodyAsync(context, graph);
    }

    async static Task CompileGraphBodyAsync(CompilationContext context, DotBaseGraph graph)
    {
        await Compile(context, graph.Identifier);
        await context.WriteLineAsync(" {");
        context.IndentationLevel++;
        await CompileAttributesAsync(context, graph);
        foreach (var element in graph.Elements)
        {
            await context.WriteIndentationAsync();
            await (element switch
            {
                DotEdge edge => Compile(context, edge),
                DotNode node => Compile(context, node),
                DotSubgraph subgraph => Compile(context, subgraph),
                _ => throw new NotImplementedException($"No compilation method for {element}")
            });
        }
        context.IndentationLevel--;
        await context.WriteIndentationAsync();
        await context.WriteLineAsync("}");
    }
    async static Task Compile(CompilationContext context, DotSubgraph subgraph)
    {
        await context.WriteAsync("subgraph ");
        await CompileGraphBodyAsync(context, subgraph);
    }

    async static Task Compile(CompilationContext context, DotEdge edge)
    {
        if (edge.From is null || edge.To is null)
            throw new Exception("Can't compile edge with null From and/or To");

        await Compile(context, edge.From);
        await context.WriteAsync($" {(context.DirectedGraph ? "->" : "--")} ");
        await Compile(context, edge.To);
        await CompileAttributeArray(context, edge);
    }

    async static Task Compile(CompilationContext context, DotNode node)
    {
        if (node.Identifier is null) throw new NullReferenceException("Node identifier is not set");
        await Compile(context, node.Identifier);
        await CompileAttributeArray(context, node);
    }

    private static async Task CompileAttributeArray(CompilationContext context, DotElement element)
    {
        if (element.Attributes.Count != 0)
        {
            await context.WriteLineAsync(" [");
            context.IndentationLevel++;
            await CompileAttributesAsync(context, element);
            context.IndentationLevel--;
            await context.WriteIndentationAsync();
            await context.WriteLineAsync("]");
        }
        else
        {
            await context.WriteLineAsync();
        }
    }

    async static Task CompileAttributesAsync(CompilationContext context, DotElement element)
    {
        foreach (var (key,value) in element.Attributes)
        {
            await context.WriteIndentationAsync();
            await context.WriteAsync($"\"{key}\"=");
            string valueStr = value switch
            {
                DotColorAttribute color => color.ToString(),
                _ => value.ToString()
            };
            await context.WriteAsync(valueStr);
            await context.WriteLineAsync();
        }
    }

    async static Task Compile(CompilationContext context, DotIdentifier identifier)
    {

        if (identifier.IsHtml)
        {
            await context.TextWriter.WriteAsync($"<{identifier.Value}>");
            return;
        }

        var value = context.Options.AutomaticEscapedCharactersFormat
            ? identifier.Value.FormatGraphvizEscapedCharacters()
            : identifier.Value;

        if (RequiresDoubleQuotes(value))
            await context.TextWriter.WriteAsync($"\"{value}\"");
        else
            await context.TextWriter.WriteAsync($"{value}");
    }

    private static readonly string[] ReservedWords =
    [
        "graph",
        "digraph",
        "subgraph",
        "strict",
        "node",
        "edge"
    ];

    private static bool RequiresDoubleQuotes(string value) => ReservedWords.Contains(value) || !NoQuotesRequiredRegex().IsMatch(value);

    [GeneratedRegex("^([a-zA-Z\\200-\\377_][a-zA-Z\\200-\\3770-9_]*|[-]?(.[0-9]+|[0-9]+(.[0-9]+)?))$")]
    private static partial Regex NoQuotesRequiredRegex();
}