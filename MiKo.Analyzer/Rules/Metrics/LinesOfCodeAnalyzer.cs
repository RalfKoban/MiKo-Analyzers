using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
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

        private static LocalizableResourceString LocalizableResource(string suffix) => new LocalizableResourceString(nameof(LinesOfCodeAnalyzer) + suffix, Resources.ResourceManager, typeof(Resources));

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var body = GetBody(context);
            if (body != null)
            {
                AnalyzeBody(body, context);
            }
        }

        private BlockSyntax GetBody(CodeBlockAnalysisContext context)
        {
            switch (context.CodeBlock)
            {
                case MethodDeclarationSyntax m: return m.Body;
                case ConstructorDeclarationSyntax c: return c.Body;
                case AccessorDeclarationSyntax a: return a.Body;
                default: return null;
            }
        }

        static readonly string[] Separators = { Environment.NewLine };
        static readonly char[] SeparatorChars = Environment.NewLine.ToCharArray();

        private void AnalyzeBody(BlockSyntax body, CodeBlockAnalysisContext context)
        {
            var sourceText = body.WithoutTrivia().GetText();

            // TODO: RKN Fix performance issue of creating new objects
            var lines = sourceText.ToString().Trim(SeparatorChars).Split(Separators, StringSplitOptions.None);
            var loc = lines.Length;
            if (loc > 0)
            {
                var firstLine = lines[0];
                var lastLine = lines.Last();
                if (firstLine.Contains('{') && string.IsNullOrWhiteSpace(firstLine.Replace('{', ' '))) loc--;
                if (lastLine.Contains('}') && string.IsNullOrWhiteSpace(lastLine.Replace('}', ' '))) loc--;
            }

            if (loc > MaxLinesOfCode)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.OwningSymbol.Locations.First(), context.OwningSymbol.Name, loc, MaxLinesOfCode));
            }
        }
    }
}
