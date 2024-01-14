using Net.Code.Graph.Dot.Extensions;

using System.Globalization;

namespace Net.Code.Graph.Dot;

public record DotAttribute(string Value, bool Quoted = false)
{
    public override string ToString() => Quoted ? $"\"{Value}\"" : Value;

}

public record DotColorAttribute(string Value) : DotAttribute(Value, true)
{
    public DotColorAttribute(DotColor color) : this(color.ToHexString())
    {
    }
    public override string ToString() => base.ToString();

    public static implicit operator DotColorAttribute(DotColor value) => new DotColorAttribute(value);
    public static implicit operator DotColorAttribute(string value) => new DotColorAttribute(value);
}
public record DotDoubleAttribute(string Value) : DotAttribute(Value)
{
    public DotDoubleAttribute(double Value, string Format = "F2") : this(Value.ToString(Format, CultureInfo.InvariantCulture))
    { }

    public override string ToString() => Value;

    public static implicit operator DotDoubleAttribute(double value) => new DotDoubleAttribute(value);
}
public record DotEdgeArrowTypeAttribute(string Value) : DotAttribute(Value, true)
{
    public override string ToString() => base.ToString();
    public DotEdgeArrowTypeAttribute(DotEdgeArrowType type) : this(type.ToString().ToLowerInvariant())
    {
    }


    public static implicit operator DotEdgeArrowTypeAttribute(DotEdgeArrowType value) => new DotEdgeArrowTypeAttribute(value);
    public static implicit operator DotEdgeArrowTypeAttribute(string value) => new DotEdgeArrowTypeAttribute(value);
}
public record DotEdgeStyleAttribute(string Value) : DotAttribute(Value, true)
{
    public DotEdgeStyleAttribute(DotEdgeStyle style) : this(style.FlagsToString())
    {
    }

    public override string ToString() => base.ToString();

    public static implicit operator DotEdgeStyleAttribute(DotEdgeStyle value) => new DotEdgeStyleAttribute(value.FlagsToString());
    public static implicit operator DotEdgeStyleAttribute(string value) => new DotEdgeStyleAttribute(value);
}

public record DotLabelAttribute(string Value, bool IsHtml = false) : DotAttribute(IsHtml ? $"<{Value}>" : $"\"{Value}\"")
{

    public override string ToString() => Value;

    public static implicit operator DotLabelAttribute(string value) => new DotLabelAttribute(value);
}
public record DotNodeShapeAttribute(string Value) : DotAttribute(Value, true)
{
    public override string ToString() => base.ToString();
    public DotNodeShapeAttribute(DotNodeShape shape) : this(shape.ToString().ToLowerInvariant())
    {
    }
    public static implicit operator DotNodeShapeAttribute(DotNodeShape value) => new DotNodeShapeAttribute(value);
    public static implicit operator DotNodeShapeAttribute(string value) => new DotNodeShapeAttribute(value);
}
public record DotNodeStyleAttribute(string Value) : DotAttribute(Value, true)
{
    public override string ToString() => base.ToString();
    public DotNodeStyleAttribute(DotNodeStyle style) : this(style.FlagsToString())
    {
    }

    public static implicit operator DotNodeStyleAttribute(DotNodeStyle value) => new DotNodeStyleAttribute(value);
    public static implicit operator DotNodeStyleAttribute(string value) => new DotNodeStyleAttribute(value);
}
public record DotPointAttribute(string Value) : DotAttribute(Value)
{
    public override string ToString() => Value;
    public DotPointAttribute(DotPoint point) : this(point.ToString())
    {
    }

    public static implicit operator DotPointAttribute(DotPoint value) => new DotPointAttribute(value);
    public static implicit operator DotPointAttribute(string value) => new DotPointAttribute(value);
}
public record DotRankDirAttribute(string Value) : DotAttribute(Value)
{
    public override string ToString() => $"\"{Value}\"";
    public DotRankDirAttribute(DotRankDir rankDir) : this(rankDir.ToString())
    {
    }
    public static implicit operator DotRankDirAttribute(DotRankDir value) => new DotRankDirAttribute(value);
    public static implicit operator DotRankDirAttribute(string value) => new DotRankDirAttribute(value);
}
public record DotSubgraphStyleAttribute(string Value) : DotAttribute(Value)
{

    public DotSubgraphStyleAttribute(DotSubgraphStyle style) : this(style.FlagsToString())
    {
    }
    public override string ToString() => $"\"{Value}\"";

    public static implicit operator DotSubgraphStyleAttribute(DotSubgraphStyle value) => new DotSubgraphStyleAttribute(value);
    public static implicit operator DotSubgraphStyleAttribute(string value) => new DotSubgraphStyleAttribute(value);
}

public static class DotAttributes
{
    public static DotColorAttribute Color(DotColor value) => new(value);
    public static DotDoubleAttribute Double(double value, string format = "F2") => new(value, format);
    public static DotEdgeArrowTypeAttribute EdgeArrowType(DotEdgeArrowType value) => new(value);
    public static DotEdgeArrowTypeAttribute EdgeArrowType(string value) => new(value);
    public static DotEdgeStyleAttribute EdgeStyle(DotEdgeStyle value) => new(value);
    public static DotEdgeStyleAttribute EdgeStyle(string value) => new(value);
    public static DotLabelAttribute Label(string value, bool isHtml = false) => new(value, isHtml);
    public static DotNodeShapeAttribute NodeShape(DotNodeShape value) => new(value);
    public static DotNodeShapeAttribute NodeShape(string value) => new(value);
    public static DotNodeStyleAttribute NodeStyle(DotNodeStyle value) => new(value);
    public static DotNodeStyleAttribute NodeStyle(string value) => new(value);
    public static DotPointAttribute Point(DotPoint value) => new(value);
    public static DotPointAttribute Point(string value) => new(value);
    public static DotRankDirAttribute RankDir(DotRankDir value) => new(value);
    public static DotRankDirAttribute RankDir(string value) => new(value);
    public static DotSubgraphStyleAttribute SubgraphStyle(string value) => new(value);
    public static DotSubgraphStyleAttribute SubgraphStyle(DotSubgraphStyle value) => new(value);
}