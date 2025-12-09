using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;

using Net.Code.AdventOfCode.Toolkit.Core;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Net.Code.AdventOfCode.Toolkit.Logic;

class CodeManager(IFileSystemFactory fileSystem, ILogger<CodeManager> logger) : ICodeManager
{
    public bool IsInitialized(Puzzle puzzle)
    {
        var codeFolder = fileSystem.GetCodeFolder(puzzle.Key);
        return codeFolder.Exists;
    }
    public async Task InitializeCodeAsync(Puzzle puzzle, bool force, string? template, Action<string> progress)
    {
        var codeFolder = fileSystem.GetCodeFolder(puzzle.Key);
        var templateDir = fileSystem.GetTemplateFolder(template);

        if (!templateDir.Exists)
        {
            throw new AoCException($"Template folder for {template??"default"} template not found.");
        }

        if (codeFolder.Exists && !force)
        {
            throw new AoCException($"Puzzle for {puzzle.Key} already initialized. Use --force to re-initialize.");
        }


        await codeFolder.CreateIfNotExists();

        var code = await templateDir.ReadCode(puzzle.Key);
        await codeFolder.WriteCode(code);
        await codeFolder.WriteSample(puzzle.Example);
        await codeFolder.WriteInput(puzzle.Input);
        if (templateDir.Notebook.Exists)
        {
            codeFolder.CopyFile(templateDir.Notebook);
        }
    }

    public async Task SyncPuzzleAsync(Puzzle puzzle)
    {
        var codeFolder = fileSystem.GetCodeFolder(puzzle.Key);
        await codeFolder.WriteInput(puzzle.Input);
    }

    public async Task<string> GenerateCodeAsync(PuzzleKey key)
    {
        var dir = fileSystem.GetCodeFolder(key);
        var aoc = await dir.ReadCode(); 
        return ConvertAoCClassToTopLevelStatements(aoc);
    }

    public string ConvertAoCClassToTopLevelStatements(string aoc)
    {

        var tree = CSharpSyntaxTree.ParseText(aoc);
        tree = tree.WithRootAndOptions(tree.GetRoot(), tree.Options);

        // find a class with 2 methods without arguments called Part1() and Part2()
        // the members of this class will be converted to top level statements
        (var aocclass, var _) = (
            from classdecl in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            let m =
                from m in classdecl.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where m.Identifier.ToString() is "Part1" or "Part2"
                && m.ParameterList.Parameters.Count == 0
                select m.WithModifiers(TokenList())
            where m.Count() == 2
            select (classdecl, m)
            ).SingleOrDefault();

        if (aocclass is null)
        {
            throw new Exception("Could not find a class with 2 methods called Part1 and Part2");
        }


        aocclass = AdjustInputReading(aocclass);

        var constructors = (from c in aocclass.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                            select c).ToArray();
        var classparameters = aocclass.ParameterList;

        IEnumerable<StatementSyntax> initialization = Array.Empty<StatementSyntax>();

        if (constructors.Length > 0 )
        {

            var initializerArguments = (
                from c in constructors
                where c.ParameterList.Parameters.Count == 0
                let initializer = c.DescendantNodes().OfType<ConstructorInitializerSyntax>().FirstOrDefault()
                where initializer is not null
                let a = initializer.ArgumentList.Arguments
                select a).FirstOrDefault();

            var constructor = (
                from c in constructors
                where c.ParameterList.Parameters.Count == initializerArguments.Count
                select c
                ).SingleOrDefault();

            var parameterList = constructor?.ParameterList ?? aocclass.ParameterList;

            if (parameterList is null) throw new InvalidOperationException("Could not convert AOC class for export");

            initialization = (
                from item in initializerArguments.Zip(parameterList.Parameters)
                let value = item.First
                let name = item.Second.Identifier.Value
                select ParseStatement($"var {name} = {value};")
             ).Concat(
                constructor is not null ?
                    from statement in constructor.DescendantNodes().OfType<BlockSyntax>().First().ChildNodes().OfType<StatementSyntax>()
                    where !IsSimpleThisAssignment(statement)
                    select ConvertConstructorInitializationStatement(statement)
                : Array.Empty<StatementSyntax>()
             ).ToArray();
        }

        // the actual methods: Part1 & Part2
        var implementations = (
            from node in aocclass.DescendantNodes().OfType<MethodDeclarationSyntax>()
            where node.Parent == aocclass
            where node.Identifier.ToString() is "Part1" or "Part2"
            && node.ParameterList.Parameters.Count == 0
            from arrow in node.ChildNodes().OfType<ArrowExpressionClauseSyntax>()
            from impl in arrow.ChildNodes().OfType<ExpressionSyntax>()
            select (name: node.Identifier.ToString(), impl)
            ).ToDictionary(x => x.name, x => x.impl);

        // all fields are converted to local declarations
        // the initialization of the input variable is converted to the corresponding System.IO.File call
        var fields =
            from node in aocclass.DescendantNodes().OfType<FieldDeclarationSyntax>()
            where node.Parent == aocclass
            && !IsInitialized(node, initialization)
            select ToLocalDeclaration(node);


        Debugger.Break();

        // methods from the AoC class are converted to top-level methods
        var methods =
            from node in aocclass.DescendantNodes().OfType<MethodDeclarationSyntax>()
            where node.Parent == aocclass
            where !node.AttributeLists.Any(al => !al.Attributes.Any(a => a.Name.ToString() is "Fact" or "Theory"))
            && !(implementations.ContainsKey(node.Identifier.Text) && node.ParameterList.Parameters.Count == 0)
            select node.WithModifiers(TokenList())
            ;

        // collect usings, records, enums and other classes
        var usings = tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();

        var records = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>();

        var enums = tree.GetRoot().DescendantNodes().OfType<EnumDeclarationSyntax>();

        var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(cd => cd.Identifier.Value != aocclass.Identifier.Value && !cd.Identifier.ToString().Contains("Tests"));

        var structs = tree.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>();
        var interfaces = tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();


        // build new compilation unit:
        // - usings
        // - top level variables
        // - top level statements
        // - top level methods
        // - records, classes, enums

        var result = CompilationUnit()
            .WithUsings(List(usings))
            .WithMembers(
                List(Enumerable.Empty<GlobalStatementSyntax>()
                    .Concat([GlobalStatement(ParseStatement("var (sw, bytes) = (Stopwatch.StartNew(), 0L);"))!])
                    .Concat([GlobalStatement(ParseStatement("var filename = args switch { [\"sample\"] => \"sample.txt\", _ => \"input.txt\" };"))])
                    .Concat(initialization.Select(GlobalStatement))
                    .Concat(fields.Select(GlobalStatement))
                    .Concat(
                    [
                        GlobalStatement(ParseStatement("Report(0, \"\", sw, ref bytes);"))!,
                        GenerateGlobalStatement(1, implementations),
                        GlobalStatement(ParseStatement("Report(1, part1, sw, ref bytes);"))!,
                        
                        GenerateGlobalStatement(2, implementations),
                        GlobalStatement(ParseStatement("Report(2, part2, sw, ref bytes);"))!,
                    ])
                    .Concat(List(methods.Select(m => GlobalStatement(TransformToLocalFunctionStatement(m)))))
                    .Concat([
                        GlobalStatement(ParseStatement("""
                        void Report<T>(int part, T value, Stopwatch sw, ref long bytes)
                        {
                            var label = part switch 
                            {
                                1 => $"Part 1: [{value}]",
                                2 => $"Part 2: [{value}]",
                                _ => "Init"
                            };

                            var time = sw.Elapsed switch 
                            {
                                { TotalMicroseconds: < 1 } => $"{sw.Elapsed.TotalNanoseconds:N0} ns",
                                { TotalMilliseconds: < 1 } => $"{sw.Elapsed.TotalMicroseconds:N0} µs",
                                { TotalSeconds: < 1 } => $"{sw.Elapsed.TotalMilliseconds:N0} ms",
                                _ => $"{sw.Elapsed.TotalSeconds:N2} s"
                            };

                            var newbytes = GC.GetTotalAllocatedBytes(false);

                            var memory = (newbytes - bytes) switch
                            {
                                < 1024 => $"{newbytes - bytes} B",
                                < 1024 * 1024 => $"{(newbytes - bytes) / 1024:N0} KB",
                                _ => $"{(newbytes - bytes) / (1024 * 1024):N0} MB"
                            };

                            Console.WriteLine($"{label} ({time} - {memory})");
                            bytes = newbytes;
                        }
                        """))
                    ])
                    .Concat(List<MemberDeclarationSyntax>(interfaces))
                    .Concat(List<MemberDeclarationSyntax>(records))
                    .Concat(List<MemberDeclarationSyntax>(structs))
                    .Concat(List<MemberDeclarationSyntax>(classes))
                    .Concat(List<MemberDeclarationSyntax>(enums))
                )
            );

        result = CompileAndFix(result);

        var workspace = new AdhocWorkspace();
        var code = Formatter.Format(result.NormalizeWhitespace(), workspace, workspace.Options
            .WithChangedOption(CSharpFormattingOptions.IndentBlock, true)
            ).ToString();
        return code;
    }

    private CompilationUnitSyntax CompileAndFix(CompilationUnitSyntax unit)
    {
        const int maxAttempts = 5;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var (diagnostics, _) = CSharpSingleFileHelper.Verify(unit);
            var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();

            if (errors.Length == 0)
                return unit;

            if (!TryApplyHeuristicFixes(errors, ref unit))
            {
                foreach (var error in errors)
                    logger.LogError($"Unfixable: {error.Id}: {error.GetMessage()}");

                return unit;
            }
        }

        return unit;
    }

    private bool TryApplyHeuristicFixes(IEnumerable<Diagnostic> diagnostics, ref CompilationUnitSyntax unit)
    {
        var changed = false;

        foreach (var diagnostic in diagnostics)
        {
            if (TryApplyMessageBasedFix(diagnostic, ref unit))
            {
                changed = true;
                continue;
            }

            if (SymbolFixDiagnosticIds.Contains(diagnostic.Id) && TryApplySymbolBasedFix(diagnostic, ref unit))
            {
                changed = true;
            }
        }

        return changed;
    }

    private bool TryApplyMessageBasedFix(Diagnostic diagnostic, ref CompilationUnitSyntax unit)
    {
        var message = diagnostic.GetMessage();

        foreach (var (fragment, usingDirective) in MessageBasedUsingFixes)
        {
            if (message.Contains(fragment, StringComparison.Ordinal) && TryAddUsingDirective(ref unit, usingDirective))
                return true;
        }

        return false;
    }

    private bool TryApplySymbolBasedFix(Diagnostic diagnostic, ref CompilationUnitSyntax unit)
    {
        var symbolName = ExtractIdentifierName(diagnostic);
        if (symbolName is null)
            return false;

        return SymbolNameUsingFixes.TryGetValue(symbolName, out var usingDirective)
            && TryAddUsingDirective(ref unit, usingDirective);
    }

    private static string? ExtractIdentifierName(Diagnostic diagnostic)
    {
        var tree = diagnostic.Location.SourceTree;
        if (tree is null)
            return null;

        var node = tree.GetRoot().FindNode(diagnostic.Location.SourceSpan);
        return node switch
        {
            IdentifierNameSyntax ident => ident.Identifier.ValueText,
            GenericNameSyntax gen => gen.Identifier.ValueText,
            MemberAccessExpressionSyntax member when member.Name is IdentifierNameSyntax id => id.Identifier.ValueText,
            _ => null
        };
    }

    private static bool TryAddUsingDirective(ref CompilationUnitSyntax unit, string usingDirective)
    {
        if (unit.Usings.Any(u => u.ToString().Equals(usingDirective, StringComparison.Ordinal)))
            return false;

        var parsed = SyntaxFactory.ParseCompilationUnit(usingDirective + "\n").Usings.First();
        unit = unit.AddUsings(parsed);
        return true;
    }

    private static readonly (string fragment, string usingDirective)[] MessageBasedUsingFixes =
    [
        ("The type or namespace name 'HashSet<>' could not be found", "using System.Collections.Generic;"),
        ("The type or namespace name 'Dictionary<,>' could not be found", "using System.Collections.Generic;"),
        ("The type or namespace name 'IEnumerable<>' could not be found", "using System.Collections.Generic;"),
        ("Non-invocable member 'Range' cannot be used like a method.", "using static System.Linq.Enumerable;")
    ];

    private static readonly Dictionary<string, string> SymbolNameUsingFixes = new(StringComparer.Ordinal)
    {
        ["Stopwatch"] = "using System.Diagnostics;",
        ["StringBuilder"] = "using System.Text;",
        ["Regex"] = "using System.Text.RegularExpressions;",
        ["Min"] = "using static System.Math;",
        ["Max"] = "using static System.Math;",
        ["Pow"] = "using static System.Math;",
        ["Sqrt"] = "using static System.Math;",
        ["Log"] = "using static System.Math;",
        ["Abs"] = "using static System.Math;",
        ["ToImmutableArray"] = "using System.Collections.Immutable;"
    };

    private static readonly HashSet<string> SymbolFixDiagnosticIds = new(StringComparer.Ordinal)
    {
        "CS0103",
        "CS0246",
        "CS1061"
    };
    
    private bool IsInitialized(FieldDeclarationSyntax node, IEnumerable<StatementSyntax> initialization)
    {
        return initialization.OfType<LocalDeclarationStatementSyntax>().Any(ld => IsInitializationFor(ld, node));
    }

    private bool IsInitializationFor(LocalDeclarationStatementSyntax ld, FieldDeclarationSyntax node)
    {
        if (ld.ChildNodes().First() is not VariableDeclarationSyntax child) return false;
        if (node.Declaration.Variables.Count != 1) return false;
        return ld.DescendantNodes().OfType<VariableDeclaratorSyntax>().Any(n => n.Identifier.Text == node.Declaration.Variables.Single().Identifier.Text);
    }

    private bool IsSimpleThisAssignment(StatementSyntax statement)
    {
        if (statement is not ExpressionStatementSyntax ess) return false;
        var child = ess.ChildNodes().Single();
        if (child is not AssignmentExpressionSyntax assignment) return false;
        if (assignment.Left is not MemberAccessExpressionSyntax left) return false;
        if (assignment.Right is not IdentifierNameSyntax right) return false;
        return left.Name.ToString().Equals(right.ToString());
    }

    // Converts 'a = b' or 'this.a = b' to 'var a = b'
    private StatementSyntax ConvertConstructorInitializationStatement(StatementSyntax node)
    {
        if (node is not ExpressionStatementSyntax ess) return node;
        var child = ess.ChildNodes().Single();
        if (child is not AssignmentExpressionSyntax assignment) return node;
        
        IdentifierNameSyntax identifierName;
        if (assignment.Left is IdentifierNameSyntax idn)
        {
            identifierName = idn;
        }
        else if (assignment.Left is MemberAccessExpressionSyntax maes && maes.Expression is ThisExpressionSyntax)
        {
            identifierName = (IdentifierNameSyntax)maes.Name;
        }
        else
        {
            return node;
        }
        
        var value = assignment.Right;
        var variableDeclarator = VariableDeclarator(identifierName.Identifier)
            .WithInitializer(EqualsValueClause(value));
        var variableDeclaration = VariableDeclaration(IdentifierName("var").WithoutTrivia())
            .WithVariables(SingletonSeparatedList(variableDeclarator.WithoutTrivia()));
        return LocalDeclarationStatement(variableDeclaration).WithoutTrivia();        
    }
    private LocalDeclarationStatementSyntax ToLocalDeclaration(FieldDeclarationSyntax node)
    {
        var type = node.Declaration.Type;

        var implicitObjectCreationExpressions =
            from v in node.Declaration.Variables
            where v.Initializer is not null && v.Initializer.Value is ImplicitObjectCreationExpressionSyntax
            select (ImplicitObjectCreationExpressionSyntax)v.Initializer!.Value;

        foreach (var n in implicitObjectCreationExpressions)
        {
            node = node.ReplaceNode(n, ObjectCreationExpression(type).WithArgumentList(n.ArgumentList));
        }

        return LocalDeclarationStatement(
                            VariableDeclaration(type
                                ).WithVariables(
                                    SingletonSeparatedList(
                                        node.DescendantNodes().OfType<VariableDeclaratorSyntax>().Single()
                                    )
                                )
                            );
    }


    private static ClassDeclarationSyntax AdjustInputReading(ClassDeclarationSyntax aocclass)
    {
        var invocations = from i in aocclass.DescendantNodes().OfType<InvocationExpressionSyntax>()
                          where i.Expression is MemberAccessExpressionSyntax
                          let m = (MemberAccessExpressionSyntax)i.Expression
                          where m.Expression is IdentifierNameSyntax
                          let l = (IdentifierNameSyntax)m.Expression
                          where l.Identifier.Value is "Read"
                          let r = m.Name.Identifier.Value
                          where r is "InputLines" or "InputText" or "InputStream" or "SampleText" or "SampleLines" or "SampleStream"
                          select (i, r);

        foreach (var (invocation, right) in invocations)
        {
            aocclass = aocclass
                .ReplaceNode(invocation, InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("File"),
                        IdentifierName(right switch
                        {
                            "InputText" or "SampleText" => "ReadAllText",
                            "InputLines" or "SampleLines" => "ReadAllLines",
                            "InputStream" or "SampleStream" => "OpenRead",
                            _ => throw new NotSupportedException("Can not convert this call")
                        })
                        )
                    )
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                IdentifierName("filename")
                                )
                            )
                        )
                    )
                );

        }
        return aocclass;
    }

    private static LocalFunctionStatementSyntax TransformToLocalFunctionStatement(MethodDeclarationSyntax method)
    {
        var localFunction = LocalFunctionStatement(
            TokenList(method.Modifiers.Where(m => !m.IsKind(SyntaxKind.PublicKeyword))),
            method.ReturnType,
            method.Identifier,
            method.TypeParameterList,
            method.ParameterList,
            method.ConstraintClauses,
            method.Body,
            method.ExpressionBody
            );

        if (method is { Body: null, ExpressionBody: not null })
        {
            localFunction = localFunction.WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        return localFunction;
    }

    private static MemberDeclarationSyntax GenerateGlobalStatement(int part, IReadOnlyDictionary<string, ExpressionSyntax> implementations)
    {
        return GlobalStatement(GenerateStatement(part, implementations));
    }
    private static StatementSyntax GenerateStatement(int part, IReadOnlyDictionary<string, ExpressionSyntax> implementations)
    {
        return implementations.ContainsKey($"Part{part}")
                        ? LocalDeclarationStatement(
                                VariableDeclaration(
                                    IdentifierName(Identifier(TriviaList(), SyntaxKind.VarKeyword, "var", "var", TriviaList()))
                                    )
                                .WithVariables(
                                    SingletonSeparatedList(VariableDeclarator(Identifier($"part{part}"))
                                    .WithInitializer(
                                            EqualsValueClause(
                                                implementations[$"Part{part}"]
                                                ))
                                        )
                                    )
                                )
                        : ParseStatement($"var part{part} = Part{part}();\r\n")!;
    }


    public async Task ExportCode(PuzzleKey key, string code, string[]? includecommon, string output)
    {
        var codeDir = fileSystem.GetCodeFolder(key);
        var commonDir = fileSystem.GetFolder("Common");
        var outputDir = fileSystem.GetOutputFolder(output);
        var templateDir = fileSystem.GetTemplateFolder(null);
        await outputDir.CreateIfNotExists();
        await outputDir.WriteCode(code);
        outputDir.CopyFiles(
            codeDir.GetCodeFiles().Where(f => !f.Name.EndsWith("Tests.cs"))
            );
        if (codeDir.Input.Exists)
            outputDir.CopyFile(codeDir.Input);

        if (codeDir.Sample.Exists)
            outputDir.CopyFile(codeDir.Sample);

        //outputDir.CopyFile(templateDir.CsProj);
        //outputDir.CopyFile(templateDir.Helpers);

        if (includecommon is { Length: >0 } && commonDir.Exists)
        {
            await outputDir.CreateIfNotExists("Common");
            foreach (var name in includecommon)
            {
                foreach (var file in commonDir.GetFiles(Path.ChangeExtension(name, "cs")))
                {
                    outputDir.CopyFile(file, "Common");
                }
            }
        }
    }
}


public static class CSharpSingleFileHelper
{
    public static (Diagnostic[] diagnostics, CSharpCompilation compilation) Compile(CompilationUnitSyntax root)
    {
        // Extract original using names
        var originalUsingNames = root.Usings
            .Select(u => u.Name?.ToString())
            .Where(n => n is not null)
            .ToHashSet();

        // Add implicit usings that aren't already present
        string[] implicitUsingNamespaces =
        [
            "System", "System.IO", "System.Collections.Generic",
                "System.Linq", "System.Net.Http", "System.Threading",
                "System.Threading.Tasks"
        ];

        var addedImplicitUsings = implicitUsingNamespaces
            .Where(n => !originalUsingNames.Contains(n))
            .Select(n => CreateUsingDirective(n))
            .ToArray();

        var updatedRoot = root.AddUsings(addedImplicitUsings);

        // Compile with implicit usings
        var compilation = CreateCompilation(updatedRoot);

        // Filter diagnostics: exclude CS8019 warnings for implicit usings we added
        return (compilation.GetDiagnostics()
            .Where(diag => !IsRedundantImplicitUsingWarning(diag, addedImplicitUsings, updatedRoot))
            .ToArray(), compilation);
    }

    private static UsingDirectiveSyntax CreateUsingDirective(ReadOnlySpan<char> namespaceName)
    {
        Span<Range> parts = stackalloc Range[32];
        int partCount = namespaceName.Split(parts, '.');

        NameSyntax nameNode = IdentifierName(new string(namespaceName[parts[0]]));

        for (int i = 1; i < partCount; i++)
            nameNode = QualifiedName(nameNode, IdentifierName(new string(namespaceName[parts[i]])));

        return UsingDirective(nameNode);
    }

    private static CSharpCompilation CreateCompilation(CompilationUnitSyntax root)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var tree = CSharpSyntaxTree.Create(root, parseOptions);

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));

        return CSharpCompilation.Create(
            assemblyName: "Verification",
            syntaxTrees: [tree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }

    private static bool IsRedundantImplicitUsingWarning(
        Diagnostic diagnostic,
        UsingDirectiveSyntax[] addedImplicitUsings,
        CompilationUnitSyntax root)
    {
        if (diagnostic.Id != "CS8019")
            return false;

        var span = diagnostic.Location.SourceSpan;
        var usingNode = root.DescendantNodes(span)
            .OfType<UsingDirectiveSyntax>()
            .FirstOrDefault(u => u.Span == span);

        return usingNode is not null &&
               addedImplicitUsings.Any(u => u.Name?.ToString() == usingNode.Name?.ToString());
    }


    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Dynamic compilation scenario")]
    public static void Run(Compilation root)
    {
        using var pe = new MemoryStream();
        var emitResult = root.Emit(pe);

        if (emitResult.Success)
        {
            pe.Position = 0;
            var asm = Assembly.Load(pe.ToArray());
            var entry = asm.EntryPoint;
            entry!.Invoke(null, new object[] { Array.Empty<string>() });
        }
    }

    public static (Diagnostic[] diagnostics, CSharpCompilation compilation) Verify(CompilationUnitSyntax root)
        => Compile(root);
    
}
