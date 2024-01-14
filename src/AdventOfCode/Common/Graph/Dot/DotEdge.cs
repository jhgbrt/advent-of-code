namespace Net.Code.Graph.Dot;

public class DotEdge : DotElement
{
    public DotEdge() { }
    public DotEdge(DotIdentifier from, DotIdentifier to)
    {
        From = from;
        To = to;
    }
    public DotEdge(string from, string to) : this(new DotIdentifier(from), new DotIdentifier(to)) { }
    public DotIdentifier? From { get; set; }

    public DotIdentifier? To { get; set; }

    public DotColorAttribute? Color
    {
        get => GetAttribute<DotColorAttribute>("color");
        set => SetAttribute("color", value);
    }

    public DotEdgeStyleAttribute? Style
    {
        get => GetAttribute<DotEdgeStyleAttribute>("style");
        set => SetAttribute("style", value);
    }

    public DotDoubleAttribute? PenWidth
    {
        get => GetAttribute<DotDoubleAttribute>("penwidth");
        set => SetAttribute("penwidth", value);
    }

    public DotEdgeArrowTypeAttribute? ArrowHead
    {
        get => GetAttribute<DotEdgeArrowTypeAttribute>("arrowhead");
        set => SetAttribute("arrowhead", value);
    }

    public DotEdgeArrowTypeAttribute? ArrowTail
    {
        get => GetAttribute<DotEdgeArrowTypeAttribute>("arrowtail");
        set => SetAttribute("arrowtail", value);
    }

    public DotPointAttribute? Pos
    {
        get => GetAttribute<DotPointAttribute>("pos");
        set => SetAttribute("pos", value);
    }
}