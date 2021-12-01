using AdventOfCode.Common;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Client;

class PublishPuzzle
{
    public record Options(int year, int day, string? output = null);
    public async Task Run(Options options)
    {
        (var year, var day, var output) = options;
        var dir = AoCLogic.GetDirectory(year, day);

        Console.WriteLine($"Publishing puzzle: {year}/{day}");

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
            select SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(
                            SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), SyntaxKind.VarKeyword, "var", "var", SyntaxFactory.TriviaList())
                            )
                        ).WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                fieldname != "input" 
                                ? node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Single()
                                : SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("input")).WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        CreateInvocationExpression(node.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Single())
                                    ) 
                                )
                            )
                        )
                    )
            );

        var result = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(
                fields
                .Select(f => SyntaxFactory.GlobalStatement(f))
                .Concat(new[] { "Part1", "Part2" }.Select(name => 
                    SyntaxFactory.GlobalStatement(
                            SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(
                                    SyntaxFactory.IdentifierName(
                                        SyntaxFactory.Identifier(
                                            SyntaxFactory.TriviaList(),
                                            SyntaxKind.VarKeyword,
                                            "var",
                                            "var",
                                            SyntaxFactory.TriviaList()
                                        )
                                    )
                                )
                                .WithVariables(
                                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier(name.ToLower())
                                        )
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(implementations[name])
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ).Concat(new[]
                    {
                        SyntaxFactory.GlobalStatement(
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory
                                            .MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("Console"), SyntaxFactory.IdentifierName("WriteLine"))
                                            .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken)
                                        )
                                    )
                                    .WithArgumentList(
                                        SyntaxFactory.ArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.TupleExpression(
                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[] {
                                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("part1")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("part2"))
                                                            }
                                                        )
                                                    )
                                                    .WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                                    .WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken))
                                                )
                                            )
                                        )
                                        .WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                        .WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken))
                                    )
                                )
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                            )

                    }
                    )
                )
            );

        File.WriteAllText(Path.Combine(publishLocation.FullName, $"aoc.cs"), result.NormalizeWhitespace().ToString());

        foreach (var file in dir.GetFiles("*.cs").Where(f => f.Name != "AoC.cs"))
        {
            file.CopyTo(Path.Combine(publishLocation.FullName, file.Name));
        }
        File.Copy(Path.Combine(dir.FullName, "input.txt"), Path.Combine(publishLocation.FullName, "input.txt"));


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

        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("File"),
                SyntaxFactory.IdentifierName(identifier2)
                )
            )
        .WithArgumentList(
            SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal("input.txt")
                        )
                    )
                )
            )
        );
    }
}



