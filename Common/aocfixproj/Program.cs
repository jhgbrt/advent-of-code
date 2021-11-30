using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using System.Xml.Linq;


var aoccsproj = Path.Combine("src", "aoc.csproj");
var dir = new DirectoryInfo("src");
var doc = XDocument.Load(aoccsproj);
var itemGroup = (
    from node in doc.Descendants()
    where node.Name == "ItemGroup"
    select node
    ).First();



foreach (var f in dir.GetFiles("*.json", SearchOption.AllDirectories))
{
    //    <EmbeddedResource Include="Year2015\Day01\input.txt" />

    var embeddedResource = new XElement("EmbeddedResource");
    embeddedResource.SetAttributeValue("Include", f.FullName.Substring(dir.FullName.Length + 1));
    itemGroup.Add(embeddedResource);
}

doc.Save(aoccsproj);
return;

for (var year = 2015; year < DateTime.Now.Year; year++)
{
    for (var day = 1; day <= 25; day++)
    {
        //Console.WriteLine($"{year}/{day}");
        //UpdateProject(year, day);
        //UpdateProgramCs(year, day);
        //UpdateNamespaces(year, day);
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

        if (!itemGroup.Descendants().Any(n => n.Attribute("Include")?.Value == "System.Diagnostics"))
        {
            var @using = new XElement("Using");
            @using.SetAttributeValue("Include", "System.Diagnostics");
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

    var usings = root.Usings;


    //foreach (var c in collector.Classes)
    //    Console.WriteLine($"{file}: {c.Identifier}");
    //foreach (var c in collector.Records)
    //    Console.WriteLine($"{file}: {c.Identifier}");

    var aocimpl = Path.Combine(year.ToString(), $"Day{day:00}", $"AoC.Impl.cs");

    if (collector.Enums.Any() && File.Exists(aocimpl))
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(aocimpl));
        var croot = tree.GetCompilationUnitRoot();
        foreach (var c in collector.Enums)
            croot = croot.AddMembers(c);
        Console.WriteLine($"Update AoC.Impl.cs");
        File.WriteAllText(aocimpl, Formatter.Format(croot, workspace).ToString());
        Console.WriteLine($"Update {file}");
        File.WriteAllText(file, Formatter.Format(root.RemoveNodes(collector.Enums, SyntaxRemoveOptions.KeepNoTrivia), workspace).ToString());
    }
    if (collector.Classes.Any() && File.Exists(aocimpl))
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(aocimpl));
        var croot = tree.GetCompilationUnitRoot();
        foreach (var c in collector.Classes)
            croot = croot.AddMembers(c);
        Console.WriteLine($"Update AoC.Impl.cs");
        File.WriteAllText(aocimpl, Formatter.Format(croot, workspace).ToString());
        Console.WriteLine($"Update {file}");
        File.WriteAllText(file, Formatter.Format(root.RemoveNodes(collector.Classes, SyntaxRemoveOptions.KeepNoTrivia), workspace).ToString());
    }
    if (collector.Interfaces.Any() && File.Exists(aocimpl))
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(aocimpl));
        var croot = tree.GetCompilationUnitRoot();
        foreach (var c in collector.Interfaces)
            croot = croot.AddMembers(c);
        Console.WriteLine($"Update AoC.Impl.cs");
        File.WriteAllText(aocimpl, Formatter.Format(croot, workspace).ToString());
        Console.WriteLine($"Update {file}");
        File.WriteAllText(file, Formatter.Format(root.RemoveNodes(collector.Interfaces, SyntaxRemoveOptions.KeepNoTrivia), workspace).ToString());
    }

    File.WriteAllLines(file, new[]
    {
        @$"using static AdventOfCode.Year{year}.Day{day:00}.AoC;",
        "Console.WriteLine(Part1());",
        "Console.WriteLine(Part2());"
    });

    //if (collector.Classes.Count == 1 && collector.Namespaces.Count == 1)
    //{
    //    var croot = SyntaxFactory.CompilationUnit()
    //        .WithUsings(usings)
    //        .AddMembers(ns, collector.Classes[0]);
    //    Console.WriteLine($"Create AoC.Impl.cs");
    //    File.WriteAllText(Path.Combine(year.ToString(), $"Day{day:00}", $"AoC.Impl.cs"), Formatter.Format(croot, workspace).ToString());
    //    Console.WriteLine($"Update {file}");
    //    File.WriteAllText(file, Formatter.Format(root.RemoveNode(collector.Namespaces[0], SyntaxRemoveOptions.KeepNoTrivia), workspace).ToString());
    //}
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
    public List<RecordDeclarationSyntax> Records { get; private set; } = new();
    public List<EnumDeclarationSyntax> Enums { get; private set; } = new();
    public List<InterfaceDeclarationSyntax> Interfaces { get; private set; } = new();


    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        Classes.Add(node);
    }
    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        Records.Add(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        Enums.Add(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        Interfaces.Add(node);
    }
}