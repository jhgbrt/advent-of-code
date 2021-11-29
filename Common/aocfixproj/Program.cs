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
        bool shouldWrite = false;

        var propertyGroup = (
            from node in doc.Descendants()
            where node.Name == "PropertyGroup"
            select node
            ).First();

        if (!propertyGroup.Descendants().Any(p => p.Name == "RootNameSpace"))
        {
            propertyGroup.Add(new XElement("RootNameSpace", $"AdventOfCode.Year{year:0000}.Day{day:00}"));
            shouldWrite = true;
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
            shouldWrite = true;
        }

        if (shouldWrite)
        {
            Console.WriteLine($"Update {csproj}");
            doc.Save(csproj);
        }
    }

}

static void UpdateProgramCs(int year, int day)
{
    var file = Path.Combine(year.ToString(), $"Day{day:00}", $"Program.cs");

    var syntax = CSharpSyntaxTree.ParseText(File.ReadAllText(file));

    var namespaceName = $"AdventOfCode.Year{year:0000}.Day{day:00}";

    var root = syntax.GetCompilationUnitRoot();

    var collector = new ProgramCsWalker();
    collector.Visit(root);

    bool shouldWrite = false;

    var workspace = new AdhocWorkspace();
    workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(Guid.NewGuid().ToString()), VersionStamp.Default));

    var ns = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));

    if (collector.Classes.Where(c => c.Identifier.ToString() != "AoC").Any())
    {
        var usings = root.Usings;
        root = root.RemoveNodes(collector.Classes.Where(c => c.Identifier.ToString() != "AoC"), SyntaxRemoveOptions.KeepNoTrivia);

        foreach (var c in collector.Classes.Where(c => c.Identifier.ToString() != "AoC"))
        {
            var croot = SyntaxFactory.CompilationUnit()
                .WithUsings(usings)
                .AddMembers(ns, c);
            Console.WriteLine($"Create {c.Identifier}.cs");
            File.WriteAllText(Path.Combine(year.ToString(), $"Day{day:00}", $"{c.Identifier}.cs"), Formatter.Format(croot, workspace).ToString());
        }

        shouldWrite = true;
    }

    if (shouldWrite)
    {
        Console.WriteLine($"Update {file}");
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
            //File.WriteAllText(file.FullName, Formatter.Format(root, workspace).ToString());
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
class ProgramCsWalker : CSharpSyntaxWalker
{
    public List<ClassDeclarationSyntax> Classes { get; private set; } = new();

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        Classes.Add(node);
    }
}