using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AdventOfCode.Client;

class ExportPuzzle
{
    public record Options(int? year, int? day, string? output = null);
    public async Task Run(Options options)
    {
        (var year, var day, var output) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day, options.output);
        var dir = AoCLogic.GetDirectory(year, day);

        Console.WriteLine($"Exporting puzzle: {year}/{day} to {dir.FullName}");

        var publishLocation = new DirectoryInfo(output ?? "publish");
        if (!publishLocation.Exists) publishLocation.Create();

        var aoc = await File.ReadAllTextAsync(Path.Combine(dir.FullName, "AoC.cs"));
        var tree = CSharpSyntaxTree.ParseText(aoc);

        var implementations = (
            from node in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
            where node.Identifier.ToString().StartsWith("Part")
            from arrow in node.ChildNodes().OfType<ArrowExpressionClauseSyntax>()
            from impl in arrow.ChildNodes().OfType<MemberAccessExpressionSyntax>()
            select (name: node.Identifier.ToString(), impl)
            ).ToDictionary(x => x.name, x => x.impl);

        var fields = (
            from node in tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>()
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
                );

        var records = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>();

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
                    ).Concat(List<MemberDeclarationSyntax>(records))
                )
            );

        var aoccs = Path.Combine(publishLocation.FullName, "aoc.cs");
        if (File.Exists(aoccs)) File.Delete(aoccs);
        var workspace = new AdhocWorkspace();
        File.WriteAllText(aoccs, Formatter.Format(result.NormalizeWhitespace(), workspace, workspace.Options
            .WithChangedOption(CSharpFormattingOptions.IndentBlock, true)
            ).ToString());

        foreach (var file in dir.GetFiles("*.cs").Where(f => !f.Name.ToLower().Equals("aoc.cs")))
        {
            file.CopyTo(Path.Combine(publishLocation.FullName, file.Name), true);
        }

        var inputtxt = Path.Combine(publishLocation.FullName, "input.txt");
        if (File.Exists(inputtxt)) File.Delete(inputtxt);
        File.Copy(Path.Combine(dir.FullName, "input.txt"), inputtxt);


        await File.WriteAllTextAsync(Path.Combine(publishLocation.FullName, "aoc.csproj"), @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Using Include=""Microsoft.FSharp.Collections"" />
    <Using Include=""System.Diagnostics"" />
    <Using Include=""System.Reflection"" />
    <Using Include=""System.Text"" />
    <Using Include=""System.Text.Json"" />
    <Using Include=""System.Text.RegularExpressions"" />
    <Using Include=""System.Collections.Immutable"" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include=""FSharp.Core"" Version=""6.0.1"" />
  </ItemGroup>
</Project>");

    }

    private InvocationExpressionSyntax CreateInvocationExpression(MemberAccessExpressionSyntax memberAccessExpression)
    {
        if (!memberAccessExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            throw new NotSupportedException($"Can not convert expression {memberAccessExpression}");

        var identifier2 = memberAccessExpression.ToString() switch
        {
            "Read.InputLines" => "ReadAllLines",
            "Read.InputText" => "ReadAllText",
            _ => throw new NotSupportedException($"Can not convert expression {memberAccessExpression}")
        };

        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("File"),
                IdentifierName(identifier2)
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
}



