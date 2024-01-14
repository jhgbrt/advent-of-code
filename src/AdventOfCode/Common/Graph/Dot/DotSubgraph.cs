namespace Net.Code.Graph.Dot;

public class DotSubgraph(DotIdentifier identifier) : DotBaseGraph(identifier)
{
    public DotColorAttribute? Color
    {
        get => GetAttribute<DotColorAttribute>("color");
        set => SetAttribute("color", value);
    }

    public DotSubgraphStyleAttribute? Style
    {
        get => GetAttribute<DotSubgraphStyleAttribute>("style");
        set => SetAttribute("style", value);
    }
}