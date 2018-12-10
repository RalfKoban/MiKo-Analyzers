using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3051_DependencyPropertyRegisterNameAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3051";

        private const string DependencyPropertyRegisterInvocation = "DependencyProperty.Register";

        public MiKo_3051_DependencyPropertyRegisterNameAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol) => UsesStringLiterals(symbol)
                                                                                       ? new[] { ReportIssue(symbol) }
                                                                                       : Enumerable.Empty<Diagnostic>();

        private static bool UsesStringLiterals(ISymbol symbol) => symbol.DeclaringSyntaxReferences
                                                                        .Select(_ => _.GetSyntax())
                                                                        .Select(_ => _.GetEnclosing<FieldDeclarationSyntax>())
                                                                        .SelectMany(_ => _.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(__ => __.ToString() == DependencyPropertyRegisterInvocation))
                                                                        .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                        .Select(_ => _.ArgumentList.Arguments)
                                                                        .Where(_ => _.Count > 0)
                                                                        .Any(_ => _[0].Expression.Kind() is SyntaxKind.StringLiteralExpression);
    }
}