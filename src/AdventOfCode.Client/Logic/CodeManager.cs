using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Text.Json;
using System.Xml.Linq;

using Spectre.Console;

using System.Reflection;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using AdventOfCode.Client.Commands;
using System.Text;

namespace AdventOfCode.Client.Logic;

class CodeFolder
{
    DirectoryInfo dir;
    int year;
    int day;
    public CodeFolder(int year, int day)
    {
        this.dir = GetDirectory(year, day);
        this.year = year;
        this.day = day;
    }

    public string CODE => GetFileName(year, day, "AoC.cs");
    public string INPUT => GetFileName(year, day, "input.txt");
    public string SAMPLE => GetFileName(year, day, "sample.txt");
    public string ANSWERS => GetFileName(year, day, "answers.json");

    private Task<string> ReadFile(string name) => File.ReadAllTextAsync(name);
    private Task WriteFile(string name, string content) => File.WriteAllTextAsync(name, content);

    public Task<string> ReadCode() => ReadFile(CODE);
    public Task WriteCode(string content) => WriteFile(CODE, content);
    public Task<string> ReadInput() => ReadFile(INPUT);
    public Task WriteInput(string content) => WriteFile(INPUT, content);
    public Task<string> ReadSample() => ReadFile(SAMPLE);
    public Task WriteSample(string content) => WriteFile(SAMPLE, content);
    public Task<string> ReadAnswers() => ReadFile(ANSWERS);
    public Task WriteAnswers(string content) => WriteFile(ANSWERS, content);
    public bool Exists => dir.Exists;
    public void Create() => dir.Create();
    public void Delete() => dir.Delete(true);

    internal async Task ExportTo(string code, string output)
    {
        var publishLocation = new DirectoryInfo(output);
        if (!publishLocation.Exists)
            publishLocation.Create();

        var aoccs = Path.Combine(publishLocation.FullName, "aoc.cs");
        if (File.Exists(aoccs))
            File.Delete(aoccs);
        File.WriteAllText(aoccs, code);

        foreach (var file in dir.GetFiles("*.cs").Where(f => !f.FullName.Equals(CODE, StringComparison.OrdinalIgnoreCase)))
        {
            file.CopyTo(Path.Combine(publishLocation.FullName, file.Name), true);
        }

        var inputtxt = Path.Combine(publishLocation.FullName, "input.txt");
        if (File.Exists(inputtxt)) File.Delete(inputtxt);
        File.Copy(INPUT, inputtxt);

        await Assembly.GetExecutingAssembly().GetManifestResourceStream(
            typeof(Export), 
            "aoc.csproj.txt")!.CopyToAsync(File.OpenWrite(Path.Combine(publishLocation.FullName, "aoc.csproj")
            ));
    }

    static DirectoryInfo GetDirectory(int year, int day) => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}"));
    static FileInfo GetFile(int year, int day, string fileName) => new FileInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName));
    static string GetFileName(int year, int day, string fileName) => GetFile(year, day, fileName).FullName;
}

class CodeManager
{
    AoCClient client;

    public CodeManager(AoCClient client)
    {
        this.client = client;
    }

    internal async Task InitializeCodeAsync(int year, int day, bool force)
    {
        var dir = new CodeFolder(year, day);
        if (dir.Exists && !force)
        {
            throw new Exception($"Puzzle for {year}/{day} already initialized. Use --force to re-initialize.");
        }

        if (dir.Exists) dir.Delete();

        dir.Create();

        Console.WriteLine("Writing file: AoC.cs");
        await dir.WriteCode(new[]
            {
                $"namespace AdventOfCode.Year{year}.Day{day:00};",
                "",
                $"public class AoC{year}{day:00} : AoCBase",
                "{",
                $"    static string[] input = Read.InputLines(typeof(AoC{year}{day:00}));",
                "    public override object Part1() => -1;",
                "    public override object Part2() => -1;",
                "}",
            }.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString());

        await dir.WriteSample("");
        AddEmbeddedResource(dir.SAMPLE);

        AnsiConsole.WriteLine("Retrieving puzzle input");

        var content = await client.GetPuzzleInputAsync(year, day);
        await dir.WriteInput(content);
        AddEmbeddedResource(dir.INPUT);

        AnsiConsole.WriteLine("Retrieving puzzle data");

        var puzzle = await client.GetPuzzleAsync(year, day, !force);
        var answer = puzzle.Answer;
        await dir.WriteAnswers(JsonSerializer.Serialize(answer));
        AddEmbeddedResource(dir.ANSWERS);
    }
    void AddEmbeddedResource(string path)
    {
        var csproj = "aoc.csproj";
        var doc = XDocument.Load(csproj);
        var itemGroup = (
            from node in doc.Descendants()
            where node.Name == "ItemGroup"
            select node
            ).First();

        var relativePath = path.Substring(Environment.CurrentDirectory.Length + 1);
        if (!itemGroup.Elements().Select(e => e.Attribute("Include")).Where(a => a != null && a.Value == relativePath).Any())
        {
            var embeddedResource = new XElement("EmbeddedResource");
            embeddedResource.SetAttributeValue("Include", relativePath);
            itemGroup.Add(embeddedResource);
        }
        doc.Save(csproj);
    }
    internal async Task<string> GenerateCodeAsync(int year, int day)
    {
        var dir = new CodeFolder(year, day);
        var aoc = await dir.ReadCode();
        var tree = CSharpSyntaxTree.ParseText(aoc);

        var aocclass = (
            from classdecl in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            where classdecl.DescendantNodes().OfType<SimpleBaseTypeSyntax>().Any(b => b.Type.ToString() == "AoCBase")
            select classdecl
            ).SingleOrDefault();

        if (aocclass is null)
        {
            throw new Exception("Could not find an implementation of AoCBase");
        }


        var implementations = (
            from node in aocclass.DescendantNodes().OfType<MethodDeclarationSyntax>()
            where node.Identifier.ToString() is "Part1" or "Part2"
            from arrow in node.ChildNodes().OfType<ArrowExpressionClauseSyntax>()
            from impl in arrow.ChildNodes().OfType<ExpressionSyntax>()
            select (name: node.Identifier.ToString(), impl)
            ).ToDictionary(x => x.name, x => x.impl);

        var fields =
            from node in aocclass.DescendantNodes().OfType<FieldDeclarationSyntax>()
            let fieldname = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Single().Identifier.ToString()
            select LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName(
                            Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())
                            )
                        ).WithVariables(
                            SingletonSeparatedList(
                                fieldname != "input"
                                ? node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Single()
                                : VariableDeclarator(
                                    Identifier("input")
                                    ).WithInitializer(
                                        EqualsValueClause(
                                            CreateInvocationExpression(node.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Single()
                                        )
                                    )
                                )
                            )
                        )
                    )
                ;

        var methods =
            from node in aocclass.DescendantNodes().OfType<MethodDeclarationSyntax>()
            where node.Identifier.ToString() is not ("Part1" or "Part2")
            select node.WithModifiers(TokenList())
            ;

        var records = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>();

        var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Where(cd => cd != aocclass);

        var result = CompilationUnit()
            .WithMembers(List(
                fields
                .Select(f => GlobalStatement(f))
                .Concat(new[] { "Part1", "Part2" }.Select(name =>
                    GlobalStatement(
                            LocalDeclarationStatement(
                                VariableDeclaration(
                                    IdentifierName(
                                        Identifier(
                                            TriviaList(),
                                            SyntaxKind.VarKeyword,
                                            "var",
                                            "var",
                                            TriviaList()
                                        )
                                    )
                                )
                                .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(
                                            Identifier(name.ToLower())
                                        )
                                        .WithInitializer(
                                            EqualsValueClause(implementations[name])
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ).Concat(new[]
                    {
                        GlobalStatement(
                                ExpressionStatement(
                                    InvocationExpression(
                                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Console"), IdentifierName("WriteLine"))
                                            .WithOperatorToken(Token(SyntaxKind.DotToken)
                                        )
                                    )
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(
                                                    TupleExpression(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[] {
                                                                Argument(IdentifierName("part1")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(IdentifierName("part2"))
                                                            }
                                                        )
                                                    )
                                                    .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                                                    .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))
                                                )
                                            )
                                        )
                                        .WithOpenParenToken(Token(SyntaxKind.OpenParenToken))
                                        .WithCloseParenToken(Token(SyntaxKind.CloseParenToken))
                                    )
                                )
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            )

                    }
                    )
                    .Concat(List<MemberDeclarationSyntax>(methods))
                    .Concat(List<MemberDeclarationSyntax>(records))
                    .Concat(List<MemberDeclarationSyntax>(classes))
                )
            );
        var workspace = new AdhocWorkspace();
        var code = Formatter.Format(result.NormalizeWhitespace(), workspace, workspace.Options
            .WithChangedOption(CSharpFormattingOptions.IndentBlock, true)
            ).ToString();
        return code;
    }
    private InvocationExpressionSyntax CreateInvocationExpression(MemberAccessExpressionSyntax memberAccessExpression)
    {
        if (!memberAccessExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            throw new NotSupportedException($"Can not convert expression {memberAccessExpression}");

        var methodName = memberAccessExpression.ToString() switch
        {
            "Read.InputLines" => "ReadAllLines",
            "Read.InputText" => "ReadAllText",
            _ => throw new NotSupportedException($"Can not convert expression {memberAccessExpression}")
        };

        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("File"),
                IdentifierName(methodName)
                )
            )
        .WithArgumentList(
            ArgumentList(
            SingletonSeparatedList(
                Argument(
                    LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        Literal("input.txt")
                        )
                    )
                )
            )
        );
    }

    internal async Task ExportCode(int year, int day, string code, string output)
    {
        var dir = new CodeFolder(year, day);
        await dir.ExportTo(code, output);
    }
}



