using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LinesOfCodeAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "MiKo_Metric_0001";
        private const string Category = "Metrics";

        public int MaxLinesOfCode { get; set; } = 20;

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = LocalizableResource("Title");
        private static readonly LocalizableString MessageFormat = LocalizableResource("MessageFormat");
        private static readonly LocalizableString Description = LocalizableResource("Description");

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context) => context.RegisterCodeBlockAction(AnalyzeCodeBlock);

        private static LocalizableResourceString LocalizableResource(string suffix) => new LocalizableResourceString(nameof(LinesOfCodeAnalyzer) + "_" + suffix, Resources.ResourceManager, typeof(Resources));

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var body = GetBody(context);
            if (body is null) return;

            var diagnostic = AnalyzeBody(body, context.OwningSymbol);
            if (diagnostic is null) return;

            context.ReportDiagnostic(diagnostic);
        }

        private BlockSyntax GetBody(CodeBlockAnalysisContext context)
        {
            switch (context.CodeBlock)
            {
                case MethodDeclarationSyntax s: return s.Body;
                case ConstructorDeclarationSyntax s: return s.Body;
                case AccessorDeclarationSyntax s: return s.Body;
                default: return null;
            }
        }

        private Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol)
        {
            var loc = CountLinesOfCode(body);

            return loc > MaxLinesOfCode
                    ? Diagnostic.Create(Rule, owningSymbol.Locations.First(), owningSymbol.Name, loc, MaxLinesOfCode)
                    : null;
        }

        private static int CountLinesOfCode(SyntaxNode body)
        {
            var collector = new SyntaxNodeCollector<StatementSyntax>();
            collector.Visit(body);

            var lines = new HashSet<int>();

            foreach (var node in collector.Nodes)
            {
                CountLinesOfCode(node, lines);
            }

            return lines.Count;
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            switch (node)
            {
                case BlockSyntax _:
                    break;

                case ForEachStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    break;

                case SwitchStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    break;

                default:
                    var lineSpan = node.GetLocation().GetLineSpan();
                    lines.Add(lineSpan.StartLinePosition.Line);
                    lines.Add(lineSpan.EndLinePosition.Line);
                    break;
            }

        }
    }
}
