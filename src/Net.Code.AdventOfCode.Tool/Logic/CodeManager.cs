
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using Net.Code.AdventOfCode.Tool.Core;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Net.Code.AdventOfCode.Tool.Logic;

class CodeManager : ICodeManager
{
    private IAoCClient client;
    private IFileSystem fileSystem;

    public CodeManager(IAoCClient client, IFileSystem fileSystem)
    {
        this.client = client;
        this.fileSystem = fileSystem;
    }

    public async Task InitializeCodeAsync(int year, int day, bool force, Action<string> progress)
    {
        var codeFolder = fileSystem.GetCodeFolder(year, day);
        var templateDir = fileSystem.GetTemplateFolder();

        if (!templateDir.Exists)
        {
            throw new FileNotFoundException($"Please provide a template folder under {templateDir} containing a file named {templateDir.Code.Name}. Use {{YYYY}} and {{DD}} as placeholders in the class name for the year and day, and provide two public methods called Part1 and Part2, accepting no arguments and returning a string");
        }

        if (codeFolder.Exists && !force)
        {
            throw new Exception($"Puzzle for {year}/{day} already initialized. Use --force to re-initialize.");
        }

        await codeFolder.CreateIfNotExists();
        var code = await templateDir.ReadCode(year, day);
        await codeFolder.WriteCode(code);
        await codeFolder.WriteSample("");
        var input = await client.GetPuzzleInputAsync(year, day);
        await codeFolder.WriteInput(input);
        await client.GetPuzzleAsync(year, day, !force);
    }

    public async Task<string> GenerateCodeAsync(int year, int day)
    {
        var dir = fileSystem.GetCodeFolder(year, day);
        var aoc = await dir.ReadCode();
        var tree = CSharpSyntaxTree.ParseText(aoc);

        (var aocclass, var _) = (
            from classdecl in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            let m =
                from m in classdecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where m.Identifier.ToString() == "Part1" || m.Identifier.ToString() == "Part2"
                && m.ParameterList.Parameters.Count() == 0
                select m.WithModifiers(TokenList())
            where m.Count() == 2
            select (classdecl, m)
            ).SingleOrDefault();

        if (aocclass is null)
        {
            throw new Exception("Could not find a class with 2 methods called Part1 and Part2");
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
            select node.WithModifiers(TokenList())
            ;

        var records = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>();

        var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(cd => cd != aocclass && !cd.Identifier.ToString().Contains("Tests"));

        var result = CompilationUnit()
            .WithMembers(List(
                fields
                .Select(f => GlobalStatement(f))
                .Concat(new[]
                {
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
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                        Identifier("sw")
                                    )
                                    .WithInitializer(
                                        EqualsValueClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("Stopwatch"),
                                                    IdentifierName("StartNew")
                                                )
                                                .WithOperatorToken(
                                                    Token(SyntaxKind.DotToken)
                                                )
                                            )
                                            .WithArgumentList(
                                                ArgumentList()
                                                .WithOpenParenToken(
                                                    Token(SyntaxKind.OpenParenToken)
                                                )
                                                .WithCloseParenToken(
                                                    Token(SyntaxKind.CloseParenToken)
                                                )
                                            )
                                        )
                                        .WithEqualsToken(
                                            Token(SyntaxKind.EqualsToken)
                                        )
                                    )
                                )
                            )
                        )
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)
                        )
                    )
                })
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
                                            EqualsValueClause(
                                                InvocationExpression(
                                                    IdentifierName(name)
                                                    )
                                                    .WithArgumentList(
                                                        ArgumentList().WithOpenParenToken(Token(SyntaxKind.OpenParenToken)).WithCloseParenToken(Token(SyntaxKind.CloseParenToken))
                                                    )
                                                )
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
                                                                Argument(IdentifierName("part2")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("sw"),
                                                                        IdentifierName("Elapsed")
                                                                    )
                                                                    .WithOperatorToken(
                                                                        Token(SyntaxKind.DotToken)
                                                                    )
                                                                )
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

    public async Task ExportCode(int year, int day, string code, string output)
    {
        var codeDir = fileSystem.GetCodeFolder(year, day);
        var outputDir = fileSystem.GetOutputFolder(output);
        var templateDir = fileSystem.GetTemplateFolder();
        await outputDir.CreateIfNotExists();
        await outputDir.WriteCode(code);
        outputDir.CopyFiles(codeDir.GetCodeFiles());
        outputDir.CopyFile(codeDir.Input);
        outputDir.CopyFile(templateDir.CsProj);
    }
}


