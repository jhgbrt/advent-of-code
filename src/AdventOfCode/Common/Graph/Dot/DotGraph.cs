using Net.Code.Graph.Dot.Core;

namespace Net.Code.Graph.Dot;

public class DotGraph(DotIdentifier identifier) : DotBaseGraph(identifier)
{
    public bool Strict { get; set; }

    public bool Directed { get; set; }

    public override string ToString()
    {
        var sw = new StringWriter();
        DotGraphWriter.CompileAsync(this, sw).Wait();
        return sw.GetStringBuilder().ToString();
    }

}