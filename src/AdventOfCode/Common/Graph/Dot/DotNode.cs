namespace Net.Code.Graph.Dot;

public class DotNode : DotElement
{
    public DotIdentifier? Identifier { get; set; }

    public DotColorAttribute? Color
    {
        get => GetAttribute<DotColorAttribute>("color");
        set => SetAttribute("color", value);
    }

    public DotColorAttribute? FillColor
    {
        get => GetAttribute<DotColorAttribute>("fillcolor");
        set => SetAttribute("fillcolor", value);
    }

    public DotNodeShapeAttribute? Shape
    {
        get => GetAttribute<DotNodeShapeAttribute>("shape");
        set => SetAttribute("shape", value);
    }

    public DotNodeStyleAttribute? Style
    {
        get => GetAttribute<DotNodeStyleAttribute>("style");
        set => SetAttribute("style", value);
    }

    public DotDoubleAttribute? Width
    {
        get => GetAttribute<DotDoubleAttribute>("width");
        set => SetAttribute("width", value);
    }

    public DotDoubleAttribute? Height
    {
        get => GetAttribute<DotDoubleAttribute>("height");
        set => SetAttribute("height", value);
    }

    public DotDoubleAttribute? PenWidth
    {
        get => GetAttribute<DotDoubleAttribute>("penwidth");
        set => SetAttribute("penwidth", value);
    }

    public DotPointAttribute? Pos
    {
        get => GetAttribute<DotPointAttribute>("pos");
        set => SetAttribute("pos", value);
    }
}