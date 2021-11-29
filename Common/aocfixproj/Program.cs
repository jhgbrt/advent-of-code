using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using System.Xml.Linq;


for (var year = 2015; year <= DateTime.Now.Year; year++)
{
    for (var day = 1; day <= 25; day++)
    {
        Console.WriteLine($"{year}/{day}");
        UpdateProject(year, day);
        UpdateProgramCs(year, day);
        UpdateNamespaces(year, day);
    }
}

static void UpdateProject(int year, int day)
{
    var csproj = Path.Combine(year.ToString(), $"Day{day:00}", $"AoC.{year}.Day{day:00}.csproj");
    if (File.Exists(csproj))
    {
        var doc = XDocument.Load(csproj);
        var propertyGroup = (
            from node in doc.Descendants()
            where node.Name == "PropertyGroup"
            select node
            ).First();

        if (!propertyGroup.Descendants().Any(p => p.Name == "RootNameSpace"))
        {
            propertyGroup.Add(new XElement("RootNameSpace", $"AdventOfCode.Year{year:0000}.Day{day:00}"));
        }

        var itemGroup = (
            from node in doc.Descendants()
            where node.Name == "ItemGroup"
            where node.Descendants().Any(n => n.Name == "Using")
            select node
            ).First();

        if (!itemGroup.Descendants().Any(n => n.Attribute("Include")?.Value == "System.Collections.Immutable"))
        {
            var @using = new XElement("Using");
            @using.SetAttributeValue("Include", "System.Collections.Immutable");
            itemGroup.Add(@using);
        }

        doc.Save(csproj);
    }

}

static void UpdateProgramCs(int year, int day)
{
    var file = Path.Combine(year.ToString(), $"Day{day:00}", $"Program.cs");

    var syntax = CSharpSyntaxTree.ParseText(File.ReadAllText(file));

    var namespaceName = $"AdventOfCode.Year{year:0000}.Day{day:00}";

    var root = syntax.GetCompilationUnitRoot();

    var collector = new FindAoCClass();
    collector.Visit(root);

    bool shouldWrite = false;

    if (collector.AoC is not null)
    {
        var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));
        root = root.ReplaceNode(collector.AoC, ns.AddMembers(collector.AoC));
        shouldWrite = true;
    }

    if (collector.UsingStaticAoC is not null)
    {
        var newusing = collector.UsingStaticAoC.WithName(SyntaxFactory.ParseName(namespaceName + ".AoC"));
        root = root.ReplaceNode(collector.UsingStaticAoC, newusing);
        shouldWrite = true;
    }

    if (shouldWrite)
    {
        var workspace = new AdhocWorkspace();
        workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(Guid.NewGuid().ToString()), VersionStamp.Default));
        Console.WriteLine($"Writing {file}");
        File.WriteAllText(file, Formatter.Format(root, workspace).ToString());
    }
}


static void UpdateNamespaces(int year, int day)
{
    var dir = new DirectoryInfo(Path.Combine(year.ToString(), $"Day{day:00}"));
    foreach (var file in dir.GetFiles("*.cs"))
    {
        if (file.Name == "Program.cs") continue;

        var syntax = CSharpSyntaxTree.ParseText(File.ReadAllText(file.FullName));

        var namespaceName = $"AdventOfCode.Year{year:0000}.Day{day:00}";

        var root = syntax.GetCompilationUnitRoot();

        var collector = new FindFileScopedNamespace();
        collector.Visit(root);

        if (!collector.HasFileScopedNamespace)
        {
            var ns = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));
            root = root.InsertNodesBefore(root.Members.First(), Enumerable.Repeat(ns, 1));
            var workspace = new AdhocWorkspace();
            workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(Guid.NewGuid().ToString()), VersionStamp.Default));
            Console.WriteLine($"Writing {file.FullName}");
            File.WriteAllText(file.FullName, Formatter.Format(root, workspace).ToString());
        }

    }

}

class FindFileScopedNamespace : CSharpSyntaxWalker
{
    public bool HasFileScopedNamespace { get;private set; }
    public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
    {
        HasFileScopedNamespace = true;
    }
}
class FindAoCClass : CSharpSyntaxWalker
{
    public UsingDirectiveSyntax UsingStaticAoC { get; private set; }
    public ClassDeclarationSyntax AoC { get; private set; }

    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        if (node.Name.ToString() == "AoC" && node.StaticKeyword != SyntaxFactory.Token(SyntaxKind.None))
        {
            UsingStaticAoC = node;
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (node.Identifier.ToString() == "AoC" && node.Parent is not NamespaceDeclarationSyntax)
        {
            AoC = node;
        }
    }
}