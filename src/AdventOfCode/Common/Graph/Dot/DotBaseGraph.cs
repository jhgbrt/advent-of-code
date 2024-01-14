using System.Collections.Generic;
using System.Linq;

namespace Net.Code.Graph.Dot;

public abstract class DotBaseGraph(DotIdentifier identifier) : DotElement
{
    public DotIdentifier Identifier => identifier;

    public DotRankDirAttribute? RankDir
    {
        get => GetAttribute<DotRankDirAttribute>("rankdir");
        set => SetAttribute("rankdir", value);
    }

    public DotNode? GetNodeByIdentifier(string identifier, bool isHtml = false)
    {
        return Elements
            .OfType<DotNode>()
            .FirstOrDefault(node => node.Identifier == new DotIdentifier(identifier, isHtml));
    }

    public List<DotElement> Elements { get; } = new List<DotElement>();

}