using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Text.Json;

using System.Reflection;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.IO.File;
using static System.IO.Path;
using static System.Environment;
using AdventOfCode.Client.Commands;

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

    private Task<string> ReadFile(string name) => ReadAllTextAsync(name);
    private Task WriteFile(string name, string content) => WriteAllTextAsync(name, content);

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

        var aoccs = Combine(publishLocation.FullName, "aoc.cs");
        if (File.Exists(aoccs))
            File.Delete(aoccs);
        await WriteAllTextAsync(aoccs, code);

        foreach (var file in dir.GetFiles("*.cs").Where(f => !f.FullName.Equals(CODE, StringComparison.OrdinalIgnoreCase)))
        {
            file.CopyTo(Combine(publishLocation.FullName, file.Name), true);
        }

        var inputtxt = Combine(publishLocation.FullName, "input.txt");
        if (File.Exists(inputtxt)) File.Delete(inputtxt);
        Copy(INPUT, inputtxt);

        Copy(Combine(CurrentDirectory, "Template", "aoc.csproj"), Combine(publishLocation.FullName, "aoc.csproj"));
    }

    static DirectoryInfo GetDirectory(int year, int day) => new(Combine(CurrentDirectory, $"Year{year}", $"Day{day:00}"));
    static FileInfo GetFile(int year, int day, string fileName) => new(Combine(CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName));
    static string GetFileName(int year, int day, string fileName) => GetFile(year, day, fileName).FullName;
}

class CodeManager
{
    AoCClient client;

    public CodeManager(AoCClient client)
    {
        this.client = client;
    }

    internal async Task InitializeCodeAsync(int year, int day, bool force, Action<string> progress)
    {
        var templateFile = Combine(CurrentDirectory, "Template", "aoc.cs");

        if (!Exists(templateFile))
            throw new FileNotFoundException("Please provide a template file under Common\\Template\\aoc.cs. Use YYYY and DD as placeholders in the class name for the year and day, and provide two public methods called Part1 and Part2, accepting no arguments and returning a string");

        var dir = new CodeFolder(year, day);
        if (dir.Exists && !force)
        {
            throw new Exception($"Puzzle for {year}/{day} already initialized. Use --force to re-initialize.");
        }

        if (dir.Exists) dir.Delete();

        dir.Create();

        progress("Writing file: AoC.cs");

        var code = (await ReadAllTextAsync(templateFile)).Replace("YYYY", year.ToString()).Replace("DD", day.ToString("00"));

        await dir.WriteCode(code);

        await dir.WriteSample("");

        progress("Retrieving puzzle input");

        var content = await client.GetPuzzleInputAsync(year, day);
        await dir.WriteInput(content);

        progress("Retrieving puzzle data");

        var puzzle = await client.GetPuzzleAsync(year, day, !force);
        var answer = puzzle.Answer;
        await dir.WriteAnswers(JsonSerializer.Serialize(answer));
    }

    internal async Task<string> GenerateCodeAsync(int year, int day)
    {
        var dir = new CodeFolder(year, day);
        var aoc = await dir.ReadCode();
        var tree = CSharpSyntaxTree.ParseText(aoc);

        (var aocclass, var _) = (
            from classdecl in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            let m = 
                from m in classdecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where m.Identifier.ToString() == "Part1" || m.Identifier.ToString() == "Part2"
                && m.ParameterList.Parameters.Count() == 0
                select m.WithModifiers(TokenList())
            where m.Count()== 2 
            select (classdecl, m)
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

        var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(cd => cd != aocclass && !cd.Identifier.ToString().Contains("Tests"));

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



