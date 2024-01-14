namespace Net.Code.Graph.Dot;

public record struct DotPoint(int X, int Y, int? Z = null, bool @Fixed = false)
{
    public override string ToString() => Z.HasValue ?
            $"{X},{Y},{Z}{(Fixed ? "!" : "")}"
            : $"{X},{Y}{(Fixed ? "!" : "")}";
}