using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TypedRegex.Generator
{

    class RecordReceiver : ISyntaxReceiver
    {
        public IEnumerable<RecordDeclarationSyntax> CandidateRecords => Records;
        List<RecordDeclarationSyntax> Records { get; } = new();
        HashSet<string> TypesUsedForTypedRegex { get; } = new();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) 
        {
            if (syntaxNode is RecordDeclarationSyntax record)
            {
                if (!record.AttributeLists.SelectMany(a => a.Attributes).Any())
                    return;
                if (record.ParameterList is null) // only allow records with primary constructor
                    return;
                Records.Add(record);
            }
        }
    }

    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RecordReceiver());
        }

        readonly static string attributeText = @"using System;
using System.Text.RegularExpressions;

namespace System.Text.RegularExpressions.Typed
{
    public class TypedRegexAttribute : Attribute
    {
        public Regex Regex { get; }
        public TypedRegexAttribute(string pattern) => Regex = new Regex(pattern);
    }
}";
        public void Execute(GeneratorExecutionContext context)
        {
            //if (!Debugger.IsAttached) Debugger.Launch();

            context.AddSource("TypedRegexAttribute.generated.cs", attributeText);

            CSharpParseOptions options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
            Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(attributeText, Encoding.UTF8), options));

            if (context.SyntaxReceiver is not RecordReceiver receiver) return;

            var stringSymbol = compilation.GetTypeByMetadataName("System.String");
            var formatProviderSymbol = compilation.GetTypeByMetadataName("System.IFormatProvider");
            var attributeSymbol = compilation.GetTypeByMetadataName("System.Text.RegularExpressions.Typed.TypedRegexAttribute");


            var inconsistentRegex
            = from record in receiver.CandidateRecords
              let model = compilation.GetSemanticModel(record.SyntaxTree)
              let symbol = model.GetDeclaredSymbol(record)
              let constructor = symbol.Constructors.First()
              let attribute = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))
              where attribute is not null && attribute.ConstructorArguments.Any()
              let pattern = (string)attribute.ConstructorArguments.First().Value
              where !string.IsNullOrEmpty(pattern)
              let regex = new Regex(pattern)
              let groupNames = regex.GetGroupNames().Skip(1)
              let propertyNames = constructor.Parameters.Select(p => p.Name)
              let missing = propertyNames.Except(groupNames)
              where missing.Any()
              select (model, regex, missing, record, symbol);

            foreach (var item in inconsistentRegex)
            {
                var descriptor = new DiagnosticDescriptor(
                    "TRX001", 
                    "Incomplete Regex",
                    "RegEx for parsing type {0} is missing named capture groups for properties {1}", 
                    "TypedRegex", DiagnosticSeverity.Error, true);
                context.ReportDiagnostic(Diagnostic.Create(descriptor, item.record.GetLocation(), item.symbol.Name, string.Join(",", item.missing)));
            }

            var q = (
                from record in receiver.CandidateRecords
                let model = compilation.GetSemanticModel(record.SyntaxTree)
                let symbol = model.GetDeclaredSymbol(record)
                let constructor = symbol.Constructors.First()
                let attribute = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                where attribute is not null
                let pattern = (string)attribute.ConstructorArguments.First().Value 
                let regex = new Regex(pattern)
                let @namespace = symbol.ContainingNamespace.ToString()
                let typedName = $"{symbol.Name}TypedRegex"
                let parameters = (
                    from parameter in record.ParameterList.Parameters
                    let parameterSymbol = model.GetDeclaredSymbol(parameter)
                    let parameterType = parameterSymbol.Type
                    let parseMethod = (
                        from method in parameterType.GetMembers("Parse").OfType<IMethodSymbol>()
                        where (method.Parameters.Length == 2
                        && method.Parameters[0].Type.Equals(stringSymbol, SymbolEqualityComparer.Default)
                        && method.Parameters[1].Type.Equals(formatProviderSymbol, SymbolEqualityComparer.Default)
                        ) || (method.Parameters.Length == 1
                        && method.Parameters[0].Type.Equals(stringSymbol, SymbolEqualityComparer.Default)
                        )
                        select method
                    ).FirstOrDefault()
                    let parameterName = parameterSymbol.Name
                    let variableName = parameterName.ToLowerInvariant()
                    select new RecordParameterInfo(parameterName, parameterType, parseMethod, variableName)
                ).ToArray()
                select (record, pattern, regex, symbol.Name, @namespace, typedName, parameters)
            ).ToArray();

            foreach (var (record, pattern, regex, recordName, @namespace, typedName, parameters) in q)
            {
                var tree = SyntaxTree(
                    CompilationUnit()
                    .WithUsings(SingletonList(UsingDirective("System.Text.RegularExpressions".AsQualifiedName())))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(@namespace.AsQualifiedName())
                            .WithMembers(
                                    SingletonList<MemberDeclarationSyntax>(
                                        RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier(recordName))
                                        .WithModifiers(record.Modifiers)
                                        .WithClassOrStructKeyword(record.ClassOrStructKeyword)
                                        .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                                        .WithMembers(List(new MemberDeclarationSyntax[] {
                                            FieldDeclaration(
                                                VariableDeclaration(IdentifierName("Regex"))
                                                .WithVariables(
                                                    SingletonSeparatedList(
                                                        VariableDeclarator(
                                                            Identifier("_regex"))
                                                        .WithInitializer(
                                                            EqualsValueClause(
                                                                ObjectCreationExpression(
                                                                    IdentifierName("Regex"))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList(
                                                                            Argument(
                                                                                LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(pattern))
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            .WithModifiers(TokenList( new [] { Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.ReadOnlyKeyword)})),
                                            MethodDeclaration(
                                                IdentifierName(recordName),
                                                Identifier("Parse"))
                                            .WithModifiers(TokenList(new []{ Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                                            .WithParameterList(
                                                ParameterList(SingletonSeparatedList(Parameter(Identifier("s")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))))
                                                )
                                            .WithBody(
                                                Block(
                                                    RegexInvocation(recordName, parameters)
                                                )
                                            )
                                        }
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(SyntaxKind.CloseBraceToken)
                                )
                            )
                        )
                    )
                )
                .NormalizeWhitespace()
                );
                var sb = new StringBuilder();
                sb
                    .AppendLine($@"using System.Text.RegularExpressions;

namespace {@namespace}
{{
    public partial record {recordName}
    {{
        static readonly Regex _regex = new Regex(@""{regex}"");
        public static {recordName} Parse(string s)
        {{
            var match = _regex.Match(s);");
                foreach (var (parameterName, parameterType, parseMethod, variableName) in parameters)
                {
                    var fullyQualifiedName = parseMethod switch
                    {
                        not null => $"{parameterType.ContainingNamespace.Name}.{parameterType.Name}.{parseMethod.Name}",
                        _ => string.Empty
                    };

                    var assignment = parseMethod switch
                    {
                        { Parameters: { Length: 2 } } 
                            => $@"            var {variableName} = {fullyQualifiedName}(match.Groups[""{parameterName}""].Value, CultureInfo.InvariantCulture);",
                        { Parameters: { Length: 1 } } 
                            => $@"            var {variableName} = {fullyQualifiedName}(match.Groups[""{parameterName}""].Value);",
                        _ 
                            => $@"            var {variableName} = match.Groups[""{parameterName}""].Value;"
                    };
                    sb.AppendLine(assignment);
                }
                sb
                    .Append($"            return new {recordName}(")
                    .Append(string.Join(", ", parameters.Select(p => p.variableName)))
                    .AppendLine(");");
                sb.AppendLine(@$"        }}
    }}
}}");
                if (!Debugger.IsAttached)
                    Debugger.Launch();
                context.AddSource($"{recordName}.generated.cs", sb.ToString());
            }

        }

        private static IEnumerable<StatementSyntax> RegexInvocation(string recordName, RecordParameterInfo[] parameters)
        {
            yield return LocalDeclarationStatement(
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
                            Identifier("match")
                        )
                        .WithInitializer(
                            EqualsValueClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("_regex"),
                                        IdentifierName("Match")
                                    )
                                )
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                IdentifierName("s")
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            foreach (var (parameterName, parameterType, parseMethod, variableName) in parameters)
            {
                yield return LocalDeclarationStatement(
                    VariableDeclaration(IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier(parameterName)
                                )
                        .WithInitializer(
                                EqualsValueClause( // TODO call Parse method if relevant
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ElementAccessExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("match"),
                                                IdentifierName("Groups")
                                            )
                                        )
                                        .WithArgumentList(
                                            BracketedArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(parameterName)
                                                        )
                                                    )
                                                )
                                            )
                                        ),
                                        IdentifierName("Value")
                                    )
                                )
                        )
                    )
                        )
                    );
            }

            
            yield return
                                                    ReturnStatement(
                                                        ObjectCreationExpression(
                                                            IdentifierName(recordName)
                                                        )
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SeparatedList<ArgumentSyntax>(
                                                                    new SyntaxNodeOrToken[]{
                                                                        Argument(
                                                                            IdentifierName(
                                                                                Identifier(
                                                                                    TriviaList(),
                                                                                    SyntaxKind.FromKeyword,
                                                                                    "from",
                                                                                    "from",
                                                                                    TriviaList()
                                                                                )
                                                                            )
                                                                        ),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        Argument(
                                                                            IdentifierName("to")
                                                                        )
                                                                    }
                                                                )
                                                            )
                                                        )
                                                    );

        }

    }

    static class Helpers
    {
        public static NameSyntax AsQualifiedName(this string @namespace)
        {
            var parts = @namespace.Split('.').Select(s => IdentifierName(s)).ToArray();
            NameSyntax name = parts[0];
            for (int i = 1; i < parts.Length; i++)
                name = QualifiedName(name, parts[i]);
            return name;
        }

    }

    internal record struct RecordParameterInfo(string parameterName, ITypeSymbol parameterType, IMethodSymbol parseMethod, string variableName)
    {
    }
}
